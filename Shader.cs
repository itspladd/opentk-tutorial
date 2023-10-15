// Alright baby let's make a shader class
// This will be responsible for pulling in our shader source files
// Plus other functions later (ooh mysterious)
using OpenTK.Graphics.OpenGL4;

public class Shader {
  // This Handle var will represent the location of the shader program after it's compiled.
  int Handle;

  // This bool represents whether or not the shader has been properly deleted before the program ends.
  // Because otherwise the memory won't be freed.
  private bool disposed = false;

  public Shader(string vertexSourcePath, string fragmentSourcePath) {
    // More handles. These will represent the location of each one after compilation(?).
    int VertexShaderHandle;
    int FragmentShaderHandle;

    // New function call! Low-level stuff: read all the text from an input file,
    // and save it into a string. So we just have a big old string with all the source code.
    // Gonna print it to the console because:
    // 1. I want to see it, and
    // 2. I cannot be stopped.
    string VertexShaderSource = File.ReadAllText(vertexSourcePath);
    string FragmentShaderSource = File.ReadAllText(fragmentSourcePath);

    // Now we generate the shader objects in GL and save their handles to our ints...
    VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
    FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

    // ...and straight-up plug the source code into them.
    // (and it looks like you can re-do this operation later,
    // if you want to change the source for a given shader.)
    GL.ShaderSource(VertexShaderHandle, VertexShaderSource);
    GL.ShaderSource(FragmentShaderHandle, FragmentShaderSource);

    // Ok now let's try compiling them.
    GL.CompileShader(VertexShaderHandle);
    GL.CompileShader(FragmentShaderHandle);

    // ...and check for errors.
    GL.GetShader(VertexShaderHandle, ShaderParameter.CompileStatus, out int getShaderSuccess);
    if (getShaderSuccess == 0)
    {
      string infoLog = GL.GetShaderInfoLog(VertexShaderHandle);
      Console.WriteLine(infoLog);
    }

    // Now we create a program handle. From here on, a "shader" is a GPU-runnable program.
    Handle = GL.CreateProgram();

    // And attach the compiled shader source codes to the program
    GL.AttachShader(Handle, VertexShaderHandle);
    GL.AttachShader(Handle, FragmentShaderHandle);

    // ...and link it all together? Not really sure.
    // ah, ok, this actually puts the compiled shaders into the program after they were attached.
    GL.LinkProgram(Handle);

    // ...and try to load it, and check for errors again.
    GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int getProgramSuccess);
    if (getProgramSuccess == 0)
    {
      string infoLog = GL.GetProgramInfoLog(Handle);
      Console.WriteLine(infoLog);
    }

    // Now we can clean up. The shader source code isn't needed anymore since it's linked
    GL.DetachShader(Handle, VertexShaderHandle);
    GL.DetachShader(Handle, FragmentShaderHandle);
    GL.DeleteShader(VertexShaderHandle);
    GL.DeleteShader(FragmentShaderHandle);
  }

  public void Use() {
    GL.UseProgram(Handle);
  }

  // Frees up the memory used by this shader, and sets the flag saying it's OK to clean up.
  protected virtual void Dispose(bool disposing) {
    if(!disposed)
    {
      GL.DeleteProgram(Handle);

      disposed = true;
    }
  }

  // Finalizer method, which logs a warning to the console if we forget to Dispose of the Shader first
  ~Shader() {
    if (disposed == false)
    {
      Console.WriteLine("GPU Resource Leak! Hey dingus you forgot to Dispose() of a Shader");
    }
  }

  // And the publicly callable version of our Disposal method.
  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}
// Alright baby let's make a shader class
// This will be responsible for pulling in our shader source files
// Plus other functions later (ooh mysterious)
using OpenTK.Graphics.OpenGL4;

public class Shader {
  // This Handle var will represent the location of the shader program after it's compiled.
  int Handle;

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

    Console.WriteLine("Vertex shader:");
    Console.WriteLine(VertexShaderSource);
    Console.WriteLine("Fragment shader:");
    Console.WriteLine(FragmentShaderSource);

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
    GL.GetShader(VertexShaderHandle, ShaderParameter.CompileStatus, out int success);
    if (success == 0)
    {
      string infoLog = GL.GetShaderInfoLog(VertexShaderHandle);
      Console.WriteLine(infoLog);
    }
  }
}
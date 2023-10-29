using System.Runtime.InteropServices;
using System.Diagnostics;
using LoggerUtil;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameSpace
{
  public readonly struct GameOptions {
      public GameOptions(LogLevel level) {
        // Set debug mode for the game if the log level is high enough
        debugMode = level >= LogLevel.DEBUG;
      }
      public readonly bool debugMode;
  }

  public class Game : GameWindow
  {
    public enum Geometry {
      TRIANGLE,
      RECTANGLE
    }

    public Game(int width, int height, string title, Logger logger, GameOptions options) : base(
      GameWindowSettings.Default,
      new NativeWindowSettings() { 
        Size = (width, height),
        Title = title,
        Flags = ContextFlags.Debug,
      }
    ) {
      logger.Debug("Initializing Game class");
      this.logger = logger;
      this.options = options;
    }
    private Logger logger;

    private Stopwatch _timer;

    private GameOptions options;
    private int FrameCount = 0;
    private const int START_LOG_FRAME = 0;
    private const int END_LOG_FRAME = 10;
    private static DebugProc DebugMessageDelegate = OnDebugMessage;

    float[] vertices;
    uint[] indices;

    float[] textureCoords;

    // Ints to store the IDs of vertex and element buffer objects (VBOs and EBOs)
    int VertexBufferObject;
    int ElementBufferObject;

    // Int to store the ID of a vertex array object (VAO)
    int VertexArrayObject;

    // Shader property
    // ? to mark it as nullable
    Shader shader;

    private Geometry currentGeom;
  
    // OnLoad runs once, when the window opens. Initialization code goes here.
    protected override void OnLoad() {
      base.OnLoad();
      LogGLInformation();
      GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
      GL.Enable(EnableCap.DebugOutput);
            
      // Decides what color the window should be after it gets cleared between frames
      GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
      currentGeom = Geometry.RECTANGLE;
      SetGeometryData(currentGeom);

      InitVertexBuffer(vertices);

      if (currentGeom == Geometry.RECTANGLE) {
        InitElementBuffer(indices);
      }

      // Hey let's try loading our shader
      shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag", logger);
      shader.Use();

      // Start our stopwatch
      _timer = new Stopwatch();
      _timer.Start();
    }

    // ..:: Initialization for a single element buffer. ::..
    // Note that we can only bind an ElementArrayBuffer if there is a VAO bound already!
    // It seems like the ElementArrayBuffer is "owned" by the VAO that's bound when we bind the EBO.
    protected void InitElementBuffer(uint[] indices) {
      // Similar code/process as the vertex buffer init!
      ElementBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
      GL.BufferData(
        BufferTarget.ElementArrayBuffer,
        indices.Length * sizeof(uint),
        indices,
        BufferUsageHint.StaticDraw
      );

    }

    protected override void OnUnload()
    {
      base.OnUnload();

      // Only dispose of the shader if it's not null
      shader.Dispose();
    }

    // Pretty self-explanatory: runs when the window gets resized.
    protected override void OnResize(ResizeEventArgs e) {
      base.OnResize(e);

      GL.Viewport(0, 0, e.Width, e.Height);
    }

    //
    protected override void OnUpdateFrame(FrameEventArgs e) {
      base.OnUpdateFrame(e);

      if (KeyboardState.IsKeyDown(Keys.Escape)) {
        Close();
      }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      if (options.debugMode && FrameCount < END_LOG_FRAME && FrameCount > START_LOG_FRAME) {
        RenderWithDebugLogs(e);
        return;
      }
      base.OnRenderFrame(e);
      GL.Clear(ClearBufferMask.ColorBufferBit);
      
      // Must call Use() on the shader before UPATING uniform vars (you can try to access them beforehand?!)
      shader.Use();

      // Set the uniform var based on current time
      double elapsedTime = _timer.Elapsed.TotalSeconds;
      float greenVal = (float)Math.Sin(elapsedTime) / 2.0f + 0.5f;

      // Retrieve the location of the currentColor Uniform!
      //int vertexColorLocation = GetUniform("currentColor");

      // Uniform4 means "expects 4 values". There are other overloads!
/*       if (vertexColorLocation != -1) {
        GL.Uniform4(vertexColorLocation, 0.0f, greenVal, 0.0f, 1.0f);
      } */
      //GL.BindVertexArray(VertexArrayObject);
      
      if (currentGeom == Geometry.TRIANGLE) {
        GL.DrawArrays(PrimitiveType.Triangles,0,3);
      }
      if (currentGeom == Geometry.RECTANGLE) {
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
      }

      SwapBuffers();
      FrameCount++;
    }

    protected int GetUniform(string uniformName) {
      int vertexColorLocation = GL.GetUniformLocation(shader.Handle, uniformName);
      if (vertexColorLocation == -1) {
        logger.Error($"GL.GetUniformLocation could not locate Uniform '{uniformName}'. Are you using the right shader?");
      }

      return vertexColorLocation;
    }

    protected void RenderWithDebugLogs(FrameEventArgs e) {
      logger.Debug(string.Format("-----  START FRAME {0}  -----", FrameCount));

      base.OnRenderFrame(e);
      logger.Debug("base.OnRenderFrame complete");
      
      // Call this first (well, after the base call) when rendering.
      // Apparently ClearBufferMask.ColorBufferBit has teh color we specified in GL.ClearColor?
      GL.Clear(ClearBufferMask.ColorBufferBit);
      logger.Debug("GL.Clear complete");
      
      // Now we can actually do stuff.

      // Assuming we have an initialized VertexArrayObject...
      // ...and assuming we have a shader ready to go...
      // then we can draw what's in our VertexArrayObject?!?!
      shader.Use();
      logger.Debug("shader.Use() complete");

      // Set the uniform var based on current time
      double elapsedTime = _timer.Elapsed.TotalSeconds;
      float greenVal = (float)Math.Sin(elapsedTime) / 2.0f + 0.5f;

      // Retrieve the location of the currentColor Uniform!
      int vertexColorLocation = GetUniform("currentColor");
      if (vertexColorLocation == -1) {
        GL.Uniform4(vertexColorLocation, 0.0f, greenVal, 0.0f, 1.0f);
      }

      logger.Debug($"Drawing {currentGeom}");
      if (currentGeom == Geometry.TRIANGLE) {
        GL.DrawArrays(PrimitiveType.Triangles,0,3);
      }
      if (currentGeom == Geometry.RECTANGLE) {
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
      }      
      logger.Debug("DrawArrays complete");

      // Now: switch the buffers.
      // OpenGL renders to one buffer while displaying a second buffer.
      // So we swap them: we are now displaying whatever was previously rendered,
      // and (I presume) rendering over what was previously displayed
      SwapBuffers();
      logger.Debug("Buffers swapped");

      logger.Debug(string.Format("-----    END FRAME {0}  -----", FrameCount));
      FrameCount++;
    }

    private void SetGeometryData(Geometry geomType) {
      // Array literals containing x, y, and z points as floats.
      // Note that they are a one-dimensional arrays! They're just formatted to look like 3xN arrays.
      // These coordinates are normalized: they must fall within the -1.0f to 1.0f range.
      // Anything outside that range will be clipped!
      // Normalized coordinates get converted to screen-space coordinates
      // using the Viewport information.

      // We can include both color and position data!
      // This corresponds with a change in our shader: we set a new 'layout (location = 1)' variable. 
      float[] triangleVertices = {
        // positions        // colors
        -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // v0
         0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // v1
         0.0f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // v2
      };

      float[] rectangleVertices = {
        // positions        // colors
         0.5f,  0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // top right
         0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f, 1.0f, // top left
      };

      // For an EBO, we need to specify which triangles map to which vertices!
      // This will draw our rectangle.
      uint[] rectangleIndices = {
        0, 1, 3,
        1, 2, 3,
      };

      if (geomType == Geometry.TRIANGLE) {
        vertices = triangleVertices;
      }

      if (geomType == Geometry.RECTANGLE) {
        vertices = rectangleVertices;
        indices = rectangleIndices;
      }

    }

        // ..:: Initialization code for a single vertex array. ::..
    // (We only do this once, unless the object will change often.)
    protected void InitVertexBuffer(float[] vertices) {
      // 1. Generate the VBO and VAO and assign their IDs to the global ints we created.
      VertexArrayObject = GL.GenVertexArray();
      VertexBufferObject = GL.GenBuffer();

      // 2. Bind it
      GL.BindVertexArray(VertexArrayObject);

      // 3. Copy our vertices into our global VBO (bind it first!)
      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

      // 4. Set the vertex attribute pointers (0: position, 1: color)
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

      // 5. Enable the attributes that we want to use
      GL.EnableVertexAttribArray(0);
      GL.EnableVertexAttribArray(1);
    }

    // Same code as InitVertexArray, but with detailed notes at each step.
    protected void InitVertexBufferWithNotes(float[] vertices) {

      // Here we're creating a Vertex Array Object (VAO) and Vertex Buffer Object (VBO) and binding them.
      // This is a becoming a common pattern!
      // Each of these is just an int pointing to the VAO/VBO in memory.
      VertexArrayObject = GL.GenVertexArray();
      VertexBufferObject = GL.GenBuffer();
      
      // Bind the VAO.
      GL.BindVertexArray(VertexArrayObject);
      // Binding the VAO sneakily influences a lot of things.
      // The currently bound VAO saves:
      // - GL.EnableVertexAttribArray calls
      // - GL.DisableVertexAttribArray calls
      // - GL.VertexAttribPointer configurations
      // - The currently bound VBO when GL.VertexAttribPointer is called
      // ...So we bind it during initialization, but we might also re-bind it many times during init and rendering to point to different VAOs.
      
      // Binds the VertexBufferOjbect to the ArrayBuffer target.
      // Now when we do anything to ArrayBuffer, we're configuring VertexBufferObject.
      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

      // And now we copy our data into the buffer bound to ArrayBuffer.
      // Note that we specify the byte size of the data in the second param!
      // Then we pass in the data itself and a hint that helps manage the data:
      //  - StaticDraw: data will change rarely or not at all
      //  - DynamicDraw: data is likely to change often
      //  - StreamDraw: data will change every time it's drawn
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        vertices.Length * sizeof(float),
        vertices,
        BufferUsageHint.StaticDraw);

      // This function call lets you omit the layout (location = 0) code in the shader,
      // and allows the next function to use the aPosition var instead of a hard-coded 0.
      // But it also adds overhead, so we're omitting it for now.
      // int aPosition = shader.GetAttribLocation("aPosition");
      
      // Alright, this is a big function call, so time for some long-form notes.
      // This function tells OpenGL *how* to interpret the vertex data, on a per-vertex-attribute basis.
      GL.VertexAttribPointer(
        // index (int):
        // Which attribute are we configuring? In this case, we're configuring the position.
        // We specified this in the shader! This line:
        ////   layout (location = 0) in vec3 aPosition;
        // So this attribute is at at location 0.
        // If we used GetAttribLocation above, we use the result instead of hard-coding a 0 here.
        0, //aposition, 

        // size (int):
        // since it's a vec3, then the size is 3 (x, y, and z floats));
        3, 
        
        // type (enum): 
        // the data is float data, so we use the appropriate enum.
        VertexAttribPointerType.Float, 
        // normalized (bool): 
        // this means "do you want me to normalize this?"
        // it does NOT mean "is this data normalized already?"
        // since our data is already normalized, we don't want GL to normalize it for us.
        false,
        // stride (int): how big is the space between each attribute?
        // When we were just passing position data, position was the only attribute, so it's 3 floats big.
        // 3 * sizeof(float),
        // Now that we're passing color attribute data as well, each attribute is further apart - twice as far!
        6 * sizeof(float),
        // offset (int):
        // Where in the buffer does this data start?
        // Since we're at the very beginning, it's just 0. (This will be explored more later.)
        0
      );

      // Since we have color data now, we need to init another pointer!
      // It's very similar, but this time the attribute is at location 1...
      GL.VertexAttribPointer(
        1, // The attribute is at location 1...
        3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
        3 * sizeof(float) // and the offset is 3 floats big (since there's 3 position floats in front of it!)
      );


      // Each vertex takes its data from memory managed by a VBOs.
      // We can have multiple VBOs in existence!
      // The current VBO is the VBO bound to ArrayBuffer *when we call VertexAttribPointer*.
      // So our VertexAttribPointer will be taking the data from the VBO that we bound earlier.

      // Now we have to enable vertex attributes (which are...disabled by default? huh?)
      // And we have to specify the index/location.
      // This matches up with the location in the shader.
      GL.EnableVertexAttribArray(0);
      GL.EnableVertexAttribArray(1); // Once we add the color data, gotta enable that too!
    }

    protected void LogGLInformation() {
      // Log some GL values for troubleshooting
      GL.GetInteger(GetPName.MaxVertexAttribs, out int numMaxVertexAttribs);
      string glInfoString = "Logging GL info:" +
        "\n** Renderer: " + GL.GetString(StringName.Renderer) +
        "\n** Version: " + GL.GetString(StringName.Version) +
        "\n** ShadingLanguageVersion: " + GL.GetString(StringName.ShadingLanguageVersion) +
        "\n** Maximum num of vertex attributes supported: " + numMaxVertexAttribs;
      logger.Debug(glInfoString);
    }

    protected void KillBuffer(BufferTarget target, int bufferObjectRef) {
      // We don't need to manually delete buffers to free memory when the program ends.
      // The program will clean up after itself without us worrying about manual deletion.
      // But, if we ever WANT to manually delete a buffer...
      // (for...reasons? the tutorial suggests "limiting VRAM usage" as a
      // potential reason, but I need to learn more about that)
      // ...then this is the way (or at least *a* way) to do it.
      
      // First, bind the BufferTarget to 0. This way, if we try to call this buffer
      // without re-binding it first, then we crash.  Which is apparently easier
      // to debug than accidentally modifying a supposedly-deleted buffer.
      // Then we actually delete the buffer object at the ref.
      GL.BindBuffer(target, 0);
      GL.DeleteBuffer(bufferObjectRef);

      // For reference, here's the version of this code if we used the specific
      // buffers/variables from the tutorial:
      // GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
      // GL.DeleteBuffer(VertexBufferObject)

      // Note that we're not changing the value of VertexBufferObject (or bufferObjectRef)!
      // It will still have its buffer ID; but GL won't have a bufffer with that ID.
    }

    private static void OnDebugMessage(
        DebugSource source,     // Source of the debugging message.
        DebugType type,         // Type of the debugging message.
        int id,                 // ID associated with the message.
        DebugSeverity severity, // Severity of the message.
        int length,             // Length of the string in pMessage.
        IntPtr pMessage,        // Pointer to message string.
        IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
    {
      // In order to access the string pointed to by pMessage, you can use Marshal
      // class to copy its contents to a C# string without unsafe code. You can
      // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
      string message = Marshal.PtrToStringAnsi(pMessage, length);

      // The rest of the function is up to you to implement, however a debug output
      // is always useful.
      Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

      // Potentially, you may want to throw from the function for certain severity
      // messages.
      if (type == DebugType.DebugTypeError)
      {
          throw new Exception(message);
      }
    }

  }
}
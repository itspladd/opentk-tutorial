using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameSpace
{
  public class Game : GameWindow
  {
    public Game(int width, int height, string title) : base(
      GameWindowSettings.Default,
      new NativeWindowSettings() { Size = (width, height), Title = title}
    ) {

    }

    // Array literal containing x, y, and z points as floats.
    // Note that it's a one-dimensional array! It's just formatted to look like a 3x3.
    // These coordinates are normalized: they must fall within the -1.0f to 1.0f range.
    // Anything outside that range will be clipped!
    // Normalized coordinates get converted to screen-space coordinates
    // using the Viewport information.
    float[] testVertices = {
      -0.5f, -0.5f, 0.0f,
       0.5f, -0.5f, 0.0f,
       0.0f,  0.5f, 0.0f 
    };

    // Int to store the ID of a vertex buffer object (VBO)
    int VertexBufferObject;

    // Shader property
    // ? to mark it as nullable
    Shader? shader;
  
    // OnLoad runs once, when the window opens. Initialization code goes here.
    protected override void OnLoad() {
      base.OnLoad();

      // Hey let's try loading our shader
      shader = new Shader("shader.vert", "shader.frag");

      // Decides what color the window should be after it gets cleared between frames
      GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

      // Generate a buffer and assign its ID to the int we created.
      VertexBufferObject = GL.GenBuffer();

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
        testVertices.Length * sizeof(float),
        testVertices,
        BufferUsageHint.StaticDraw);

      // Here we're creating a Vertex Array Object (VAO) and bind it.
      // This is a becoming a common pattern!
      int VertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(VertexArrayObject);

      // Alright, this is a big function call, so time for some long-form notes.
      // This function tells OpenGL *how* to interpret the vertex data, on a per-vertex-attribute basis.
      GL.VertexAttribPointer(
        // index (int):
        // Which attribute are we configuring? In this case, we're configuring the position.
        // We specified this in the shader! This line:
        ////   layout (location = 0) in vec3 aPosition;
        // So this attribute is at at location 0.
        0,
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
        3 * sizeof(float),
        // offset (int):
        // Where in the buffer does this data start?
        // Since we're at the very beginning, it's just 0. (This will be explored more later.)
        0
      );

      // Each vertex takes its data from memory managed by a VBOs.
      // We can have multiple VBOs in existence!
      // The current VBO is the VBO bound to ArrayBuffer *when we call VertexAttribPointer*.
      // So our VertexAttribPointer will be taking the data from the VBO that we bound earlier.
    }

    protected override void OnUnload()
    {
      base.OnUnload();

      // Only dispose of the shader if it's not null
      shader?.Dispose();
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
      base.OnRenderFrame(e);

      // Call this first (well, after the base call) when rendering.
      // Apparently ClearBufferMask.ColorBufferBit has teh color we specified in GL.ClearColor?
      GL.Clear(ClearBufferMask.ColorBufferBit);

      // Now we can actually do stuff.

      // First off: switch the buffers.
      // OpenGL renders to one buffer while displaying a second buffer.
      // So we swap them: we are now displaying whatever was previously rendered,
      // and (I presume) rendering over what was previously displayed
      SwapBuffers();
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
  }
}
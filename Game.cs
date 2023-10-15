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
      new NativeWindowSettings() { 
        Size = (width, height),
        Title = title,
        Flags = ContextFlags.Debug
      }
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

    // Int to store the ID of a vertex array object (VAO)
    int VertexArrayObject;

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

      InitVertexArray(testVertices);

      shader?.Use();
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

      // Assuming we have an initialized VertexArrayObject...
      // ...and assuming we have a shader ready to go...
      // then we can draw what's in our VertexArrayObject?!?!
      
      // This code is breaking something!
      shader?.Use();
      GL.BindVertexArray(VertexArrayObject);
      GL.DrawArrays(PrimitiveType.Triangles,0,3);
      /////

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
    
    // ..:: Initialization code for a single vertex array. ::..
    // (We only do this once, unless the object will change often.)
    protected void InitVertexArray(float[] vertices) {
      // 1. Generate the array and set it to the global param
      VertexArrayObject = GL.GenVertexArray();

      // 2. Bind it
      GL.BindVertexArray(VertexArrayObject);

      // 3. Copy our vertices into our global VBO (bind it first!)
      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, 3 * sizeof(float), vertices, BufferUsageHint.StaticDraw);

      // 4. Set the vertex attribute pointers
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);

    }

    // Same code as InitVertexArray, but with detailed notes at each step.
    protected void InitVertexArrayWithNotes(float[] vertices) {

      // Here we're creating a Vertex Array Object (VAO) and bind it.
      // This is a becoming a common pattern!
      VertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(VertexArrayObject);
      
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

      // Now we have to enable vertex attributes (which are...disabled by default? huh?)
      // And we have to specify the index.
      GL.EnableVertexAttribArray(0);
    }
  }
}
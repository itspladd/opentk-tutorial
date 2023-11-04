//PLADD's FIRST FRAGMENT SHADER
//AND SECOND SHADER EVER HA HA WHEE

//(.frag for fragment shader!)

// Note to self: 'glxinfo' command showed me the OpenGL version I'm running.
#version 300 es

// INPUTS/OUTPUTS
// Fragment shaders must declare an output color for their fragments.
// Otherwise, it will be undefined (which will probably mean they're either black or white)

// We can also pass an output of an earlier shader to this one by using the same var name/type.
// (More about this in shader.vert notes!)


// INPUT
//in mediump vec3 vertexColor; // if passing in color from vertex
in mediump vec2 textureCoordinates; // if passing in texture coords from vertex

// OUTPUT
// Output color variable. It's vec4 because it'll hold 4 floats: the RGBA values.
out mediump vec4 FragColor;

// UNIFORM
// Uniform vars are unique and global across an entire shader program: i.e. all the linked shaders!
// So we can declare a uniform in one shader and access it later in another.
// If we don't use it, it will be silently removed from the code, which can cause weird errors!
// UNIFORMS CAN BE ACCESSED OUTSIDE THE SHADER ITSELF! SO WE CAN PASS VALUES FROM EXTERNAL CODE.
// We'll be grabbing and changing this uniform var inside Game.cs!

// Commented out for round 3, when we moved to having color data input with the vertex data.
//uniform mediump vec4 currentColor;

// *** TEXTURES ***
// Once a texture's pixels are loaded (using functions in our Texture class),
// We need to tell the shader what textures it has access to.
// OpenGL requires that 16 texture "units" (essentially slots) be available at the same time.
// Some systems might support more than 16!
// Check out this stackoverflow answer: https://stackoverflow.com/questions/46426331/number-of-texture-units-gl-texturei-in-opengl-4-implementation-in-visual-studi

// Textures use the sampler2D type (TODO: always? or is that just the simplest case?)
// "texture#" is the default naming for these uniforms. (Is it just the default? Can it be changed?)
uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
// Each of these will have an integer value assigned.
// The integer value tells that variable WHICH TEXTURE "UNIT" TO USE.


void main()
{
  // *** Round 1 ***
  // We're just making it orange. No further processing. Baby steps.
  // FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);

  // *** Round 2 ***
  // Now we try to use the input from the vertex shader.
  //FragColor = vertexColor;

  // *** Round 3 ***
  // *** Using uniform var
  //FragColor = vec4(vertexColor, 1.0);

  // *** Round 4 ***
  // *** Texture!
  // Execution here is relatively simple. We tell the fragment shader
  // to use the new sampler2D uniform to determine the color.
  // We lso use the coordinates from the vertex shader.
  // But...where does that uniform actually get set? HMMM.
  // oh ha ha that's literally the next lesson
  // 0 is the default texture "unit"
  //FragColor = texture(texture0, textureCoordinates);

  // *** Round 5 ***
  // *** MIXING TEXTURES? WHAT. WHY.
  FragColor = mix( // Built-in GLSL function. Mixes two textures.
    texture(texture0, textureCoordinates), // First texture in the mix
    texture(texture2, textureCoordinates), // Second texture in the mix
    0.2 // Weighting for the lerp between the textures.
        // This value is the amount of the SECOND texture to use.
        // So 0.2 is 80% texture 1, 20% texture 2.
  );
}
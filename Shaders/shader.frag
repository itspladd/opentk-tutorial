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
in mediump vec4 vertexColor;

// OUTPUT
// Output color variable. It's vec4 because it'll hold 4 floats: the RGBA values.
out mediump vec4 FragColor;

void main()
{
  // Round 1: We're just making it orange. No further processing. Baby steps.
  // FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);

  // Round 2: Now we try to use the input from the vertex shader.
  FragColor = vertexColor;
}
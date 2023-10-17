//PLADD's FIRST FRAGMENT SHADER
//AND SECOND SHADER EVER HA HA WHEE

//(.frag for fragment shader!)

// Note to self: 'glxinfo' command showed me the OpenGL version I'm running.
#version 420 core

//Declare output variable
//It's vec4 because it'll hold 4 floats: the RGBA values.
out vec4 FragColor;

void main()
{
  //We're just making it orange. No further processing. Baby steps.
  FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}
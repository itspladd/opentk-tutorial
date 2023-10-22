//PLADD'S FIRST VERTEX SHADER
//AND FIRST SHADER EVER
//ha ha let's hope this works

//(.vert file extension, for vertex shader!)

//Declare version and "core profile" (??)
#version 300 es

//ha ha what
//okay so this is a vector with length three to hold the vertices
in vec3 aPosition;

//Every shader has a main function
void main()
{
  //gl_Position is a built-in vertex shader varible.
  //It represents the final position of a vertex.
  //We're not processing any input data, just forwarding the input to the output.
  //So this is, like, the simplest vertex shader imaginable.
  //...according to the OpenTK docs.
  gl_Position = vec4(aPosition, 1.0);
}
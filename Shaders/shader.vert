//PLADD'S FIRST VERTEX SHADER
//AND FIRST SHADER EVER
//ha ha let's hope this works

//(.vert file extension, for vertex shader!)

//Declare version and "core profile" (??)
#version 300 es

// Alright let's take some better notes here.

// INPUTS/OUTPUTS
// Shaders pass their outputs to the inputs of the next shader.
// You define an output in one shader, and then a corresponding input in the next.
// When the names and types of those input/outputs match, they get linked by OpenGL.

// Vertex shaders have an exception, though, since they're the first in the pipeline.
// They need vertex data input. And this isn't coming from another shader,
// so we need extra layout info to tell the shader where to look.
// The location must match with arguments in GL.VertexAttribPointer and GL.EnableVertexAttribArray calls.

// INPUT
layout (location = 0) in vec3 aPosition;

// OUTPUT
// We're gonna be silly here and use the vertex shader to define the fragment shader's color.
out vec4 vertexColor;

//Every shader has a main function
void main()
{
  // gl_Position is a built-in vertex shader varible.
  // It represents the final position of a vertex.
  // We're not processing any input data, just forwarding the input to the output.
  // So this is, like, the simplest vertex shader imaginable.
  // ...according to the OpenTK docs.
  gl_Position = vec4(aPosition, 1.0); // Look! We're using a vector in a constructor! Like in the example notes!

  // Theoretically we should be able to mess up some stuff if we swizzle...let's try it.
  // gl_Position = vec4(aPosition.yxz, 1.0); // Swap x and y for science?
  // YUP the rectangle flipped its dimensions. Fun.

  // Round 1: Send a dark red to the fragment shader.
  vertexColor = vec4(0.5, 0.0,  0.0, 1.0);

}
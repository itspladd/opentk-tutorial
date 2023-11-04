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
//layout (location = 1) in vec3 aColor; // if passing in color data
layout (location = 1) in vec2 aTextureCoordinates;

// OUTPUT
// We're gonna be silly here and use the vertex shader to define the fragment shader's color.
//out vec3 vertexColor;

// Send texture coordinates to fragment shader
out vec2 textureCoordinates;

// *** UNIFORMS ***
// Transformation matrix. Defined in code and passed in through uniform variable.
uniform mat4 transform;


//Every shader has a main function
void main()
{
  // *** BASIC USE CASE ***
  // gl_Position is a built-in vertex shader varible.
  // It represents the final position of a vertex.
  // We're not processing any input data, just forwarding the input to the output.
  // So this is, like, the simplest vertex shader imaginable.
  // ...according to the OpenTK docs.
  //gl_Position = vec4(aPosition, 1.0); // Look! We're using a vector in a constructor! Like in the example notes!

  // *** SWIZZLING THE VERTEX POSITIONS ***
  // Theoretically we should be able to mess up some stuff if we swizzle...let's try it.
  // gl_Position = vec4(aPosition.yxz, 1.0); // Swap x and y for science?
  // YUP the rectangle flipped its dimensions. Fun.

  // *** APPLYING TRANSFORMATIONS ***
  // Simple case first: just turn the vertex positions into a vec4 and multiply by the transformation matrix. 
  gl_Position = vec4(aPosition, 1.0) * transform;

  // *** Round 1 ***
  // *** Send a dark red to the fragment shader.
  //vertexColor = vec4(0.5, 0.0,  0.0, 1.0);

  // *** Round 2 ***
  // *** What happens if we base the fragment shader's color on the vertex data?
  //vertexColor = vec4(aPosition, 1.0);
  // OH HECK COOL WE GOT A GRADIENT.
  // IT's mostly black, starting from the center and also all throughout quadrant 3.
  // So if we do an absolute value of the inputs, can we get a full radial gradient?
  //vec3 test = absolute(aPosition);
  //vertexColor = vec4(test, 1.0);
  
  // Okay that's doing something strange where the colors are (mostly) all the same.
  // The color makes sense SOMEWHAT: if the abs of each vertex is (0.5, 0.5) then of course the colors will be the same
  // So maybe since they're all identical, the color lerp doesn't happen?
  // So let's try an explicit function to set the color based on the quadrant of the vertex.
  
  //vertexColor = vec4(radialColor(aPosition), 1.0);
  
  // HAHA I GOT A COOL RAINBOW GRADIENT. HELL YES.
  // Note: I had a weird issue! I was accidentally drawing an extra triangle that had an undefined vertex!
  // So it was using 0, 0 for that vertex and creating a weird break in the gradient.

  // *** Round 3 ***
  // *** Bring color data in with the vertex position data, and pass it to the fragment shader.
  //vertexColor = aColor;

  // *** Round 4 ***
  // *** Bring texture coordinate data in with the position data, and pass it to the fragment shader
  textureCoordinates = aTextureCoordinates;
}

// A helper function I wrote that returns an "absolute" vector of an input vec3.
// i.e. each component of the output is the absolute value of the input component.
vec3 absolute(vec3 inputVec) {
  vec3 returnVec = vec3(0.0,0.0,0.0);
  for(int i = 0; i < 3; i++) {
    float iVal = inputVec[i];
    iVal = iVal * sign(iVal);
    returnVec[i] = iVal;
  }
  return returnVec;
}

// A helper function I wrote to determine the quadrant for an input 2D vector.
int quadrant(vec2 position) {
  float sX = sign(position.x);
  float sY = sign(position.y);
  if (sX == 1.0 && sY == 1.0) {
    return 1;
  }
  if (sX == -1.0 && sY == 1.0) {
    return 2;
  }
  if (sX == -1.0 && sY == -1.0) {
    return 3;
  }
  if (sX == 1.0 && sY == -1.0) {
    return 4;
  }

  return 0;
}

// A helper function I wrote to return an RGB color based on the quadrant.
vec3 radialColor(vec3 inputVec) {
  // Red in q1 (top right), yellow in ( q2top left), blue in q3 (bottom left), green in q4 (bottom right).
  vec3 returnRGB;
  int q = quadrant(inputVec.xy);
  if (q == 1) {
    returnRGB = vec3(1.0, 0.0, 0.0);
  }
  else if (q == 2) {
    returnRGB = vec3(1.0, 1.0, 0.0);
  }
  else if (q == 3) {
    returnRGB = vec3(0.0, 0.0, 1.0);
  }
  else if (q == 4) {
    returnRGB = vec3(0.0, 1.0, 0.0);
  }
  else {
    returnRGB = vec3(1.0, 1.0, 1.0);
  }
  return returnRGB;
}
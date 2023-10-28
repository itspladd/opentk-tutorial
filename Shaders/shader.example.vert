// Example/template shader code
#version version_number

// Types!
in type in_variable_name;
in type in_variable_name;

//// Vectors! Replace n with a number 1-4 when using 'em
in vecn  vector_of_n_floats;
in bvecn vector_of_n_booleans;
in ivecn vector_of_n_integers;
in uvecn vector_of_n_unsigned_ints;
in dvecn vector_of_n_doubles;

// GLSL lets you do some funky stuff with accessing vectors.
// They have x, y, z, and w components (depending on the vector's size)
// and you can access them to define other vectors! 
// This is called 'swizzling', which is hilarious.
vec2 two_float_vec
vec4 swizzled_vec = two_float_vec.xyxx
vec3 second_swizzled = swizzled_vec.zyw
vec4 computed_swizzled = two_float_vec.xxxx + second_swizzled.yzxy

// You can also use them in vector construction calls.
vec2 newVec = vec2(0.3, 0.8)
vec3 newConstructed = vec3(newVec, 0.0)
vec4 newSwizzled = (newConstructed.zx, newVec.yy)

out type out_variable_name;
  
uniform type uniform_name;
  
void main()
{
  // process input(s) and do some weird graphics stuff
  ...
  // output processed stuff to output variable
  out_variable_name = weird_stuff_we_processed;
}
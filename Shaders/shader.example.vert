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


out type out_variable_name;
  
uniform type uniform_name;
  
void main()
{
  // process input(s) and do some weird graphics stuff
  ...
  // output processed stuff to output variable
  out_variable_name = weird_stuff_we_processed;
}
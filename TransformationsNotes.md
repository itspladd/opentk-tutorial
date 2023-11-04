# Transformations

Translations are a bit wild, and involve linear algebra. HUZZAH.

I'm not going to take extensive notes on the mathematics underlying everything, since that's easily referenced. Instead, these notes wil be more about the idea of transformations as they apply to graphics.

As a foundational point, remember:

- Vectors are single-column matrices
- Graphics applications tend to use 4x4 matrices for transformations
- Matrices are indexed in `(i,j)` notation. `i` is the row, `j` is the column.
- Note that that notation is flipped vertically from the `(x,y)` graph notation; i.e. `j` starts at the top and goes down, whereas `y` starts at the bottom and goes up. 

## Identity Matrix

This is the `NxN` matrix `I` that, when multiplied by a `4xN` matrix `J`, leaves `J` exactly the same as it was:

`I•J = J`

An `NxN` identity matrix simply has 1 where `i=j`, and 0 everywhere else. So the 4x4 identity matrix is:

```
| 1 0 0 0 |
| 0 1 0 0 | 
| 0 0 1 0 |
| 0 0 0 1 |
```

This matrix is a useful starting point for creating other transformation matrices.

## Scaling

A scalar matrix is a matrix `S` that, when multiplied by a vector, results in a vector where each element is scaled by a single number.

Since we're currently just thinking about 3D space, we won't worry about the `w` component of the 4d vector.

```
| S1  0  0 0 |   | x |   | S1•x |
|  0 S2  0 0 | • | y | = | S2•y |
|  0  0 S3 0 |   | z |   | S3•z |
|  0  0  0 1 |   | 1 |   |   1  |
```

## Translation

Translation "moves" a vector by another vector by adding them together.

We could just add the two vectors together, but we can also use a transfomration matrix!

```
| 1 0 0 Tx |   | x |   | x+Tx |
| 0 1 0 Ty | • | y | = | y+Ty |
| 0 0 1 Tz |   | z |   | z+Tz |
| 0 0 0 1  |   | 1 |   |   1  |
```

## Homogeneous Coordinates

Okay this gets weird but bear with me.

A 4D vector has 4 components: `(x,y,z,w)`.

The `w` component is known as a *homogeneous coordinate*.

We can turn a 4D vector into a 3D vector by dividing the `xyz` coordinates by the `w` coordinate. So far, that hasn't had any actual impact, since `w` has been 1 in all of these examples.

However, its existence lets us use the 4x4 transformation matrix on 3D vectors.

In the future, we will use `w` as part of creating 3D visuals.

Also, when `w=0`. that vector is known as a *direction vector*. It specifies a direction in 3D space that *cannot be translated*. Any translation matrix applied to it will result in the same vector:

```
| 1 0 0 Tx |   | x |   | x+(0Tx) |   | x |
| 0 1 0 Ty | • | y | = | y+(0Ty) | = | y |
| 0 0 1 Tz |   | z |   | z+(0Tz) |   | z |
| 0 0 0 1  |   | 0 |   | 0+(0•1) |   | 0 |
```

## Rotation

Now its time to get CONFUSING BAYBEE

A rotation in 2D or 3D space is specified using an angle. Degrees, radians, etc.

```
Conversion reference:
radians = (pi•degrees)/180
degrees = radians*(180/pi)
```

(OpenTK includes a `MathHelper` namespace with a `DegreesToRadians` function.)

Rotations need an *angle* and an *axis*. A 2D rotation is simply a 3D rotation around the `z` axis.

Let's ignore the in-between math. You can define three separate transformation matrices to rotate a vector around the `xyz` axes by a specific amount.

Those matrices take forever to type out, and we won't be using them too much. You can apply them in sequence to represent a combined rotation in 3D space.

However, doing rotation like this can create a situation called *gimbal lock:* essentially it puts the system in an unstable state, where you can't rotate it properly anymore. That's bad!

*Quaternions* are the solution to the gimbal lock problem. We'll learn about them later. Probably.

## Combinining Transformations

By multiplying transformation matrices together, you can get a single matrix that represents both transformations.

The order matters, because matrix multiplication is NOT commutative! To make sure each transformation is preserved, combine transformations in the following order:

1. Scale
2. Rotation
3. Translation

Scale, rotate, translate. Scale, rotate, translate.

So a combined matrix to scale and translate:

```
| 1 0 0 1 |   | 2 0 0 0 |   | 2 0 0 1 |
| 0 1 0 2 | • | 0 2 0 0 | = | 0 2 0 2 |
| 0 0 1 3 |   | 0 0 2 0 |   | 0 0 2 3 |
| 0 0 0 1 |   | 0 0 0 2 |   | 0 0 0 1 |
```

## In OpenTK

OpenTK has a library for transformations. Hurrah!

We can use `Matrix4.CreateRotationY` and similar functions to create transformation matrices.

This is what I'll actually be doing in the code!
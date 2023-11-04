using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GameSpace {
  public class Texture {

    public readonly int Handle;
    public Texture(string texturePath) {
      Handle = GL.GenTexture();
      
      // Bind Texture2D to the handle
      Use();

      // StbImage loads from the top-left, but OpenGL loads from the bottom-left, so the texture will be inverted vertically.
      // So: flip it!
      StbImage.stbi_set_flip_vertically_on_load(1);

      // Load the image
      ImageResult image = ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);

      // Upload it!
      GL.TexImage2D(
        TextureTarget.Texture2D, // First, declare what kind of texture
        0, // Level of detail. You can use this to set a default mipmap lower than the max-scaled mipmap.
        PixelInternalFormat.Rgba, // Format for OpenGL to use when storing pixels on the GPU. (RGBA is usually what we want.)
        image.Width, // Height, pretty simple.
        image.Height, // Width. Again, pretty straightforward; just get the height from the loaded image.
        0, // Border of the image. This is always 0, apparently, because it's a legacy parameter. Fun.
        PixelFormat.Rgba, // Format of the pixels. This could be different depending on what you use to load the image. ImageSharp uses RGBA.
        PixelType.UnsignedByte, // Data type of the pixel. Not sure when other options would be appropriate.
        image.Data // And finally the input image data to be loaded into the texture!
      );

      // After calling TexImage2D, you can generate the mipmaps (if you need them; we don't yet).
      GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

      // EXCEPT MATBE WE DO. BECAUSE THE DEFAULTS MIN/MAG SETTINGS USE THEM.
      // YEAH I GOT A BLACK SQUARE WHEN I DIDN'T GENERATE THE MIPMAPS.
      // PRETTY SNEAKY, OPENTK
    }

    public void Use() {
      GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    private void SetTextureData() {
      // *** TEXTURE MAPPING ***
      // Texture mappings go from {0,0} to {1,1} (so only in quadrant 1).
      // This is telling each vertex where in the texture to sample from.
      // It's NOT related yet to the position of the vertices themselves. (i.e. the {-0.5, -0.5} vertex won't break things here)
      float[] triangleTextureCoords = {
        0.0f, 0.0f, // Bottom-left of the texture
        0.5f, 1.0f, // Top-middle of the texture
        1.0f, 0.0f, // Bottom-right of the texture
      };

      // *** TEXTURE WRAPPING ***
      // Wrapping tells OpenGL what to do if a texture is outside the bounds of the texture map.
      // Four options: 
      // Repeat: Just start the texture over.
      // MirroredRepeat: Start over, but flip it.
      // ClampToEdge: "Stretch" whatever's at the edge of the texture to fill the space.
      // ClampToBorder: Create a user-defined border color; anything outside now has that color.
      // Each option can be set separately for each axis. They have weird conventions:
      // s -> x axis
      // t -> y axis
      // p -> z axis
      // Use GL.TexParameter to configure them:
      // Note that we have to cast the final enum param to an int, for reasons.
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // For the s (or x) axis
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); // For the t (or y) axis

      // Or we can specify a border w/ a RGBA color...
      // Note/question: this seems like it applies to both axes?
      float[] borderColor = { 0.0f, 1.0f, 1.0f, 1.0f };
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

      // *** TEXTURE FILTERING *** 
      // Texture filtering tells OpenGL what to do if the correct pixel of the texture is in question.
      // i.e. if the texture has to be "scaled up" or "scaled down" to fit its target, how should it pick its pixels?
      // Two options for OpenGL: "Nearest" and "Linear"
      // - "Nearest" 
      // --- Picks an actual pixel directly from the texture
      // --- Results in a pixellated/blocky look
      // - "Linear"
      // --- Calculates pixel color based on surrounding pixels, basically "mixing" nearby pixel colors together
      // --- Results in a smoother/blurred look 
      // This is also set with GL.TexParameter! It can be set separately for "min" and "mag" operations:
      // - "min" is when the texture is scaled down
      // - "mag" is when the texture is scaled up
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

      // *** TEXTURE MIPMAPS ***
      // Mipmaps are "sets" of a single texture: a single texture image with multiple versions of the "base" texture.
      // Each version is a half-scaled version of the previous version. So it has the full texture, then a half-size version, then a quarter-size,
      // and so on until it gets to, like, a single pixel.
      // OpenGL loads a different section of the mipmapped texture depending on how small the area to be textured is.
      // This is how a far-away object can be rendered with a small version of the exact same texture as a close-up object.
      // Mipmaps improve both performance and appearance: you're not loading a tone of independent or high-res textures
      // for lots of far-away objects, plus the far-away objects have a texture appropriately sized for them.
      // OpenGL can generate mipmaps for us so we don't have to make the image manually (woo!)
      GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

      // You can filter between mipmaps just like filtering between pixels. Again, there's "linear" and "nearest".
      // BUT you don't need one for "mag", just "min". Mipmaps are for downscaling, not upscaling!
      // There's an extra layer, though: you start by choosing "nearest" of "linear" for the mipmap itself...
      // (i.e. either choose the closest existing mipmap or "create" one based on the surrounding ones)
      // and then "nearest" or "linear" for the pixels WITHIN the mipmap!
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
    }
  }
}
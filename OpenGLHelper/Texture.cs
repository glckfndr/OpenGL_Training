using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using StbImageSharp;
using GL = OpenTK.Graphics.OpenGL4.GL;
using InternalFormat = OpenTK.Graphics.OpenGL4.InternalFormat;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelType = OpenTK.Graphics.OpenGL4.PixelType;
using TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter;
using TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter;
using TextureParameterName = OpenTK.Graphics.OpenGL4.TextureParameterName;
using TextureTarget = OpenTK.Graphics.OpenGL4.TextureTarget;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;


namespace OpenGLHelper
{
    public class Texture
    {
        private readonly int _handle;
        private int _width;
        private int _height;
        private TextureUnit _unit;

        // Create texture from path.
        public Texture(string path, TextureWrapMode mode = TextureWrapMode.ClampToBorder,
                        TextureUnit unit = TextureUnit.Texture0,
                        TextureMinFilter minFilter = TextureMinFilter.Linear,
                        TextureMagFilter magFilter = TextureMagFilter.Linear
                        )
        {
            // Generate handle
            _unit = unit;
            _handle = GL.GenTexture();

            // SetAttribPointer the handle
            Use();

            // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

            // Load the image
            using (var image = new Bitmap(path))
            {
                // First, we get our pixels from the bitmap we loaded.
                // Arguments:
                //   The pixel area we want. Typically, you want to leave it as (0,0) to (width,height), but you can
                //   use other rectangles to get segments of textures, useful for things such as spritesheets.
                //   The locking mode. Basically, how you want to use the pixels. Since we're passing them to OpenGL,
                //   we only need ReadOnly.
                //   Next is the pixel format we want our pixels to be in. In this case, ARGB will suffice.
                //   We have to fully qualify the name because OpenTK also has an enum named PixelFormat.
                _width = image.Width;
                _height = image.Height;
                var data = image.LockBits(new Rectangle(0, 0, _width, _height),
                    ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D
                // Arguments:
                //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
                //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
                //   Target format of the pixels. This is the format OpenGL will store our image with.
                //   Width of the image
                //   Height of the image.
                //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
                //   The format of the pixels, explained above. Since we loaded the pixels as ARGB earlier, we need to use BGRA.
                //   Data type of the pixels.
                //   And finally, the actual pixels.
                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    _width,
                    _height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

            // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
            // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
            // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
            // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
            // your image will fail to Render at all (usually resulting in pure black instead).
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                (int) minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                                (int) magFilter);

            // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
            // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) mode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) mode);

            // Next, generate mipmaps.
            // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
            // Generated mipmaps go all the way down to just one pixel.
            // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
            // This prevents distant objects from having their colors become muddy, as well as saving on memory.
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public Texture(int width, int height, TextureWrapMode mode = TextureWrapMode.ClampToBorder, int unit = 0,
            TextureUnit textureUnit = TextureUnit.Texture0)
        {
            _width = width;
            _height = height;
            _unit = textureUnit;
            _handle = GL.GenTexture();
            Use();
            // GL.ActiveTexture(TextureUnit.Texture0);
            //  GL.BindTexture(TextureTarget.Texture2D, _handle);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, _width, _height);
            GL.BindImageTexture(unit, _handle, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba8);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) mode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) mode);
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use()
        {
            GL.ActiveTexture(_unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Copy()
        {
            Use();
            GL.CopyTexImage2D(TextureTarget.Texture2D,
                0,
                InternalFormat.Rgba,
                x: 0,
                y: 0,
                width: _width,
                height: _height,
                border: 0);
            //1465 * _width/1000
        }

        public void CopySubImage(int width, int height, int startX = 0, int startY = 0, int xOffset = 0,
            int yOffset = 0)
        {
            Use();
            GL.CopyTexSubImage2D(TextureTarget.Texture2D,
                0,
                xOffset,
                yOffset,
                x: startX,
                y: startY,
                width: width,
                height: height
            );

        }

        public Size GetSize()
        {
            return new Size(_width, _height);
        }

        public int GetIndex()
        {
            return _handle;
        }

        public void BindToFrameBuffer()
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _handle, 0);
        }


        public static int LoadCubeMap(string baseName)
        {
            int texID;
            texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, texID);

            string[] suffixes = new string[] {"posx", "negx", "posy", "negy", "posz", "negz"};
            int w = 0, h = 0;
            TextureTarget[] targets = new TextureTarget[]{
                TextureTarget.TextureCubeMapPositiveX,
                TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapPositiveY,
                TextureTarget.TextureCubeMapNegativeY,
                TextureTarget.TextureCubeMapPositiveZ,
                TextureTarget.TextureCubeMapNegativeZ
            };


            // Load the first one to get width/height
            string texName = baseName + "_" + suffixes[0] + ".png";
            //string texName = baseName + "_" + suffixes[0] + ".hdr";
            //var reader = new StbSharp.ImageReader();
            //reader.Read()
            //var bytes = File.ReadAllBytes(texName);
            //var data = StbImage.LoadFromMemory(bytes, StbImage.STBI_rgb);
            int nch = 0;
            var data = stbi_loadB(texName, ref w, ref h).Scan0;
            //var data = LoadTextureAsBytes(texName, ref w, ref h);
            //var data2 = stbi_loadB(texName, ref w, ref h);


            // Allocate immutable storage for the whole cube map texture
            //GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 1,PixelInternalFormat.Rgb , w, h, 0, 
            //    PixelFormat.Rgb, PixelType.Byte, data);



            GL.TexStorage2D(TextureTarget2d.TextureCubeMap, 1, 
                SizedInternalFormat.Rgba8, w, h);
            GL.TexSubImage2D(targets[0],
                0, 0, 0, w, h, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            //GL.TexSubImage2D(TextureTarget.TextureCubeMapPositiveX,
            //    0, 0, 0, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data);



            // Load the other 5 cube-map faces
            for (int i = 1; i < 6; i++)
            {
                texName = baseName + "_" + suffixes[i] + ".png";
                data = stbi_loadB(texName, ref w, ref h).Scan0;
                //data = LoadTextureAsBytes(texName, ref w, ref h);
                //data2 = stbi_loadB(texName, ref w, ref h);

                //data = stbi_loadf(texName.c_str(), &w, &h, NULL, 3);
                GL.TexSubImage2D(targets[i], 0,
                    0, 0, w, h, PixelFormat.Rgba , 
                    PixelType.UnsignedByte, data);

            }

            GL.TexParameter(TextureTarget.TextureCubeMap, 
                TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, 
                        TextureParameterName.TextureMinFilter, 
                        (int)TextureMinFilter.Nearest); 
            GL.TexParameter(TextureTarget.TextureCubeMap, 
                TextureParameterName.TextureWrapS, 
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, 
                TextureParameterName.TextureWrapT, 
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, 
                TextureParameterName.TextureWrapR, 
                (int)TextureWrapMode.ClampToEdge);

            return texID;
        }

        //static private byte[] stbi_loadf(string path, ref int w, ref int h, ref int numChannels)
        //{
        //    StbiImage image;
        //    byte[] data;
        //    using (var stream = File.OpenRead(path))
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        stream.CopyTo(memoryStream);
        //        image = Stbi.LoadFromMemory(memoryStream, StbImage.STBI_rgb);
                
        //        w = image.Width;
        //        h = image.Height;
        //        numChannels = image.NumChannels;
        //        data = image.Data.ToArray();
        //        return data;
        //    }

        //    //var res = new float[data.Length/4];
        //    //var k = 0;
        //    //for (int i = 0; i < data.Length; i+=4)
        //    //    res[k++] = System.BitConverter.ToSingle(data, i);

        //}
        public static int LoadTexture(string path, TextureUnit unit = TextureUnit.Texture0)
        {
            int w = 0;
            int h = 0;

            //var data = stbi_loadB(path, ref  w, ref  h);
            var data = LoadTextureAsBytes(path, ref w, ref h);
            //GL.PixelStore(PixelStoreParameter.UnpackRowLength, data.Width * 4); // 4x for 32bpp
            int tex = GL.GenTexture();
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1,
                        SizedInternalFormat.Rgba8, w, h);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, w, h,
                                PixelFormat.Rgb, PixelType.UnsignedByte, data);
            //GL.TexImage2D(TextureTarget.Texture2D,
            //    0,
            //    PixelInternalFormat.Rgba,
            //    w,
            //    h,
            //    0,
            //    PixelFormat.Bgra,
            //    PixelType.UnsignedByte,
            //    data.Scan0);


            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            return tex;

        }

        static private BitmapData stbi_loadB(string path, ref int w, ref int h)
        {
            using (var image = new Bitmap(path))
            {
                w = image.Width;
                h = image.Height;
                var data = image.LockBits(new Rectangle(0, 0, w, h),
                    ImageLockMode.ReadOnly, //image.PixelFormat
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb
                   // System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    );

                return data;

            }


        }

        public static byte[] LoadTextureAsBytes(string path, ref int w, ref int h)
        {
            
            byte[] buffer = File.ReadAllBytes(path);

            ImageResult image = ImageResult.FromMemory(buffer, ColorComponents.RedGreenBlueAlpha);
            w = image.Width;
            h = image.Height;
            //return image.Data;
            byte[] data = image.Data;
            
            //ConvertRgbaToBgra(image, data);

            return data;
        }

        private static void ConvertRgbaToBgra(ImageResult image, byte[] data)
        {
            for (int i = 0; i < image.Height * image.Width; ++i)
            {
                byte r = data[i * 4];
                byte g = data[i * 4 + 1];
                byte b = data[i * 4 + 2];
                byte a = data[i * 4 + 3];


                data[i * 4] = b;
                data[i * 4 + 1] = g;
                data[i * 4 + 2] = r;
                data[i * 4 + 3] = a;
            }
        }
    }
}

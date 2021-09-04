using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class Texture2D32
    {
        private readonly int _handle;
        private int _width;
        private int _height;
        private int _size;
        private TextureUnit _unit;
        private int _binding_unit;

        public void Use()
        {
            GL.ActiveTexture(_unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public Texture2D32(int width, int height, int unit, TextureUnit textureUnit = TextureUnit.Texture0,
            TextureWrapMode mode = TextureWrapMode.ClampToBorder)
        {
            _width = width;
            _height = height;
            _unit = textureUnit;
            _handle = GL.GenTexture();
            Use();
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba32f, _width, _height);
            GL.BindImageTexture(unit, _handle, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba32f);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)mode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)mode);
        }

        public Texture2D32(int width, int height, float[] data, int unit, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            _width = width;
            _height = height;
            _unit = textureUnit;
            _binding_unit = unit;
            _handle = GL.GenTexture();
            _size = data.Length;
            Use();

            //GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba32f, _width, _height);
            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _width, _height, PixelFormat.Rgba, PixelType.Float, data);

            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.R32f, _width, _height);

            GL.BindImageTexture(unit, _handle, 0, false, 0, 
                                    TextureAccess.ReadWrite, SizedInternalFormat.R32f);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _width, _height, 
                                                PixelFormat.Red, PixelType.Float, data);

           // GL.GetTextureSubImage();
        }


        public float[] GetData()
        {
            Use();
            float[] data = new float[_size];
            GL.GetTexImage<float>(TextureTarget.Texture2D, 0, PixelFormat.Red, PixelType.Float, data);
            return data;
        }

    }
}

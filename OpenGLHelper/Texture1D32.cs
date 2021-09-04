using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{

    public class Texture1D32
    {
        private readonly int _handle;
        private int _size;
        private int _height;
        private TextureUnit _unit;

        public void Use()
        {
            GL.ActiveTexture(_unit);
            GL.BindTexture(TextureTarget.Texture1D, _handle); ;
        }

        public Texture1D32(int size, float[] data, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            _size = size;
            _unit = textureUnit;
            _handle = GL.GenTexture();
            Use();
            GL.TexStorage1D(TextureTarget1d.Texture1D, 1, SizedInternalFormat.R32f, _size);
            GL.TexSubImage1D(TextureTarget.Texture1D, 0, 0, _size, PixelFormat.Red, PixelType.Float, data);

            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        }


    }
}

using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class Texture3D32
    {
        private readonly int _handle;
        private int _width;
        private int _height;
        private int _depth;
        private int _size;
        private TextureUnit _unit;
        private int _binding_unit;
        public void Use()
        {
            GL.ActiveTexture(_unit);
            GL.BindTexture(TextureTarget.Texture3D, _handle);
        }
        public Texture3D32(int width, int height, int depth, float[] data, int unit, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            _width = width;
            _height = height;
            _depth = depth;
            _unit = textureUnit;
            _binding_unit = unit;
            _handle = GL.GenTexture();
            _size = data.Length;
            Use();

            GL.TexStorage3D(TextureTarget3d.Texture3D, 1, 
                                SizedInternalFormat.R32f, _width, _height, _depth);

            GL.BindImageTexture(_binding_unit, _handle, 0, true, 0,
                                    TextureAccess.ReadWrite, SizedInternalFormat.R32f);

            GL.TexSubImage3D(TextureTarget.Texture3D, 0, 0, 0, 0, 
                                _width, _height, _depth,
                                PixelFormat.Red, PixelType.Float, data);


        }
    }
}

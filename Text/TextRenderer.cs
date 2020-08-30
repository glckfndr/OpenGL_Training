using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Text
{
    public class TextRenderer : IDisposable
    {
        private Bitmap _bitmap;
        private Graphics _graphics;
        private int _textureIndex;
        private Rectangle rectGFX;
        private bool disposed;


        // Конструктор нового экземпляра класса
        // width, height - ширина и высота растрового образа
        public TextRenderer(int width, int height)
        {
            if (GraphicsContext.CurrentContext == null) throw new InvalidOperationException("GraphicsContext не обнаружен");
            _bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            _graphics = Graphics.FromImage(_bitmap);
            // Используем сглаживание
            _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            _textureIndex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textureIndex);
            // Свойства текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            // Создаем пустую тектсуру, которую потом пополним растровыми данымми с текстом (см.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }

        // Заливка образа цветом color
        public void Clear(Color color)
        {
            _graphics.Clear(color);
            rectGFX = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
        }
        // Выводит строку текта text в точке point растрового образе, используя фонт font и цвета brush
        // Начало координат растрового образа находится в его левом верхнем углу
        public void DrawString(string text, Font font, Brush brush, PointF point)
        {
            _graphics.DrawString(text, font, brush, point);
        }
        // Получает обработчик _textureIndex (System.Int32) текструры, который связывается с TextureTarget.Texture2d
        // см.в OnRenderFrame: GL.BindTexture(TextureTarget.Texture2D, renderer.TextureIndex)
        public int TextureIndex
        {
            get
            {
                UploadBitmap();
                return _textureIndex;
            }
        }

        // Выгружеат растровые данные в текстуру OpenGL
        private void UploadBitmap()
        {
            if (rectGFX != RectangleF.Empty)
            {
                System.Drawing.Imaging.BitmapData data = _bitmap.LockBits(rectGFX,
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.BindTexture(TextureTarget.Texture2D, _textureIndex);
                // Текстура формируется на основе растровых данных, содержащихся в data
                GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                    rectGFX.X, rectGFX.Y, rectGFX.Width, rectGFX.Height,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                // Освобождаем память, занимаемую data
                _bitmap.UnlockBits(data);
                rectGFX = Rectangle.Empty;
            }
        }

        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    _bitmap.Dispose();
                    _graphics.Dispose();
                    if (GraphicsContext.CurrentContext != null) GL.DeleteTexture(_textureIndex);
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TextRenderer()
        {
            Console.WriteLine("[Предупреждение] Есть проблеммы: {0}.", typeof(TextRenderer));
        }
    }
}

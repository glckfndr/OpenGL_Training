using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Text
{

    // Выводит текст в растровый образ, на основе которого формируется текстура
    // Текстура используется при выводе полигона (квадрата)
    internal class TextRendering : GameWindow
    {
        private TextRenderer renderer;

        // Шрифты для вывода текста
        private Font serif = new Font(FontFamily.GenericSerif, 24);
        private Font sans = new Font(FontFamily.GenericSansSerif, 24);
        private Font mono = new Font(FontFamily.GenericMonospace, 24);
        //
        // Окно OpenGL размером 500 * 150
        public TextRendering() : base(500, 150) { }
        //
        // Использует System.Drawing для вывода 2d-текста

        protected override void OnLoad(EventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Enable(EnableCap.Texture2D);
            renderer = new TextRenderer(Width, Height);
            PointF position = PointF.Empty;
            renderer.Clear(Color.SaddleBrown);
            // Текст белым цветом и разными шрифтами
            // GenericSerif
            renderer.DrawString("За рекой гремит гроза", serif, Brushes.White, position);
            position.Y += serif.Height;
            // GenericSansSerif
            renderer.DrawString("За рекой гремит гроза", sans, Brushes.White, position);
            position.Y += sans.Height;
            // GenericMonospace
            renderer.DrawString("За рекой гремит гроза", mono, Brushes.White, position);
            // Позиция для следующей строки текста, если такая появится
            position.Y += mono.Height;
        }
        protected override void OnUnload(EventArgs e)
        {
            renderer.Dispose();
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Выход по Escape
            var keyboard = OpenTK.Input.Keyboard.GetState();
            if (keyboard[OpenTK.Input.Key.Escape]) this.Exit();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindTexture(TextureTarget.Texture2D, renderer.TextureIndex);
            // Вывод квадрата с текстурой, содержащей текст (три строки)
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1f, -1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, 1f);
            GL.End();
            SwapBuffers();
        }
        public static void Main()
        {
            using (TextRendering example = new TextRendering())
            {
                example.Title = "Проба текста";
                example.Run(30.0);
            }
        }
    }
}

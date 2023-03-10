using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;


namespace DemoFlowVisualization
{


    public class Game : GameWindow
    {
        private bool _is3D = true;
        private bool _isPause = false;

        private float _eyePos = 7.0f;
        private float _xPosition = 1.0f;
        
       // private VortexDynamic2D _vortexDynamic2D;
        private RectangleFlow _rectangleFlow;
        private bool _isDrawVortex = true;

        public Game(int width, int height, string title) : 
            base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
         //   _vortexDynamic2D = new VortexDynamic2D(800,800);
            _rectangleFlow = new RectangleFlow(1024, 768);
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
          //  _vortexDynamic2D.ComputeAndDraw(_isPause, _is3D, _isDrawVortex);
          //  _vortexDynamic2D.SetEye(_eyePos);
          //  _vortexDynamic2D.SetHorizontal(_xPosition);

            _rectangleFlow.ComputeAndDraw(_isPause, _is3D, _isDrawVortex);
            _rectangleFlow.SetViewPoint(_xPosition, _eyePos);
            Context.SwapBuffers();
            //Thread.Sleep(500);
            base.OnRenderFrame(e);
            Thread.Sleep(10);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GetInput();
            base.OnUpdateFrame(e);
        }

        private void GetInput()
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.Number3))
            {
                _is3D = true;
            }

            if (input.IsKeyDown(Key.Space))
            {
                _isPause = true;
            }

            if (input.IsKeyDown(Key.C))
            {
                _isPause = false;
            }

            if (input.IsKeyDown(Key.Number2))
            {
                _is3D = false;
            }

            if (input.IsKeyDown(Key.Down))
            {
                _eyePos -= 0.1f;
            }

            if (input.IsKeyDown(Key.Up))
            {
                _eyePos += 0.1f;
            }

            if (input.IsKeyDown(Key.Left))
            {
                _xPosition += 0.1f;
            }

            if (input.IsKeyDown(Key.Right))
            {
                _xPosition -= 0.1f;
            }

            if (input.IsKeyDown(Key.V))
            {
                _isDrawVortex = true;
            }

            if (input.IsKeyDown(Key.N))
            {
                _isDrawVortex = false;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            //_shape.DeleteBuffers();
            //_particleShader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}

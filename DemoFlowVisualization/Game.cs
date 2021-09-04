﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;


namespace DemoFlowVisualization
{


    public class Game : GameWindow
    {
        private bool _is3D = false;
        private bool _isPause = false;

        private float _eyePos = 7.0f;
        private float _xPosition = 1.0f;
        
        private VortexDynamic2D vd;
        private RectangleFlow rf;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            // vd = new VortexDynamic2D(800,800);
            rf = new RectangleFlow(1024, 768);

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

           //vd.ComputeAndDraw(_isPause, _is3D);
            rf.ComputeAndDraw(_isPause, _is3D);
            rf.SetEye(_eyePos);
            rf.SetHorizontal(_xPosition);
            Context.SwapBuffers();
            //Thread.Sleep(500);
            base.OnRenderFrame(e);
            Thread.Sleep(20);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
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

            base.OnUpdateFrame(e);
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

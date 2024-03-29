﻿using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace ComputeShaderVortex
{
    public class Game : GameWindow
    {
        private Shader _particleShader;
        private Shader _vortexShader;
        private Shader _adsShader;
        private Shader _computeShader;
        private vec3 nParticles = new vec3(64, 64, 32);
        private int totalParticles;
        private int vortexNumber = 72;
        private float eyePos = 3.0f;

        private StorageBuffer posBuf;
        private StorageBuffer startPosBuf;
        private StorageBuffer velBuf;
        private StorageBuffer vortexBuf;
        private float _gamma = 0.1f;

        private float _time;
        private float _deltaT;
        private float _speed = 35.0f;
        private float _angle;

        private VertexArray _particleVAO;
        private VertexArray _vortexVAO;

        private float t;
        private float dt;
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Sphere _sphere1;
        private Sphere _sphere2;
        private double outDeltaR = 0.15f;
        private double innerDeltaR = 0.15f;

        private double _ringRadius = 1.0;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _model = new mat4(1.0f);
            GL.Enable(EnableCap.DepthTest);
            totalParticles = (int)(nParticles.x * nParticles.y * nParticles.z);
            _particleShader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _vortexShader = new Shader("../../Shaders/vortex.vert", "../../Shaders/vortex.frag");

            _computeShader = new Shader("../../Shaders/vortex.comp");
            _computeShader.SetInt("vortexNumber", vortexNumber + 1);
            _computeShader.SetFloat("ringRadius",(float) _ringRadius);
            _computeShader.SetFloat("gamma", _gamma);
            InitBuffers();

            GL.ClearColor(0, 0.0f, 0.0f, 1);

            _projection = glm.perspective(glm.radians(50.0f),
                                        (float)Width / Height, 0.1f, 100.0f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            if (_time == 0.0f)
            {
                _deltaT = 0.0f;
            }
            else
            {
                _deltaT = t - _time;
            }
            _time = t;
            if (true)
            {
                _angle += _speed * _deltaT;
                if (_angle > 360.0f) _angle -= 360.0f;
            }
            t += 0.005f;

            // Execute the compute shader
            _computeShader.Use();
            posBuf.BindLayout(0);
            velBuf.BindLayout(1);
            vortexBuf.BindLayout(2);
            startPosBuf.BindLayout(3);

            _computeShader.SetInt("vortexNumber", vortexNumber);
            _computeShader.SetFloat("gamma", _gamma);

           for (int k = 0; k < 3; k++)
                _computeShader.Compute(totalParticles / 64, 1, 1, 
                    MemoryBarrierFlags.ShaderStorageBarrierBit);

            // Draw the scene
            SetMatrices();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _particleShader.Use();
            // Draw the particles
            _particleShader.SetVector4("Color", new Vector4(0.9f, 0.7f, 0, 0.8f));
            GL.PointSize(1.0f);
            _particleVAO.Draw(PrimitiveType.Points, 0, totalParticles);
            // Draw the vortexes
            _vortexShader.Use();
            _vortexShader.SetVector4("Color", new Vector4(0.7f, 0.9f, 0.3f, 0.8f));
            //velBuf.SetAttribPointer(2, totalParticles);
           // _vortexVAO.Draw(PrimitiveType.LineLoop, 0, vortexNumber);


            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void InitBuffers()
        {
            // Initial positions of the particles
            
            // We need _buffers for position , and velocity.
            //int bufSize = totalParticles * 4 * sizeof(float);

            // The _buffers for positions
            List<float> initialPosition = GetInitialPosition2(_ringRadius);

            posBuf = new StorageBuffer(BufferUsageHint.DynamicDraw);
            posBuf.SetData(initialPosition.ToArray(), 0);

            var vel = GetInitialVelocity();
            velBuf = new StorageBuffer(BufferUsageHint.DynamicDraw);
            velBuf.SetData(vel.ToArray(), 1);

            List<float> vortexPosition = GetVortexPosition(vortexNumber);
            vortexBuf = new StorageBuffer(BufferUsageHint.DynamicDraw);
            vortexBuf.SetData(vortexPosition.ToArray(), 2);

            startPosBuf = new StorageBuffer(BufferUsageHint.StaticDraw);
            startPosBuf.SetData(initialPosition.ToArray(), 3);


            // Set up the VAO
            _particleVAO = new VertexArray();
            posBuf.SetAttribPointer(0, 4);
            velBuf.SetAttribPointer(1, 4);
            _particleVAO.Unbind();


            // Set up a buffer and a VAO for drawing the vortexes
            _vortexVAO = new VertexArray(); ;
            _vortexVAO.Bind();

            vortexBuf.SetAttribPointer(0, 4);
            _vortexVAO.Unbind();
        }

        private List<float> GetVortexPosition(int n)
        {
            List<float> lst = new List<float>(n);
            
            var dfi = 2 * Math.PI / n;
            for (int i = 0; i < n; i++)
            {


                var x = (float)(_ringRadius * Math.Cos(i * dfi));
                var y = 0;
                var z = (float)(_ringRadius * Math.Sin(i * dfi));
                var w = 1.0f;

                lst.Add(x);
                lst.Add(y);
                lst.Add(z);
                lst.Add(w);

            }

            lst.Add(lst[0]);
            lst.Add(lst[1]);
            lst.Add(lst[2]);
            lst.Add(lst[3]);

            return lst;
        }

        private List<float> GetInitialPosition2(double ringRadius)
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;

                        
                        var r = ringRadius - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = 0.0f;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }

            return initialPosition;
        }

        private List<float> GetInitialVelocity()
        {
            List<float> lst = new List<float>();

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {

                        var x = 0.0f;
                        var y = 0.0f;
                        var z = 0.0f;
                        var w = 1.0f;

                        lst.Add(x);
                        lst.Add(y);
                        lst.Add(z);
                        lst.Add(w);
                    }
                }
            }

            return lst;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.Up))
            {
                eyePos += 0.1f;
            }

            if (input.IsKeyDown(Key.Down))
            {
                eyePos -= 0.1f;
            }

            base.OnUpdateFrame(e);
        }



        private void SetMatrices()
        {
            _view = glm.lookAt(new vec3(3, eyePos, 0), new vec3(0, 0, 0), new vec3(0, 1, 0));
            //_model = new mat4(1.0f);
            _particleShader.Use();
            _particleShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _particleShader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _particleShader.SetMatrix4("view", _view.ConvertToMatrix4());

            _vortexShader.Use();
            _vortexShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _vortexShader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _vortexShader.SetMatrix4("view", _view.ConvertToMatrix4());
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
            _particleShader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}

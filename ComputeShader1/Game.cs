﻿using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputeShader1
{
    public class Game : GameWindow
    {

        private Shader _shader;
        private Shader _computeShader;
        private vec3 nParticles = new vec3(100, 100, 100);
        private int totalParticles;

        private float time = 0.0f;
        private float deltaT = 0.0f;
        private float speed = 35.0f;
        private float angle = 0.0f;

        //private int _particleVAO;
        private VertexArray _particleVAO;
        private VertexArray _blackHoleVAO;
        private ArrayBuffer _blackHoleBuffer;  // black hole VAO and buffer
        private vec4 _blackHole1 = new vec4(3.9f, 1, 0, 0);
        private vec4 _blackHole2 = new vec4(-3.9f, -1, 0, 0.8f);



        //private int location;
        //private uint nParticles = 8000;
        //private VertexObject _shape;
        private float t = 0;
        private float dt;
        //private mat4 MVP;
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            totalParticles = (int)(nParticles.x * nParticles.y * nParticles.z);
            _shader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _computeShader = new Shader("../../Shaders/particles.comp");

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

            if (time == 0.0f)
            {
                deltaT = 0.0f;
            }
            else
            {
                deltaT = t - time;
            }
            time = t;
            if (true)
            {
                angle += speed * deltaT;
                if (angle > 360.0f) angle -= 360.0f;
            }

            // Rotate the attractors ("black holes")
            mat4 rot = glm.rotate(new mat4(1.0f), glm.radians(angle), new vec3(0, 0, 1));
            vec3 att1 = new vec3(rot * _blackHole1);
            vec3 att2 = new vec3(rot * _blackHole2);

            // Execute the compute shader
            _computeShader.Use();
            t += 0.005f;
            var x = (float)(0.1 * Math.Sin(t));
            _computeShader.SetVector3("BlackHolePos1", new Vector3(att1.x, att1.y, att1.z));
            _computeShader.SetVector3("BlackHolePos2", new Vector3(att2.x, att2.y, att2.z));
            //GL.DispatchCompute(totalParticles / 1000, 1, 1);
            //GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
            _computeShader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit, totalParticles / 1000, 1, 1);

            // Draw the scene
            _shader.Use();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SetMatrices();

            // Draw the particles
            _shader.SetVector4("Color", new Vector4(1, 0.5f, 0, 0.2f));
            GL.PointSize(1.0f);
            _particleVAO.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, totalParticles);
            _particleVAO.Unbind();

            float[] data = new float[] { att1.x, att1.y, att1.z, 1.0f, att2.x, att2.y, att2.z, 1.0f };

            _blackHoleBuffer.SetSubData(IntPtr.Zero, data.Length * sizeof(float), data);
            _shader.SetVector4("Color", new Vector4(0, 1, 0, 1.0f));
            GL.PointSize(10.0f);
            // Draw the attractors
            _blackHoleVAO.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, 2);
            _blackHoleVAO.Unbind();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void InitBuffers()
        {
            // Initial positions of the particles

            List<float> initVel = Enumerable.Repeat(0.0f, (int)totalParticles * 4).ToList();
            List<float> initialPosition = GetInitialPosition2();


            // We need _buffers for position , and velocity.
            int bufSize = totalParticles * 4 * sizeof(float);

            // The _buffers for positions
            var velBuf = new StorageBuffer(BufferUsageHint.DynamicDraw);
            velBuf.SetData(initVel.ToArray(), 1);

            var posBuf = new StorageBuffer(BufferUsageHint.DynamicDraw);
            posBuf.SetData(initialPosition.ToArray(), 0);

            // Set up the VAO
            _particleVAO = new VertexArray();
            _particleVAO.Bind();

            posBuf.SetAttribPointer(0, 4);

            _particleVAO.Unbind();


            // Set up a buffer and a VAO for drawing the attractors (the "black holes")
            _blackHoleBuffer = new ArrayBuffer(BufferUsageHint.DynamicDraw);
            _blackHoleBuffer.Allocate(8 * sizeof(float));

            _blackHoleVAO = new VertexArray();
            _blackHoleVAO.Bind();

            _blackHoleBuffer.SetAttribPointer(0, 4);

            _blackHoleVAO.Unbind();
        }

        private List<float> GetInitialPosition()
        {
            List<float> initialPosition = new List<float>();
            vec4 p = new vec4(0.0f, 0.0f, 0.0f, 1.0f);
            float dx = 2.0f / (nParticles.x - 1);
            float dy = 2.0f / (nParticles.y - 1);
            float dz = 2.0f / (nParticles.z - 1);
            // We want to center the particles at (0,0,0)
            mat4 transf = glm.translate(new mat4(1.0f), new vec3(-1, -1, -1));
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        p.x = dx * i;
                        p.y = dy * j;
                        p.z = dz * k;
                        p.w = 1.0f;
                        p = transf * p;
                        initialPosition.Add(p.x);
                        initialPosition.Add(p.y);
                        initialPosition.Add(p.z);
                        initialPosition.Add(p.w);
                    }
                }
            }

            return initialPosition;
        }


        private List<float> GetInitialPosition2()
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;
                        var theta = rnd.NextDouble() * Math.PI;
                        var r = rnd.NextDouble();
                        var x = (float)(r * Math.Sin(theta) * Math.Cos(phi));
                        var y = (float)(r * Math.Sin(theta) * Math.Sin(phi));
                        var z =(float) (r * Math.Cos(theta));
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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        

        private void SetMatrices()
        {
            _view = glm.lookAt(new vec3(2, 0, 20), new vec3(0, 0, 0), new vec3(0, 1, 0));
            _model = new mat4(1.0f);
            _shader.Use();
            _shader.SetMatrix4("model", _model.ConvertToMatrix4());
            _shader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _shader.SetMatrix4("view", _view.ConvertToMatrix4());
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
            _shader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}

using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ComputeShaderGravityGalaxies
{
    public class RectangleFlow : ISimulator
    {
        //  shaders for computing;
        private Shader _particleComputeShader;
        private Shader _particleShader;
        private Shader _vortexShader;
        private Shader _bodyShader;

        private int _vortexNumber;
        private VortexSystem _vortexSystem;
        private VortexesFromMemory _vortexesFromMemory;
        private StorageBuffer _particleBuf;

        private StorageBuffer _particleVelBuf;
        private StorageBuffer _lifeTimeBuf;

        private StorageBuffer _vortexBuf;
        //private vec3 _nParticles = new vec3(128, 128, 1);
        private vec3 _nParticles = new vec3(32, 32, 16);
        //private vec3 _nParticlesCircle = new vec3(128, 128, 1);
        private int _particleNumber;
        private Vector _flowVelovity = new Vector(1.0, 0.0);
        private StorageBuffer _startPos;
        private VertexArray _particleVAO;
        private mat4 _view;
        private mat4 _model;
        private float _eyePos = 10.0f;
        private float _angle = 0;
        private int Height;
        private int Width;
        private int _counter = 0;
        private VortexStruct[] _vortexStructArray = null;
        private float _xCenter = 1;
        private VertexArray _vortexVAO;
        private Texture2D _texture;
        private List<Vector2> _bogyTriangles;
        private StorageBuffer _bodyBufferObject;
        private VertexArray _bodyVAO;
        private StorageBuffer _velocityBuffer;
        private Shader _velocityComputeShader;


        public RectangleFlow(int width, int height)
        {
            Height = height;
            Width = width;

            GeometryShape polygon = new GeometryShape(new List<Vector>
            {
                new Vector(-0.7,-0.5),
                new Vector(-0.7,0.4),
                new Vector(0.2,0.6),
                new Vector(0.5,0.0),
                new Vector(0.5,-0.6)
                
                
                //new Vector(0.3,0.0),
                //new Vector(0.25,-0.1)

            }, true);
            _bogyTriangles = GerVertex(polygon.Triangulation);

            GeometryShape polygon1 = new GeometryShape(new List<Vector>
            {

                new Vector(1.2,-0.6),
                new Vector(1.9,0.5),
                new Vector(2.1,-0.4),
                new Vector(1.9,-1.6),
                //new Vector(0.25,-0.1)

            }, true);

            _bogyTriangles.AddRange(GerVertex(polygon1.Triangulation));


            GeometryShapeCollection sp = new GeometryShapeCollection();
            sp.AddShape(polygon);
            sp.AddShape(polygon1);
            var ds = new Discretizator(0.1, 0.1);
            _vortexSystem = new VortexSystem(sp, ds, new RankinVortex());
            _vortexSystem.FlowVelocity = _flowVelovity;
            _vortexSystem.FirstStep();



            _particleNumber = (int)(_nParticles.x * _nParticles.y * _nParticles.z);
            SetOpenGlParameters();
            CreateShaders();
            CreateBuffers();

            _vortexesFromMemory = new VortexesFromMemory(_vortexSystem);
            _vortexStructArray = _vortexesFromMemory.GetArrayOfVortexStruct();
            _vortexBuf.SubData(_vortexStructArray);
        }

        private List<Vector2> GerVertex(List<Triangle> polygonTriangulation)
        {
            var list = new List<Vector2>(3);
            foreach (var triangle in polygonTriangulation)
            {
                var p0 = new Vector2((float)triangle.Edge[0].X, (float)triangle.Edge[0].Y);
                var p1 = new Vector2((float)triangle.Edge[1].X, (float)triangle.Edge[1].Y);
                var p2 = new Vector2((float)triangle.Edge[2].X, (float)triangle.Edge[2].Y);
                list.Add(p0);
                list.Add(p1);
                list.Add(p2);
            }
            return list;
        }

        private void CreateBuffers()
        {
            const string texName = "../../Textures/bubble.png";
            _texture = new Texture2D(texName, TextureWrapMode.Repeat);
            _texture.Use();
            // var particles = GetParticles(_nParticles).ToArray();
            var particles = GetParticlesInCircles(_nParticles).ToArray();
            _particleBuf = SetBufferData(particles, 0);
            //_particleBuf = SetBufferData(GetV(_nParticles).ToArray(), 0);

            // до 8 * 1024 вихорів
            var nv = 8 * 1024;
            var vrt = new VortexStruct[nv];
            _vortexBuf = AllocateBufferData(vrt, 1);
            var vel = new Vector2[nv];
            _velocityBuffer = AllocateBufferData(vel, 11);

            _particleVelBuf = SetBufferData(GetInitialVelocity(_nParticles).ToArray(), 2);
            _startPos = SetBufferData(particles, 3);
            _lifeTimeBuf = SetBufferData(GetLifeTime(_nParticles).ToArray(), 4);
            _bodyBufferObject = SetBufferData(_bogyTriangles.ToArray(), 6);

            // Set up the VAO
            _particleVAO = SetVAO(new[] { _particleBuf, _particleVelBuf }, new[] { 0, 1 }, new[] { 4, 4 });
            _vortexVAO = SetVAO(new[] { _vortexBuf }, new[] { 1 }, new[] { 4 });
            //_vortexVAO = new VertexArray();
            //  _vortexBuf.SetAttribPointer(1, 4);
            //_vortexVAO.Unbind();

            //_bodyBufferObject = new StorageBuffer(BufferUsageHint.DynamicDraw); ;
            //_bodyBufferObject.SetData(_bogyTriangles.ToArray(), 6);

            _bodyVAO = SetVAO(new[] { _bodyBufferObject }, new[] { 6 }, new[] { 2 });
            //_bodyVAO = new VertexArray();
            //  _bodyBufferObject.SetAttribPointer(6, 2);
            //_bodyVAO.Unbind();
        }

        private VertexArray SetVAO(StorageBuffer[] buffers, int[] ind, int[] num)
        {
            var vertexArray = new VertexArray();
            //_particleBuf.SetAttribPointer(0, 4);
            //_vortexBuf.SetAttribPointer(0,4);
            //_particleVelBuf.SetAttribPointer(1, 4);
            for (int i = 0; i < buffers.Length; i++)
            {
                buffers[i].SetAttribPointer(ind[i], num[i]);
            }


            vertexArray.Unbind();
            return vertexArray;
        }

        private StorageBuffer SetBufferData<T>(T[] data, int layoutShaderIndex) where T : struct
        {

            var buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            buffer.SetData(data, layoutShaderIndex); // copy data on GPU
            return buffer;
        }

        private StorageBuffer AllocateBufferData<T>(T[] data, int layoutShaderIndex) where T : struct
        {
            var buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            buffer.Allocate(data, layoutShaderIndex); // allocate  data  on GPU
            return buffer;
        }

        private void CreateShaders()
        {
            _particleComputeShader = new Shader("../../Shaders/particle.comp");
            _velocityComputeShader = new Shader("../../Shaders/Velocity2D.comp");

            _particleShader = new Shader("../../Shaders/particles.vert",
                "../../Shaders/particles.frag");


            _vortexShader = new Shader("../../Shaders/vortexTex.vert",
                "../../Shaders/vortexTex.frag");
            _bodyShader = new Shader("../../Shaders/body.vert",
                "../../Shaders/body.frag");
        }

        public void Draw(bool isPause, bool is3D)
        {

            if (_counter % 1 == 0)
            {
                Console.WriteLine(_vortexSystem.Time);
                _counter = 0;

                //_vortexesFromMemory = new VortexesFromMemory(_vortexSystem);
                //_vortexes = _vortexesFromMemory.GetArrayOfVortexStruct();
                Console.WriteLine("Len : " + _vortexStructArray.Length);

                // _vortexBuf.SubData(_vortexes, 1);
                //_vortexBuf.SubData(_vortexes);
                var freeVortexes = _vortexesFromMemory.GetListOfFreeVortex();

                _velocityComputeShader.SetInt("vortexNumber", _vortexStructArray.Length);
                _velocityComputeShader.SetInt("freeVortexNumber", freeVortexes.Count);
                var startIndex = _vortexStructArray.Length - freeVortexes.Count;
                _velocityComputeShader.SetInt("startFreeVortexNumber", startIndex);

                _velocityComputeShader.SetVector4("flowVelocity",
                                    new Vector4((float)_flowVelovity.X, (float)_flowVelovity.Y, 0, 1));
                //_velocityBuffer.BindLayout(1);
                //_vortexBuf.BindLayout(1);
                _velocityComputeShader.Compute(1024 * 8 / 128, 1, 1);
                var gpuVortexVelocities = _velocityBuffer.GetVector2Data();
                for (var i = 0; i < freeVortexes.Count; i++)
                {
                    freeVortexes[i].InVelocity = new Vector(gpuVortexVelocities[i + startIndex].X, gpuVortexVelocities[i + startIndex].Y);
                }

                //  _vortexSystem.NextStep();
                _vortexSystem.NextStepGPU();
                _vortexesFromMemory = new VortexesFromMemory(_vortexSystem);
                _vortexStructArray = _vortexesFromMemory.GetArrayOfVortexStruct();
                Console.WriteLine("Len : " + _vortexStructArray.Length);
                //_vortexBuf.SubData(_vortexes, 1);
                _vortexBuf.SubData(_vortexStructArray);
            }

            _counter++;

            _particleComputeShader.SetInt("vortexNumber", _vortexStructArray.Length);
            _particleComputeShader.SetFloat("deltaTime", 0.05f);
            _particleComputeShader.SetVector4("flowVelocity",
                new Vector4((float)_flowVelovity.X, (float)_flowVelovity.Y, 0, 1));

            // _particleBuf.BindLayout(0);
            //_vortexBuf.BindLayout(1);
            //_particleVelBuf.BindLayout(2);
            //_startPos.BindLayout(3);
            //_lifeTimeBuf.BindLayout(4);

            _particleComputeShader.Compute(_particleNumber / 128, 1, 1);

            SetParticleShader();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _particleShader.Use();
            // Draw the particles
            _particleShader.SetVector4("Color", new Vector4(0.9f, 0.7f, 0, 0.8f));
            GL.PointSize(1.0f);
            _particleVAO.Draw(PrimitiveType.Points, 0, (int)(_nParticles.x * _nParticles.y * _nParticles.z));

            _vortexShader.Use();
            //_vortexVAO.Bind();
            //GL.VertexAttribDivisor(1, 1);
            //GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _vortexStructArray.Length);
            // GL.PointSize(8.0f);
            // _vortexVAO.Draw(PrimitiveType.Points, 0, _vortexes.Length);
            //_vortexVAO.Unbind();

            _bodyShader.Use();
            GL.PointSize(8.0f);
            // _bodyVAO.Draw(PrimitiveType.Triangles,0,_bogyTriangles.Count);
            _bodyVAO.Draw(PrimitiveType.Points, 0, _bogyTriangles.Count * 3);


        }

        public static List<Vector4> GetParticles(vec3 nParticles)
        {
            List<Vector4> list = new List<Vector4>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float shiftX = -2.0f;
            float L = 0.9f;
            float thickness = 1.4f;

            for (int k = 0; k < nParticles.z; k++)
                for (int i = 0; i < nParticles.x; i++)
                {
                    for (int j = 0; j < nParticles.y; j++)
                    {

                        var x = (float)(-L + 2 * L * rnd.NextDouble()) + shiftX;
                        var y = (float)(-thickness + 2 * thickness * rnd.NextDouble());
                        float z = 0.0f;
                        var w = 1.1f;
                        var p = new Vector4(x, y, z, w);
                        list.Add(p);

                    }
                }

            return list;
        }

        public static List<Vector4> GetParticlesInCircles(vec3 nParticles)
        {
            List<Vector4> list = new List<Vector4>();
            double R = 0.015;
            var rnd = new Random();
            for (int k = 0; k < nParticles.z; k++)
            {
                float centerX = -1.8f + 0.2f * (float)rnd.NextDouble();
                float centerY = -1.5f + 3 * (float)rnd.NextDouble(); ;

                for (int i = 0; i < nParticles.x; i++)
                {
                    for (int j = 0; j < nParticles.y; j++)
                    {

                        var phi = rnd.NextDouble() * 2.0 * Math.PI;

                        var r = R * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi)) + centerX;
                        var y = (float)(r * Math.Sin(phi)) + centerY;
                        float z = 0.0f;
                        var w = 1.0f;
                        var p = new Vector4(x, y, z, w);
                        list.Add(p);
                    }
                }
            }

            return list;
        }




        private List<Vector4> GetInitialVelocity(vec3 nParticles)
        {
            List<Vector4> lst = new List<Vector4>();

            for (int k = 0; k < nParticles.z; k++)
                for (int i = 0; i < nParticles.x; i++)
                {
                    for (int j = 0; j < nParticles.y; j++)
                    {
                        var x = 0.0f;
                        var y = 0.0f;
                        var z = 0.0f;
                        var w = 1.0f;
                        lst.Add(new Vector4(x, y, z, w));
                    }
                }
            return lst;
        }

        private List<Vector2> GetLifeTime(vec3 nParticles)
        {
            List<Vector2> lst = new List<Vector2>();
            var rnd = new Random();

            for (int k = 0; k < nParticles.z; k++)
                for (int i = 0; i < nParticles.x; i++)
                {
                    for (int j = 0; j < nParticles.y; j++)
                    {
                        var x = 0.0f;
                        var y = (float)(5.5 + 20.0 * rnd.NextDouble());

                        lst.Add(new Vector2(x, y));
                    }
                }
            return lst;
        }


        private void SetOpenGlParameters()
        {
            GL.ClearColor(0, 0.0f, 0.0f, 1);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.PointSize(1.0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void SetParticleShader()
        {
            //_angle = 90;
            _view = glm.lookAt(new vec3(_xCenter, 0, _eyePos), new vec3(_xCenter, 0, 0), new vec3(0, 1, 0));
            _model = glm.rotate(new mat4(1.0f), glm.radians(_angle), new vec3(1, 0.0f, 0.0f));
            mat4 proj = glm.perspective(glm.radians(60.0f), (float)Width / Height, 1.0f, 100.0f);

            var model = _model.ConvertToMatrix4();
            var view = _view.ConvertToMatrix4();
            var projection = proj.ConvertToMatrix4();
            _particleShader.SetMatrix4("model", model);
            _particleShader.SetMatrix4("view", view);
            _particleShader.SetMatrix4("projection", projection);


            _vortexShader.SetMatrix4("model", model);
            _vortexShader.SetMatrix4("view", view);
            _vortexShader.SetMatrix4("projection", projection);

            _bodyShader.SetMatrix4("model", model);
            _bodyShader.SetMatrix4("view", view);
            _bodyShader.SetMatrix4("projection", projection);

        }

        public void SetEye(float eyePos)
        {
            _eyePos = eyePos;
        }

        public void SetHorizontal(float xPosition)
        {
            _xCenter = xPosition;
        }
    }
}

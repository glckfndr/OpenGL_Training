using DiscreteVortexLibrary;
using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DemoFlowVisualization
{
    public class RectangleFlow : ISimulator
    {
        //  shaders for computing;
        private Shader _particleComputeShader;
        private Shader _particleShader;
        private Shader _vortexShader;
        private Shader _bodyShader;
        private Shader _velocityComputeShader;

        private VortexSystem _vortexSystem;
        private VortexesFromMemory _vortexesFromMemory;

        private StorageBuffer _particlePositionBuf;
        private StorageBuffer _particleVelBuf;
        private StorageBuffer _lifeTimeBuf;
        private StorageBuffer _vortexBuf;
        private StorageBuffer _startPosBuf;
        private StorageBuffer _bodyPositionBuf;
        private StorageBuffer _velocityVortexBuffer;

        private VertexArray _particleVAO;
        private VertexArray _bodyVAO;
        private VertexArray _vortexVAO;

        //private vec3 _nParticles = new vec3(128, 128, 1);
        private vec3 _nParticles = new vec3(32, 32, 16);

        private int _maxNumberOfVortex = 1024 * 12;
        //private vec3 _nParticlesCircle = new vec3(128, 128, 1);
        private int _particleNumber;
        private Vector _flowVelovity = new Vector(1.0, 0.0);

        private MvpMatrix _mvp = new MvpMatrix();
        private float _eyePos = 10.0f;
        private float _angle = 0;
        private int Height;
        private int Width;
        private int _counter = 0;
        private VortexStruct[] _vortexStructArray = null;
        private float _xCenter = 1;
        private Texture2D _texture;
        private List<Vector2> _bogyTriangles;

        public RectangleFlow(int width, int height)
        {
            Height = height;
            Width = width;

            SetGeometryAndVortexSystem();
            _particleNumber = (int)(_nParticles.x * _nParticles.y * _nParticles.z);
            SetOpenGlParameters();
            CreateShaders();
            CreateBuffers();

            _vortexesFromMemory = new VortexesFromMemory(_vortexSystem);
            _vortexStructArray = _vortexesFromMemory.GetArrayOfVortexStruct();
            _vortexBuf.SubData(_vortexStructArray);

            Console.WriteLine("Flow Around Building");
            Console.WriteLine("Move camera left, right:  left, right arrow key");
            Console.WriteLine("Zoom camera:  up, down arrow key");
            Console.WriteLine("Pause, Continue:  space, C key");
            Console.WriteLine("On, Off vortex:  V, N key");
        }

        private void SetGeometryAndVortexSystem()
        {
            var shapeCollection = GetBuildings();
            var ds = new Discretizator(0.1, 0.1);
            _vortexSystem = new VortexSystem(shapeCollection, ds, new RankinVortex());
            _vortexSystem.FlowVelocity = _flowVelovity;
            _vortexSystem.FirstStep();
        }

        private GeometryShapeCollection GetBuildings()
        {
            var polygonPoints = new List<Vector>
            {
                new Vector(-0.5, -0.5),
                new Vector(-0.5, 0.5),
                new Vector(0.5, 0.5),
                new Vector(0.5, -0.5),
            };

            var shapes = new List<(Vector scale, Vector offset, double angle)>() {
                (new Vector(0.5, 1.75), new Vector(2.0, -1), 0),
                (new Vector(0.6, 1.2), new Vector(0, -1), 0),
                (new Vector(0.6, 2), new Vector(2, 3.9), 0),
                (new Vector(0.6, 1.2), new Vector(0, 1.0), 0),
                (new Vector(0.5, 1.0), new Vector(2, 1.25), 0)

            };

            _bogyTriangles = new List<Vector2>();
            GeometryShapeCollection shapeCollection = new GeometryShapeCollection();
           // GeometryShape polygon;
            for (var i = 0; i < shapes.Count; i++)
            {
                var plg = CreateRectangle(polygonPoints, shapes[i]);
                _bogyTriangles.AddRange(GetVertex(plg.Triangulation));
                shapeCollection.AddShape(plg);
            }

            double offsetX = 4;
            double offsetY = 2.5;
            double angle = 90;
            shapes = new List<(Vector scale, Vector offset, double angle)>() {
                (new Vector(0.5, 1.75), new Vector(2.5 + offsetX, -1 + offsetY), angle),
                (new Vector(0.6, 1.2), new Vector(0 + offsetX, -1 + offsetY), angle),
                (new Vector(0.7, 1.2), new Vector(0 + offsetX, 0.75 + offsetY), angle),
                (new Vector(0.5, 1.2), new Vector(2 + offsetX, 0.75 + offsetY), angle)                
            };

            //for (var i = 0; i < shapes.Count; i++)
            //{
            //    var plg = CreateRectangle(polygonPoints, shapes[i]);
            //    _bogyTriangles.AddRange(GetVertex(plg.Triangulation));
            //    shapeCollection.AddShape(plg);
            //}
            
            return shapeCollection;
        }

        private static GeometryShape CreateRectangle(List<Vector> polygonPoints, (Vector scale, Vector offset, double angle) shape)
        {
            GeometryShape polygon = new GeometryShape(polygonPoints, true);
            polygon.SetScaleRotateMove(shape.scale, shape.offset, shape.angle);
            return polygon;
        }

        private List<Vector2> GetVertex(List<Triangle> polygonTriangulation)
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
             var particles = GetParticles(_nParticles).ToArray();
           // var particles = GetParticlesInCircles(_nParticles).ToArray();
            _particlePositionBuf = StorageBuffer.SetBufferData(particles, 0); // layout 0 in particle.comp, particle.vert

            var vortexStructs = new VortexStruct[_maxNumberOfVortex];
            _vortexBuf = StorageBuffer.AllocateBufferData(vortexStructs, 1); // layout 1 in particle.comp, Velocity2D.comp
            var vel = new Vector2[_maxNumberOfVortex];
            _velocityVortexBuffer = StorageBuffer.AllocateBufferData(vel, 11); // layout 11 in Velocity2D.comp

            _particleVelBuf = StorageBuffer.SetBufferData(GetInitialVelocity(_nParticles).ToArray(), 2); // layout 2 in particle.comp, particle.vert
            _startPosBuf = StorageBuffer.SetBufferData(particles, 3); // layout 3 in particle.comp
            _lifeTimeBuf = StorageBuffer.SetBufferData(GetLifeTime(_nParticles).ToArray(), 4); // layout 4 in particle.comp
            _bodyPositionBuf = StorageBuffer.SetBufferData(_bogyTriangles.ToArray(), 6); // layout 6 in body.vert
            // Set up the VAO
            _particleVAO = VertexArray.GetVAO(new[] { _particlePositionBuf, _particleVelBuf },
                                                new[] { _particlePositionBuf.LayoutShaderIndex, _particleVelBuf.LayoutShaderIndex },
                                                new[] { 4, 4 });
            _vortexVAO = VertexArray.GetVAO(new[] { _vortexBuf }, new[] { _vortexBuf.LayoutShaderIndex }, new[] { 4 });
            _bodyVAO = VertexArray.GetVAO(new[] { _bodyPositionBuf }, new[] { _bodyPositionBuf.LayoutShaderIndex }, new[] { 2 });

        }        

        private void CreateShaders()
        {
            // шейдер для розрахунку руху частинок
            _particleComputeShader = new Shader("../../Shaders/particle.comp");
            // шейдер для розрахунку руху вихорів
            _velocityComputeShader = new Shader("../../Shaders/Velocity2D.comp");
            // шейдер для відображення частинок
            _particleShader = new Shader("../../Shaders/particles.vert",
                "../../Shaders/particles.frag");
            // шейдер для відображення вихорів текстурами
            _vortexShader = new Shader("../../Shaders/vortexTex.vert",
                "../../Shaders/vortexTex.frag");
            // шейдер для відображення тіл
            _bodyShader = new Shader("../../Shaders/body.vert",
                "../../Shaders/body.frag");
        }

        public void ComputeAndDraw(bool isPause, bool is3D, bool isDrawVortex)
        {
            if (_vortexStructArray.Length > _maxNumberOfVortex)
                isPause = true;
            if (!isPause)
            {
                if (_counter % 1 == 0)
                {
                   // Console.WriteLine(_vortexSystem.Time);
                    _counter = 0;

                    CalculateVortexVelocity();
                    // робимо крок по часу методом дискретних вихорів
                    //  _vortexSystem.NextStep();
                    _vortexSystem.NextStepGPU();
                    CopyVortexInGPU();
                  //  Console.WriteLine("Len : " + _vortexStructArray.Length);
                }

                _counter++;
                ComputeParticlePosition();
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            //  малювання частинок
            DrawParticleWithPoints();
            // малювання вихорів текстурами
            if (isDrawVortex)
                DrawVortexWithTexture();
            // малювання тіл
            DrawBody();
        }

        private void CalculateVortexVelocity()
        {
            var freeVortexes = _vortexesFromMemory.GetListOfFreeVortex();
            var startIndex = ComputeVelocityWithShader(freeVortexes);
            SetVoretexVelocity(freeVortexes, startIndex);
        }

        private void DrawBody()
        {
            _bodyShader.SetMvpMatrix(_mvp.GetModel(), _mvp.GetView(), _mvp.GetProjection());
            _bodyShader.Use();
            _bodyVAO.Draw(PrimitiveType.Triangles, 0, _bogyTriangles.Count);
            // GL.PointSize(8.0f);
            // _bodyVAO.Draw(PrimitiveType.Points, 0, _bogyTriangles.Count);
        }

        private void DrawParticleWithPoints()
        {            
            _particleShader.Use();
            _particleShader.SetMvpMatrix(_mvp.GetModel(), _mvp.GetView(), _mvp.GetProjection());
            _particleShader.SetVector4("Color", new Vector4(0.9f, 0.7f, 0, 0.8f));
            //_particleShader.SetVector4("Color", new Vector4(0.0f, 0.0f, 0, 0.8f));
            GL.PointSize(1.0f);
            _particleVAO.Draw(PrimitiveType.Points, 0, (int)(_nParticles.x * _nParticles.y * _nParticles.z));
        }

        private void DrawVortexWithTexture()
        {
            _vortexShader.Use();
            _vortexShader.SetMvpMatrix(_mvp.GetModel(), _mvp.GetView(), _mvp.GetProjection());
            _vortexVAO.Bind();
            GL.VertexAttribDivisor(1, 1);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _vortexStructArray.Length);
            _vortexVAO.Unbind();
        }

        private void CopyVortexInGPU()
        {
            _vortexesFromMemory = new VortexesFromMemory(_vortexSystem);
            // формуємо структури вихорів із вихрової системи
            _vortexStructArray = _vortexesFromMemory.GetArrayOfVortexStruct();
            // копіюємо структури вихорів на відеокарту
            _vortexBuf.SubData(_vortexStructArray);
        }

        private void SetVoretexVelocity(List<Vortex> freeVortexes, int startIndex)
        {
            var gpuVortexVelocities = _velocityVortexBuffer.GetVector2Data();
            for (var i = 0; i < freeVortexes.Count; i++)
            {
                freeVortexes[i].InVelocity =
                    new Vector(gpuVortexVelocities[i + startIndex].X, gpuVortexVelocities[i + startIndex].Y);
            }
        }

        private void ComputeParticlePosition()
        {
            _particleComputeShader.SetInt("vortexNumber", _vortexStructArray.Length);
            _particleComputeShader.SetFloat("deltaTime", 0.05f);
            _particleComputeShader.SetVector4("flowVelocity",
                new Vector4((float)_flowVelovity.X, (float)_flowVelovity.Y, 0, 1));
            // обчислення позицій частинок
            _particleComputeShader.Compute(_particleNumber / 128, 1, 1);
        }

        private int ComputeVelocityWithShader(List<Vortex> freeVortexes)
        {
            _velocityComputeShader.SetInt("vortexNumber", _vortexStructArray.Length);
            _velocityComputeShader.SetInt("freeVortexNumber", freeVortexes.Count);
            var startIndex = _vortexStructArray.Length - freeVortexes.Count;
            _velocityComputeShader.SetInt("startFreeVortexNumber", startIndex);

            _velocityComputeShader.SetVector4("flowVelocity",
                new Vector4((float)_flowVelovity.X, (float)_flowVelovity.Y, 0, 1));

            _velocityComputeShader.Compute(_maxNumberOfVortex / 128, 1, 1);
            return startIndex;
        }

        public static List<Vector4> GetParticles(vec3 nParticles)
        {
            List<Vector4> list = new List<Vector4>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float shiftX = -0.5f; // position of particle source on X
            float shiftY = 1.3f;
            float L = 0.3f;
            float thickness = 4.0f;

            for (int k = 0; k < nParticles.z; k++)
                for (int i = 0; i < nParticles.x; i++)
                {
                    for (int j = 0; j < nParticles.y; j++)
                    {
                        var x = (float)(-L + 2 * L * rnd.NextDouble()) + shiftX;
                        var y = (float)(-thickness + 2 * thickness * rnd.NextDouble()) + shiftY;
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
                float centerY = -1.0f + 4 * (float)rnd.NextDouble(); ;

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
                        var y = (float)(0.5 + 20.0 * rnd.NextDouble());
                        lst.Add(new Vector2(x, y));
                    }
                }

            return lst;
        }


        private void SetOpenGlParameters()
        {
            // GL.ClearColor(1, 1.0f, 1.0f, 1);
            GL.ClearColor(0, 0.0f, 0.0f, 1);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.PointSize(1.0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
                
        public void SetViewPoint(float xPosition, float eyePos)
        {
            _xCenter = xPosition;
            _eyePos = eyePos;
            _mvp.SetMvpMatrix(_xCenter, _eyePos, _angle, (float)Width / Height);
        }
    }
}

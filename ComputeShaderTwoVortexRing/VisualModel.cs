using OpenTK;
using OpenGLHelper;
using OpenTK.Graphics.OpenGL4;
using GlmNet;

namespace ComputeShaderTwoVortexRing
{
    internal class VisualModel
    {
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;

        private float _yEyePos = 0.0f;
        private float _yLookCenter = 0.0f;
        private float _xEyePos = 2.0f;

        public Matrix4 GetView() => _view.ConvertToMatrix4();
        public Matrix4 GetModel() => _model.ConvertToMatrix4();
        public Matrix4 GetProjection() => _projection.ConvertToMatrix4();


        public VisualModel(int Width, int Height)
        {
            _view = glm.lookAt(new vec3(_xEyePos, _yEyePos, 0), new vec3(0, _yLookCenter, 0), new vec3(0, 1, 0));
            _model = new mat4(1.0f);
            _projection = glm.perspective(glm.radians(50.0f),
                                        (float)Width / Height, 0.1f, 100.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0, 0.0f, 0.0f, 1);
            GL.PointSize(1.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        }

        

        internal void SetMatricesForShader(Shader shader)
        {
            _view = glm.lookAt(new vec3(_xEyePos, _yEyePos, 0),
                new vec3(0, _yLookCenter, 0), new vec3(0, 1, 0));

            shader.Use();
            shader.SetMatrix4("model", _model.ConvertToMatrix4());
            shader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            shader.SetMatrix4("view", _view.ConvertToMatrix4());
        }


        public (Matrix3 normal, Matrix4 model) GetNormal(float yTranslate)
        {
            var model = new mat4(1.0f);
            float angle = 90;
            model = glm.rotate(model, -glm.radians(angle), new vec3(1, 0, 0));
            model = glm.translate(model, new vec3(0, 0, yTranslate));
            mat4 mv = _view * model;
            var norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));
            return (norm.ConvertToMatrix3(), model.ConvertToMatrix4());
        }

        internal void SetLookCenter(float delta)
        {
            _yLookCenter += delta;
        }

        internal void SetEyeX(float delta)
        {
            _xEyePos += delta;
        }

        internal void SetEyeY(float delta)
        {
            _yEyePos += delta;
        }
    }
}

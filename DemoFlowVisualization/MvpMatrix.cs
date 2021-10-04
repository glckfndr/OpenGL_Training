using GlmNet;
using OpenTK;
using OpenGLHelper;

namespace DemoFlowVisualization
{
    public class MvpMatrix
    {
        private Matrix4 _view;
        private Matrix4 _model;
        private Matrix4 _projection;

        public void SetMvpMatrix(float _xCenter, float _eyePos, float _angle, float ratio)
        {
            var view = glm.lookAt(new vec3(_xCenter, 0, _eyePos), new vec3(_xCenter, 0, 0), new vec3(0, 1, 0));
            var model = glm.rotate(new mat4(1.0f), glm.radians(_angle), new vec3(1, 0.0f, 0.0f));
            var proj = glm.perspective(glm.radians(60.0f), ratio, 1.0f, 100.0f);
             _model = model.ConvertToMatrix4();
             _view = view.ConvertToMatrix4();
             _projection = proj.ConvertToMatrix4();
        }

        public Matrix4 GetModel() => _model;
        public Matrix4 GetView() => _view;
        public Matrix4 GetProjection() => _projection;
        
    }
}

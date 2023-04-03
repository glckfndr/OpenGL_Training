using GlmNet;
using OpenTK;
using OpenGLHelper;
using System;

namespace DemoFlowVisualization
{
    public class MvpMatrix
    {
        private Matrix4 _view;
        private Matrix4 _model;
        private Matrix4 _projection;
        private mat4 _viewGlm;
        private mat4 _modelGlm;

        public void SetMvpMatrix(float _xCenter, float _eyePos, float _angle, float ratio)
        {
            _modelGlm = new mat4(1.0f);
            _viewGlm = glm.lookAt(new vec3(_xCenter, 0, _eyePos), new vec3(_xCenter, 0, 0), new vec3(0, 1, 0));
            
           // _modelGlm = glm.rotate(new mat4(1.0f), glm.radians(_angle), new vec3(0, 1.0f, 0.0f));
            var proj = glm.perspective(glm.radians(60.0f), ratio, 0.1f, 100.0f);
            _model = _modelGlm.ConvertToMatrix4();
            _view = _viewGlm.ConvertToMatrix4();
            _projection = proj.ConvertToMatrix4();
        }

        public Matrix4 GetModel() => _model;
        public Matrix4 GetView() => _view;
        public Matrix4 GetProjection() => _projection;

        public mat4 GetVM() => _viewGlm * _modelGlm;   
        
    }
}

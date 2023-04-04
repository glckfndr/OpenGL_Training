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
        private Matrix3 _normalMatrix;

        public void SetMvpMatrix(float xCenter, float yCenter, float eyePos, float angle, float ratio)
        {
            _modelGlm = new mat4(1.0f);
            if (angle != 0)
                _modelGlm = glm.rotate(_modelGlm, glm.radians(angle), new vec3(1, 0.0f, 0.0f));

            _viewGlm = glm.lookAt(new vec3(xCenter, yCenter, eyePos), new vec3(xCenter, yCenter, 0), new vec3(0, 1, 0));
            var proj = glm.perspective(glm.radians(60.0f), ratio, 0.1f, 100.0f);
            _model = _modelGlm.ConvertToMatrix4();
            _view = _viewGlm.ConvertToMatrix4();
            _projection = proj.ConvertToMatrix4();

            var mv = _viewGlm * _modelGlm;
            mat3 norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));
            _normalMatrix = norm.ConvertToMatrix3();
        }

        public Matrix3 GetNormalMatrix() => _normalMatrix;
        public Matrix4 GetModel() => _model;
        public Matrix4 GetView() => _view;
        public Matrix4 GetProjection() => _projection;       

    }
}

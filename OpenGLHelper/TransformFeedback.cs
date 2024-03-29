﻿using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class TransformFeedback
    {
        private int _handle;
        public TransformFeedback()
        {
            _handle = GL.GenTransformFeedback();
            Bind();
        }

        public void Bind(TransformFeedbackTarget target = TransformFeedbackTarget.TransformFeedback)
        {
            GL.BindTransformFeedback(target, _handle);
        }

        public void UnBind()
        {
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, 0);
        }

        //public static void Varyings(int programHandle, string[] attributeNames, TransformFeedbackMode mode = TransformFeedbackMode.SeparateAttribs)
        //{
        //    GL.TransformFeedbackVaryings(programHandle, attributeNames.Length, attributeNames, mode);
        //}
        
        public void Begin(TransformFeedbackPrimitiveType type = TransformFeedbackPrimitiveType.Points)
        {
            Bind();
            GL.BeginTransformFeedback(type);
        }
        
        public void End()
        {
            GL.EndTransformFeedback();
        }

        public void Draw(PrimitiveType primitiveType = PrimitiveType.Points)
        {
            GL.DrawTransformFeedback(primitiveType, _handle);
        }
    }
}

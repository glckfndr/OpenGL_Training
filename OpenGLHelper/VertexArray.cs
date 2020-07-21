﻿using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class VertexArray
    {
        private int _id;

        // disable copying
        //VertexArray& operator = ( const VertexArray& );
        public VertexArray(VertexArray VAO)
        {
            _id = VAO.GetId();
            Bind();
        }


        public VertexArray()
        {
            _id = GL.GenVertexArray();
            Bind();
        }

        private bool IsOk()
        {
            return GL.IsVertexArray(_id);
        }

        public int GetId()
        {
            return _id;
        }
        
        public void Destroy()
        {
            if (_id != 0)
                GL.DeleteVertexArray(_id);

            _id = 0;
        }

        public void Bind()
        {
            GL.BindVertexArray(_id);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}
using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class VertexArray
    {
        private int _id;

        // disable copying
        //VertexArray( const VertexArray& );
        //VertexArray& operator = ( const VertexArray& );


        public VertexArray()
        {
            // _id = 0;
            Create();
        }

        private bool IsOk()
        {
            return GL.IsVertexArray(_id);
        }

        public int GetId()
        {
            return _id;
        }

        private void Create()
        {
            _id = GL.GenVertexArray();
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

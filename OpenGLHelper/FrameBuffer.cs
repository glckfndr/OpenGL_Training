using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class FrameBuffer
    {
        int _handle;

        public FrameBuffer()
        {
            _handle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
        }

        public void UnBind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void CheckStatus()
        {
            var result = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (result == FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("Framebuffer is complete");
            }
            else
            {
                Console.WriteLine("Framebuffer error: " + result);
            }
        }
    }
}

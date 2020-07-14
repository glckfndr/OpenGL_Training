using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class RenderBuffer
    {
        private readonly int _handle;
        public RenderBuffer(int width, int height)
        {
            _handle = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _handle);

            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.DepthComponent,
                width, height);


        }

        public void BindToFrameBuffer()
        {
            // Bind the depth buffer to the FBO
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, _handle);
        }
    }
}

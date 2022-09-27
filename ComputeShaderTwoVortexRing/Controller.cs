using OpenTK.Input;

namespace ComputeShaderTwoVortexRing
{
    internal class Controller
    {
        private VisualModel _visualModel;

        public Controller(VisualModel visualModel)
        {
            _visualModel = visualModel;
        }

        public void GetInput(KeyboardState input)
        {
            
            if (input.IsKeyDown(Key.Up))
            {
                _visualModel.SetEyeY(-0.025f);
            }

            if (input.IsKeyDown(Key.Down))
            {
                _visualModel.SetEyeY(0.025f);
            }
            if (input.IsKeyDown(Key.W))
            {
                _visualModel.SetEyeX(0.05f);
            }
            if (input.IsKeyDown(Key.S))
            {
                _visualModel.SetEyeX(-0.05f);
            }
            if (input.IsKeyDown(Key.A))
            {
                _visualModel.SetLookCenter(0.025f);
            }
            if (input.IsKeyDown(Key.D))
            {
                _visualModel.SetLookCenter(-0.025f);
            }
        }


    }
}

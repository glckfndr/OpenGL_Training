using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFlowVisualization
{
    interface ISimulator
    {
        void ComputeAndDraw(bool isPause, bool is3D);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace SampleLogic.Components
{
    public class MovementComponent : Component
    {
        public float DX { get; set; }
        public float DY { get; set; }

        public void Init(float dx, float dy)
        {
            DX = dx;
            DY = dy;
        }

        public override void Reset()
        {
            DX = 0;
            DY = 0;
        }
    }
}

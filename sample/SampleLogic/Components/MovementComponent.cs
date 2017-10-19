using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace SampleLogic.Components
{
    public class MovementComponent : IComponent
    {
        public float DX { get; set; }
        public float DY { get; set; }

        public MovementComponent(float dx, float dy)
        {
            DX = dx;
            DY = dy;
        }
    }
}

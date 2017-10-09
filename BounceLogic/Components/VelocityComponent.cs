using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace BounceLogic
{
    public class VelocityComponent : IComponent
    {
        public float X;
        public float Y;

        public VelocityComponent(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}

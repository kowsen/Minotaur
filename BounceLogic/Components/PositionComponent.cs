using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace BounceLogic
{
    public class PositionComponent : IComponent
    {
        public float X;
        public float Y;

        public PositionComponent(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}

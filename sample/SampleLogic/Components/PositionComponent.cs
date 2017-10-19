using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace SampleLogic.Components
{
    public class PositionComponent : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PositionComponent(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}

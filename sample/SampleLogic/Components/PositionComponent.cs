using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace SampleLogic.Components
{
    public class PositionComponent : Component
    {
        public float X { get; set; }
        public float Y { get; set; }

        public void Init(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override void Reset()
        {
            X = 0;
            Y = 0;
        }
    }
}

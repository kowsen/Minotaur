using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace Game1
{
    public class MovementSystem : GameSystem
    {
        public MovementSystem()
        {
            AddComponentConstraint<PositionComponent>();
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var position = entity.GetComponent<PositionComponent>();
            position.X += 1;
            position.Y += 1;
        }
    }
}

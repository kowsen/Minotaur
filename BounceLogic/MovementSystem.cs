using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace Game1
{
    public class MovementSystem : EntitySystem
    {
        public MovementSystem()
        {
            AddComponentConstraint<PositionComponent>();
            AddComponentConstraint<VelocityComponent>();
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var position = entity.GetComponent<PositionComponent>();
            var velocity = entity.GetComponent<VelocityComponent>();
            position.X += velocity.X;
            position.Y += velocity.Y;
        }
    }
}

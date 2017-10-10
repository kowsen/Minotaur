using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using BounceLogic.Components;

namespace BounceLogic.Systems
{
    public class MovementSystem : EntitySystem
    {
        public MovementSystem()
        {
            AddComponentRequirement<PositionComponent>();
            AddComponentRequirement<VelocityComponent>();
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

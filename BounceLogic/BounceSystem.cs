using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace Game1
{
    public class BounceSystem : EntitySystem
    {
        private int _screenWidth;
        private int _screenHeight;

        public BounceSystem(int screenWidth, int screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            AddComponentConstraint<PositionComponent>();
            AddComponentConstraint<VelocityComponent>();
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var position = entity.GetComponent<PositionComponent>();
            var velocity = entity.GetComponent<VelocityComponent>();
            if (position.X < 0 && velocity.X < 0)
            {
                velocity.X = -velocity.X;
            }
            if (position.Y < 0 && velocity.Y < 0)
            {
                velocity.Y = -velocity.Y;
            }
            if (position.X > _screenWidth && velocity.X > 0)
            {
                velocity.X = -velocity.X;
            }
            if (position.Y > _screenHeight && velocity.Y > 0)
            {
                velocity.Y = -velocity.Y;
            }
        }
    }
}

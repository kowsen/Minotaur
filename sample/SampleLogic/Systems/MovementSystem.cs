using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;
using SampleLogic.Components;

namespace SampleLogic.Systems
{
    public class MovementSystem : EntitySystem<SampleGameObject>
    {
        public MovementSystem()
        {
            var requirements = new List<Type>()
            {
                typeof(PositionComponent),
                typeof(MovementComponent)
            };
            var restrictions = new List<Type>();
            Signature = ComponentSignatureManager.GenerateComponentSignature(
                requirements,
                restrictions
            );
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var position = entity.GetComponent<PositionComponent>();
            var movement = entity.GetComponent<MovementComponent>();

            // var screenWidth = Game.Viewport.VirtualWidth;
            // var screenHeight = Game.Viewport.VirtualHeight;
            var screenWidth = 100;
            var screenHeight = 100;

            var didBounce = false;

            if (
                (position.X < 0 && movement.DX < 0) || (position.X > screenWidth && movement.DX > 0)
            )
            {
                movement.DX = -movement.DX;
                didBounce = true;
            }
            if (
                (position.Y < 0 && movement.DY < 0)
                || (position.Y > screenHeight && movement.DY > 0)
            )
            {
                movement.DY = -movement.DY;
                didBounce = true;
            }

            if (didBounce)
            {
                Game.Bus.Notify(new BounceEvent(entity.Id));
            }

            position.X += movement.DX;
            position.Y += movement.DY;
        }
    }
}

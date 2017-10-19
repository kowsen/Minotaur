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
    public class PlayerInputSystem : EntitySystem<SampleGameObject>
    {
        public PlayerInputSystem()
        {
            var requirements = new List<Type>() { typeof(MovementComponent), typeof(PlayerFlagComponent) };
            var restrictions = new List<Type>();
            Signature = ComponentSignatureManager.GenerateComponentSignature(requirements, restrictions);
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var movement = entity.GetComponent<MovementComponent>();

            movement.DX = 0;
            movement.DY = 0;

            if (Game.Input.IsUp)
            {
                movement.DY -= Game.State.PlayerSpeed;
            }
            if (Game.Input.IsRight)
            {
                movement.DX += Game.State.PlayerSpeed;
            }
            if (Game.Input.IsDown)
            {
                movement.DY += Game.State.PlayerSpeed;
            }
            if (Game.Input.IsLeft)
            {
                movement.DX -= Game.State.PlayerSpeed;
            }
        }
    }
}

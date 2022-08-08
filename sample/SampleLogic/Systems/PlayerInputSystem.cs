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
            SetRequirements(typeof(MovementComponent), typeof(PlayerFlagComponent));
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var movement = entity.GetComponent<MovementComponent>();

            movement.DX = 0;
            movement.DY = 0;

            if (_game.Input.IsUp)
            {
                movement.DY -= _game.State.PlayerSpeed;
            }
            if (_game.Input.IsRight)
            {
                movement.DX += _game.State.PlayerSpeed;
            }
            if (_game.Input.IsDown)
            {
                movement.DY += _game.State.PlayerSpeed;
            }
            if (_game.Input.IsLeft)
            {
                movement.DX -= _game.State.PlayerSpeed;
            }
        }
    }
}

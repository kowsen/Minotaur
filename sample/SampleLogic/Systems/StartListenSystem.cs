using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;

namespace SampleLogic.Systems
{
    public class StartListenSystem : GameSystem<SampleGameObject>
    {
        public override void OnActivate()
        {
            Game.Bus.SpacePress.Event += OnSpacePress;
        }

        public override void OnDeactivate()
        {
            Game.Bus.SpacePress.Event -= OnSpacePress;
        }

        private void OnSpacePress()
        {
            Game.Bus.SpacePress.Emit();
        }
    }
}

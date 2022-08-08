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
            Game.Bus.Register<SpacePressEvent>(OnSpacePress);
        }

        public override void OnDeactivate()
        {
            Game.Bus.Remove<SpacePressEvent>(OnSpacePress);
        }

        private void OnSpacePress(SpacePressEvent spacePressEvent)
        {
            Game.Bus.Notify(new StartEvent());
        }
    }
}

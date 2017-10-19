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
            _game.Bus.Register(Events.SPACE_PRESS, OnSpacePress);
        }

        public override void OnDeactivate()
        {
            _game.Bus.Remove(Events.SPACE_PRESS, OnSpacePress);
        }

        private void OnSpacePress(EventBusArgs args)
        {
            _game.Bus.Notify(Events.START);
        }
    }
}

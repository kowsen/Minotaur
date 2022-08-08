using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;

namespace SampleLogic.Systems
{
    public class BounceNotifySystem : GameSystem<SampleGameObject>
    {
        public override void OnActivate()
        {
            Game.Bus.Register<BounceEvent>(OnBounce);
        }

        public override void OnDeactivate()
        {
            Game.Bus.Remove<BounceEvent>(OnBounce);
        }

        private void OnBounce(BounceEvent bounceEvent)
        {
            Console.WriteLine($"Bounce by: {bounceEvent.Id}");
        }
    }
}

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
            Game.Bus.Bounce.Event += OnBounce;
        }

        public override void OnDeactivate()
        {
            Game.Bus.Bounce.Event -= OnBounce;
        }

        private void OnBounce(int id)
        {
            Console.WriteLine($"Bounce by: {id}");
        }
    }
}

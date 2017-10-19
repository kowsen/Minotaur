﻿using System;
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
            _game.Bus.Register(Events.BOUNCE, OnBounce);
        }

        public override void OnDeactivate()
        {
            _game.Bus.Remove(Events.BOUNCE, OnBounce);
        }

        private void OnBounce(EventBusArgs args)
        {
            var bounceArgs = args.Unpack<BounceArgs>();
            Console.WriteLine($"Bounce by: {bounceArgs.Id}");
        }
    }
}

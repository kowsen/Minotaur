using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;

namespace SampleLogic
{
    public class SampleWorld : World<SampleGameObject>
    {
        public SampleWorld(SampleGameObject game) : base(game) { }

        public override void InitializeListeners()
        {
            Game.Bus.Register(Events.START, (_) =>
            {
                Switch("main");
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class ActionDelayerUpdateSystem<T> : GameSystem<T>
    {
        public override void Update(TimeSpan time)
        {
            ActionDelayer.Update();
        }
    }
}

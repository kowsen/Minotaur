using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EaseValueUpdateSystem<T> : GameSystem<T>
    {
        public override void Update(TimeSpan time)
        {
            EaseValue.UpdateAll(time);
        }
    }
}

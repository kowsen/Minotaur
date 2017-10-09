using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EventBus
    {
        public event Action<string> OnEvent;

        public void Notify(string val)
        {
            OnEvent?.Invoke(val);
        }
    }
}

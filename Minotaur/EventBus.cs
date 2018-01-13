using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EventBus<T>
    {
        private Dictionary<T, List<Action<IEventBusArgs>>> _callbacks;

        public EventBus()
        {
            _callbacks = new Dictionary<T, List<Action<IEventBusArgs>>>();
        }

        public void Register(T name, Action<IEventBusArgs> cb)
        {
            var success = _callbacks.TryGetValue(name, out var callbacks);
            if (!success)
            {
                callbacks = new List<Action<IEventBusArgs>>();
                _callbacks[name] = callbacks;
            }
            callbacks.Add(cb);
        }

        public void Remove(T name, Action<IEventBusArgs> cb)
        {
            _callbacks[name].Remove(cb);
        }

        public void Notify(T name, IEventBusArgs arg = null)
        {
            var success = _callbacks.TryGetValue(name, out var callbacks);
            if (success)
            {
                for (var i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i].Invoke(arg);
                }
            }
        }
    }

    public interface IEventBusArgs { }
}

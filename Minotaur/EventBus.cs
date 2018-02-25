using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EventBus<T>
    {
        private Dictionary<T, List<Action<IEventBusArgs>>> _callbacks;

        private List<Tuple<T, Action<IEventBusArgs>>> _toAdd;
        private List<Tuple<T, Action<IEventBusArgs>>> _toRemove;

        public EventBus()
        {
            _callbacks = new Dictionary<T, List<Action<IEventBusArgs>>>();

            _toAdd = new List<Tuple<T, Action<IEventBusArgs>>>();
            _toRemove = new List<Tuple<T, Action<IEventBusArgs>>>();
        }

        public void Register(T name, Action<IEventBusArgs> cb)
        {
            _toAdd.Add(new Tuple<T, Action<IEventBusArgs>>(name, cb));
        }

        public void Remove(T name, Action<IEventBusArgs> cb)
        {
            _toRemove.Add(new Tuple<T, Action<IEventBusArgs>>(name, cb));
        }

        public void CommitCallbackChanges()
        {
            foreach (var val in _toAdd)
            {
                var name = val.Item1;
                var cb = val.Item2;
                var success = _callbacks.TryGetValue(name, out var callbacks);
                if (!success)
                {
                    callbacks = new List<Action<IEventBusArgs>>();
                    _callbacks[name] = callbacks;
                }
                callbacks.Add(cb);
            }

            foreach (var val in _toRemove)
            {
                var name = val.Item1;
                var cb = val.Item2;
                _callbacks[name].Remove(cb);
            }

            _toAdd.Clear();
            _toRemove.Clear();
        }

        public void Notify(T name, IEventBusArgs arg = null)
        {
            CommitCallbackChanges();
            var success = _callbacks.TryGetValue(name, out var callbacks);
            if (success)
            {
                for (var i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i].Invoke(arg);
                }
            }
            CommitCallbackChanges();
        }
    }

    public interface IEventBusArgs { }
}

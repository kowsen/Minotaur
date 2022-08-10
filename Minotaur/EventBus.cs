using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EventBus
    {
        private Dictionary<Type, List<Action<IEvent>>> _callbacks =
            new Dictionary<Type, List<Action<IEvent>>>();

        private List<EventListener<IEvent>> _toAdd = new List<EventListener<IEvent>>();
        private List<EventListener<IEvent>> _toRemove = new List<EventListener<IEvent>>();
        private List<IEvent> _toNotify = new List<IEvent>();

        public void Register<TEvent>(Action<TEvent> cb) where TEvent : IEvent
        {
            _toAdd.Add(new EventListener<IEvent>(cb as Action<IEvent>));
        }

        public void Remove<TEvent>(Action<TEvent> cb) where TEvent : IEvent
        {
            _toRemove.Add(new EventListener<IEvent>(cb as Action<IEvent>));
        }

        public void CommitCallbackChanges()
        {
            foreach (var listener in _toAdd)
            {
                var success = _callbacks.TryGetValue(listener.EventType, out var callbacks);
                if (!success)
                {
                    callbacks = new List<Action<IEvent>>();
                    _callbacks[listener.EventType] = callbacks;
                }
                callbacks.Add(listener.Action);
            }

            foreach (var listener in _toRemove)
            {
                _callbacks[listener.EventType].Remove(listener.Action);
            }

            _toAdd.Clear();
            _toRemove.Clear();
        }

        private bool isNotifying = false;

        private void NotifyImmediately<TEvent>(TEvent data) where TEvent : IEvent
        {
            isNotifying = true;
            CommitCallbackChanges();
            var success = _callbacks.TryGetValue(typeof(TEvent), out var callbacks);
            if (success)
            {
                foreach (var callback in callbacks)
                {
                    callback.Invoke(data);
                }
            }
            CommitCallbackChanges();
            isNotifying = false;
        }

        public void Notify<TEvent>(TEvent data) where TEvent : IEvent
        {
            if (isNotifying)
            {
                _toNotify.Add(data);
            }
            else
            {
                NotifyImmediately(data);
                DrainNotifyQueue();
            }
        }

        private void DrainNotifyQueue()
        {
            while (_toNotify.Count > 0)
            {
                NotifyImmediately(_toNotify[0]);
                _toNotify.RemoveAt(0);
            }
        }
    }

    public struct EventListener<TEvent> where TEvent : IEvent
    {
        public Type EventType;
        public Action<TEvent> Action;

        public EventListener(Action<TEvent> action)
        {
            Action = action;
            EventType = typeof(TEvent);
        }
    }

    public interface IEvent { }
}

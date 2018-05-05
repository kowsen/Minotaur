using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class ErrandManager<T>
    {
        private Dictionary<Type, List<Errand<T>>> _errands;
        private EntityComponentManager _entities;
        private T Game;

        private List<ErrandStore> _removeQueue;
        private List<ErrandStore> _addQueue;

        public ErrandManager(EntityComponentManager entities, T game)
        {
            _entities = entities;
            Game = game;
            _errands = new Dictionary<Type, List<Errand<T>>>();

            _removeQueue = new List<ErrandStore>();
            _addQueue = new List<ErrandStore>();
        }

        public U Run<U>() where U : Errand<T>, new()
        {
            var errand = ErrandFactory<T>.Get<U>();
            errand.Attach(this, _entities, Game);
            var type = typeof(U);
            _addQueue.Add(ErrandStoreFactory.Get(errand, type));
            return errand;
        }

        private List<Errand<T>> _provisionStorage = new List<Errand<T>>();
        public void Provision<U>(int num) where U : Errand<T>, new()
        {
            for (var i = 0; i < num; i++)
            {
                _provisionStorage.Add(ErrandFactory<T>.Get<U>());
            }
            for (var i = 0; i < num; i++)
            {
                ErrandFactory<T>.Recycle((U)_provisionStorage[i]);
            }
            if (!_errands.ContainsKey(typeof(U)))
            {
                _errands[typeof(U)] = new List<Errand<T>>();
            }
            _provisionStorage.Clear();
        }

        public void Remove<U>(U value) where U : Errand<T>, new()
        {
            Remove(value, typeof(U));
        }

        public void Remove(Errand<T> value, Type type)
        {
            _removeQueue.Add(ErrandStoreFactory.Get(value, type));
        }

        public void CommitErrandChanges()
        {
            foreach (var item in _addQueue)
            {
                var errand = item.Errand;
                var type = item.Type;
                var doesListExist = _errands.TryGetValue(type, out var errands);
                if (!doesListExist)
                {
                    errands = new List<Errand<T>>();
                    _errands[type] = errands;
                }
                errands.Add(errand);
                ErrandStoreFactory.Recycle(item);
            }

            foreach (var item in _removeQueue)
            {
                var value = item.Errand;
                var type = item.Type;
                var isInitialized = _errands.TryGetValue(type, out var errands);
                if (!isInitialized)
                {
                    throw new Exception($"Trying to remove errand with nonexistant type {type}");
                }

                var isInList = errands.Remove(value);

                if (isInList)
                {
                    ErrandFactory<T>.Recycle(value, type);
                }
                ErrandStoreFactory.Recycle(item);
            }

            _addQueue.Clear();
            _removeQueue.Clear();
        }

        public int NumRunning<U>() where U : Errand<T>
        {
            var isInitialized = _errands.TryGetValue(typeof(U), out var errands);
            if (isInitialized)
            {
                return errands.Count;
            }
            return 0;
        }

        public void Cancel<U>(U value) where U : Errand<T>, new()
        {
            value.End(true);
        }

        public void CancelAll<U>() where U : Errand<T>, new()
        {
            var isInitialized = _errands.TryGetValue(typeof(U), out var errands);
            if (isInitialized)
            {
                var type = typeof(U);
                foreach (var errand in errands)
                {
                    Remove(errand, type);
                }
                for (var i = 0; i < _addQueue.Count; i++)
                {
                    if (_addQueue[i].Type.Equals(type))
                    {
                        Remove(_addQueue[i].Errand, _addQueue[i].Type);
                        ErrandStoreFactory.Recycle(_addQueue[i]);
                        _addQueue.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
        }

        public void Update(TimeSpan time)
        {
            foreach (var errandType in _errands)
            {
                foreach (var errand in errandType.Value)
                {
                    errand.Update(time);
                }
            }
            CommitErrandChanges();
        }

        public void Draw(TimeSpan time, Errand<T>.DrawStatus drawStatus)
        {
            foreach (var errandType in _errands)
            {
                foreach (var errand in errandType.Value)
                {
                    if (errand.DrawOrder == drawStatus)
                    {
                        errand.Draw(time);
                    }
                }
            }
            // Maybe I should just put CommitErrandChanges here for safety, but I
            // stubbornly refuse to enable future me to hack in a change to my errand
            // lists during a draw call. Take that, future Kyle!
        }

        public void Clear()
        {
            foreach (var errandType in _errands)
            {
                foreach (var errand in errandType.Value)
                {
                    errand.OnEnd(true);
                }
            }
            _errands.Clear();
        }

        private class ErrandStore
        {
            public Errand<T> Errand { get; set; }
            public Type Type { get; set; }

            public ErrandStore(Errand<T> errand, Type type)
            {
                Errand = errand;
                Type = type;
            }
        }

        private static class ErrandStoreFactory
        {
            private static Queue<ErrandStore> _queue = new Queue<ErrandStore>();

            public static ErrandStore Get(Errand<T> errand, Type type)
            {
                if (_queue.Count > 0)
                {
                    var item = _queue.Dequeue();
                    item.Errand = errand;
                    item.Type = type;
                    return item;
                }
                else
                {
                    return new ErrandStore(errand, type);
                }
            }

            public static void Recycle(ErrandStore item)
            {
                _queue.Enqueue(item);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class ErrandManager<T>
    {
        private Dictionary<Type, List<Errand<T>>> _errands;
        private EntityComponentManager _entities;
        private T Game;

        private List<ErrandWithType> _removeQueue;
        private List<ErrandWithType> _addQueue;

        public ErrandManager(EntityComponentManager entities, T game)
        {
            _entities = entities;
            Game = game;
            _errands = new Dictionary<Type, List<Errand<T>>>();

            _removeQueue = new List<ErrandWithType>();
            _addQueue = new List<ErrandWithType>();
        }

        public U Run<U>() where U : Errand<T>, new()
        {
            var errand = ErrandFactory<T>.Get<U>();
            errand.Attach(this, _entities, Game);
            var type = typeof(U);
            _addQueue.Add(new ErrandWithType(errand, type));
            return errand;
        }

        public void Remove<U>(U value) where U : Errand<T>, new()
        {
            Remove(value, typeof(U));
        }

        public void Remove(Errand<T> value, Type type)
        {
            _removeQueue.Add(new ErrandWithType(value, type));
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

        private struct ErrandWithType
        {
            public Errand<T> Errand { get; set; }
            public Type Type { get; set; }

            public ErrandWithType(Errand<T> errand, Type type)
            {
                Errand = errand;
                Type = type;
            }
        }
    }
}

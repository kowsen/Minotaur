using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class ErrandManager<T>
    {
        private Dictionary<Type, List<Errand<T>>> _errands;
        private T Game;

        private List<Tuple<Errand<T>, Type>> _removeQueue;
        private List<Tuple<Errand<T>, Type>> _addQueue;

        public ErrandManager(T game)
        {
            Game = game;
            _errands = new Dictionary<Type, List<Errand<T>>>();

            _removeQueue = new List<Tuple<Errand<T>, Type>>();
            _addQueue = new List<Tuple<Errand<T>, Type>>();
        }

        public U Run<U>() where U : Errand<T>, new()
        {
            var errand = ErrandFactory<T>.Get<U>();
            errand.Attach(this, Game);
            var type = typeof(U);
            _addQueue.Add(new Tuple<Errand<T>, Type>(errand, type));
            return errand;
        }

        public void Remove<U>(U value) where U : Errand<T>, new()
        {
            Remove(value, typeof(U));
        }

        public void Remove(Errand<T> value, Type type)
        {
            _removeQueue.Add(new Tuple<Errand<T>, Type>(value, type));
        }

        public void CommitErrandChanges()
        {
            foreach (var item in _addQueue)
            {
                var errand = item.Item1;
                var type = item.Item2;
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
                var value = item.Item1;
                var type = item.Item2;
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
            value.OnEnd(true);
        }

        public void CancelAll<U>() where U : Errand<T>, new()
        {
            var isInitialized = _errands.TryGetValue(typeof(U), out var errands);
            if (!isInitialized)
            {
                throw new Exception($"Trying to cancel all nonexistant errand with type {typeof(U)}");
            }

            while (errands.Count > 0)
            {
                Cancel((U)errands[0]);
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
    }
}

using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class ErrandManager<TGame>
    {
        private Dictionary<Type, List<Errand<TGame>>> _errands;
        private EntityComponentManager _entities;
        private TGame Game;

        private List<ErrandWithType> _removeQueue;
        private List<ErrandWithType> _addQueue;

        public ErrandManager(EntityComponentManager entities, TGame game)
        {
            _entities = entities;
            Game = game;
            _errands = new Dictionary<Type, List<Errand<TGame>>>();

            _removeQueue = new List<ErrandWithType>();
            _addQueue = new List<ErrandWithType>();
        }

        public TErrand Run<TErrand>() where TErrand : Errand<TGame>, new()
        {
            var errand = ErrandFactory<TGame>.Get<TErrand>();
            errand.Attach(this, _entities, Game);
            var type = typeof(TErrand);
            _addQueue.Add(new ErrandWithType(errand, type));
            return errand;
        }

        public void Remove<TErrand>(TErrand errand) where TErrand : Errand<TGame>, new()
        {
            Remove(errand, typeof(TErrand));
        }

        public void Remove(Errand<TGame> value, Type type)
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
                    errands = new List<Errand<TGame>>();
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
                    ErrandFactory<TGame>.Recycle(value, type);
                }
            }

            _addQueue.Clear();
            _removeQueue.Clear();
        }

        public int NumRunning<TErrand>() where TErrand : Errand<TGame>
        {
            var isInitialized = _errands.TryGetValue(typeof(TErrand), out var errands);
            if (isInitialized)
            {
                return errands.Count;
            }
            return 0;
        }

        public void Cancel<TErrand>(TErrand value) where TErrand : Errand<TGame>, new()
        {
            value.End(true);
        }

        public void CancelAll<TErrand>() where TErrand : Errand<TGame>, new()
        {
            var isInitialized = _errands.TryGetValue(typeof(TErrand), out var errands);
            if (isInitialized)
            {
                var type = typeof(TErrand);
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

        public void Draw(TimeSpan time, Errand<TGame>.DrawStatus drawStatus)
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
            public Errand<TGame> Errand { get; set; }
            public Type Type { get; set; }

            public ErrandWithType(Errand<TGame> errand, Type type)
            {
                Errand = errand;
                Type = type;
            }
        }
    }
}

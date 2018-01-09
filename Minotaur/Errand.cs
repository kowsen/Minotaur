using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Errand<T>
    {
        private static Dictionary<Type, Errand<T>> _errands = new Dictionary<Type, Errand<T>>();
        private static List<Type> _typesToRemove = new List<Type>();

        public static void Run<U>(U errand, T game) where U : Errand<T>
        {
            var type = typeof(U);
            CancelType(type, false);
            for (var i = 0; i < errand.ToCancel.Count; i++)
            {
                CancelType(errand.ToCancel[i]);
            }
            _errands[type] = errand;
            errand.Game = game;
            errand.OnBegin();
        }

        public static bool IsRunning<U>() where U : Errand<T>
        {
            return _errands.ContainsKey(typeof(U));
        }

        private static void CancelType(Type type, bool removeFromAll = true)
        {
            var success = _errands.TryGetValue(type, out var errand);
            if (success)
            {
                errand.End(true, removeFromAll);
            }
        }

        public static void UpdateAll(TimeSpan time)
        {
            foreach (var errand in _errands)
            {
                errand.Value.Update(time);
            }
            foreach (var type in _typesToRemove)
            {
                _errands.Remove(type);
            }
            _typesToRemove.Clear();
        }

        public static void Clear()
        {
            _errands.Clear();
        }

        private List<Type> ToCancel = new List<Type>();

        protected void AddCancelType<U>()
        {
            ToCancel.Add(typeof(U));
        }

        protected T Game;

        public virtual void OnBegin() { }
        public virtual void Update(TimeSpan time) { }
        public virtual void OnEnd(bool isCancelled) { }

        public void End(bool isCancelled = false, bool removeFromAll = true)
        {
            if (removeFromAll)
            {
                _typesToRemove.Add(GetType());
            }
            OnEnd(isCancelled);
        }
    }

    public class ErrandSpawner<T>
    {
        public static void Run<U>(U errandSpawner, T game) where U : ErrandSpawner<T>
        {
            errandSpawner.Game = game;
            errandSpawner.OnRun();
        }

        protected void RunErrand<U>(U errand) where U : Errand<T>
        {
            Errand<T>.Run(errand, Game);
        }

        protected T Game;

        public virtual void OnRun() { }
    }
}

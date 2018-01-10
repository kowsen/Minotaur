using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Errand<T>
    {
        private static Dictionary<Type, Errand<T>> _errands = new Dictionary<Type, Errand<T>>();

        public static U Run<U>(T game) where U : Errand<T>
        {
            var type = typeof(U);
            CancelType(type);
            var errand = _errands.ContainsKey(type) ? (U)_errands[type] : (U)Activator.CreateInstance(type);
            for (var i = 0; i < errand.ToCancel.Count; i++)
            {
                CancelType(errand.ToCancel[i]);
            }
            _errands[type] = errand;
            errand.Game = game;
            return errand;
        }

        public static bool IsRunning<U>() where U : Errand<T>
        {
            return _errands.ContainsKey(typeof(U)) && _errands[typeof(U)]._isActive;
        }

        private static void CancelType(Type type)
        {
            var success = _errands.TryGetValue(type, out var errand);
            if (success)
            {
                errand.End(true);
            }
        }

        public static void UpdateAll(TimeSpan time)
        {
            foreach (var errand in _errands)
            {
                if (errand.Value._isActive)
                {
                    errand.Value.Update(time);
                }
            }
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
        private bool _isActive;

        public void Begin()
        {
            _isActive = true;
            OnBegin();
        }

        public virtual void OnBegin() { }
        public virtual void Update(TimeSpan time) { }
        public virtual void OnEnd(bool isCancelled) { }

        public void End(bool isCancelled = false)
        {
            _isActive = false;
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

        protected U RunErrand<U>() where U : Errand<T>
        {
            return Errand<T>.Run<U>(Game);
        }

        protected T Game;

        public virtual void OnRun() { }
    }
}

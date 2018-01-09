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
            CancelType(type);
            for (var i = 0; i < errand.ToCancel.Count; i++)
            {
                CancelType(errand.ToCancel[i]);
            }
            _errands[type] = errand;
            errand.Game = game;
            errand.OnBegin();
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

        public void End(bool isCancelled = false)
        {
            _typesToRemove.Add(GetType());
            OnEnd(isCancelled);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public static class ErrandFactory<T>
    {
        private static Dictionary<Type, Queue<Errand<T>>> _pool = new Dictionary<Type, Queue<Errand<T>>>();

        public static U Get<U>() where U : Errand<T>, new()
        {
            var doesExist = _pool.TryGetValue(typeof(U), out var queue);
            if (doesExist && queue.Count > 0)
            {
                return (U)queue.Dequeue();
            }
            return new U();
        }

        public static void Recycle<U>(U value) where U : Errand<T>, new()
        {
            Recycle(value, typeof(U));
        }

        public static void Recycle(Errand<T> value, Type type)
        {
            var doesExist = _pool.TryGetValue(type, out var queue);
            if (!doesExist)
            {
                queue = new Queue<Errand<T>>();
                _pool[type] = queue;
            }
            queue.Enqueue(value);
        }
    }
}

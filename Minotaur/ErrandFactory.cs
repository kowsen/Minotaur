using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public static class ErrandFactory<TGame>
    {
        private static Dictionary<Type, Queue<Errand<TGame>>> _pool =
            new Dictionary<Type, Queue<Errand<TGame>>>();

        public static TErrand Get<TErrand>() where TErrand : Errand<TGame>, new()
        {
            var doesExist = _pool.TryGetValue(typeof(TErrand), out var queue);
            if (doesExist && queue.Count > 0)
            {
                return (TErrand)queue.Dequeue();
            }
            return new TErrand();
        }

        public static void Recycle<TErrand>(TErrand value) where TErrand : Errand<TGame>, new()
        {
            Recycle(value, typeof(TErrand));
        }

        public static void Recycle(Errand<TGame> value, Type type)
        {
            var doesExist = _pool.TryGetValue(type, out var queue);
            if (!doesExist)
            {
                queue = new Queue<Errand<TGame>>();
                _pool[type] = queue;
            }
            queue.Enqueue(value);
        }
    }
}

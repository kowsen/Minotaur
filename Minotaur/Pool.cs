using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Minotaur
{
    internal interface Poolable
    {
        void Reset();
    }

    internal static class Pool
    {
        private static Dictionary<Type, Action<Poolable>> _recycleActions =
            new Dictionary<Type, Action<Poolable>>();

        public static void RegisterRecycleAction(Type type, Action<Poolable> recycleAction)
        {
            _recycleActions.Add(type, recycleAction);
        }

        public static void Recycle(Type type, Poolable poolable)
        {
            var success = _recycleActions.TryGetValue(type, out var recycleAction);
            if (!success)
            {
                throw new Exception(
                    $"Trying to recycle poolable of type {type} but pool is not registered"
                );
            }
            recycleAction(poolable);
        }
    }

    internal static class Pool<TPoolable> where TPoolable : class, Poolable, new()
    {
        static Pool()
        {
            Pool.RegisterRecycleAction(typeof(TPoolable), TryRecycle);
        }

        private static ConcurrentBag<TPoolable> _pool = new ConcurrentBag<TPoolable>();

        public static TPoolable Get()
        {
            TPoolable item;
            var didTake = _pool.TryTake(out item);
            if (!didTake)
            {
                item = new TPoolable();
            }
            return item;
        }

        public static void Recycle(TPoolable item)
        {
            item.Reset();
            _pool.Add(item);
        }

        public static int GetCount()
        {
            return _pool.Count;
        }

        private static void TryRecycle(Poolable item)
        {
            if (item is TPoolable)
            {
                Recycle(item as TPoolable);
            }
            else
            {
                throw new Exception(
                    $"Trying to recycle poolable of type {item.GetType()} in pool of type {typeof(TPoolable)}"
                );
            }
        }
    }
}

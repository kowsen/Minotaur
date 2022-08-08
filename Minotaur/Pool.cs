using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace Minotaur
{
    public abstract class Poolable
    {
        public Poolable()
        {
            Reset();
        }

        public abstract void Reset();
    }

    public static class Pool
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

    public static class Pool<TPoolable> where TPoolable : Poolable, new()
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

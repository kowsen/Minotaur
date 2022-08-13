using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Minotaur
{
    public abstract class Poolable
    {
        public abstract void Reset();
    }

    public class Pool
    {
        private Dictionary<Type, ConcurrentBag<Poolable>> _pools =
            new Dictionary<Type, ConcurrentBag<Poolable>>();

        public TPoolable Get<TPoolable>() where TPoolable : Poolable, new()
        {
            var pool = GetPool(typeof(TPoolable));
            var poolHadItem = pool.TryTake(out var item);
            if (!poolHadItem)
            {
                item = new TPoolable();
            }
            return (TPoolable)item;
        }

        public void Recycle(Poolable item)
        {
            var pool = GetPool(item.GetType());
            item.Reset();
            pool.Add(item);
        }

        private ConcurrentBag<Poolable> GetPool(Type type)
        {
            var poolExists = _pools.TryGetValue(type, out var pool);
            if (!poolExists)
            {
                pool = new ConcurrentBag<Poolable>();
                _pools.Add(type, pool);
            }
            return pool;
        }
    }
}

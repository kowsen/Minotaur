using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Minotaur
{
    public abstract class Poolable
    {
        public abstract void Reset();
    }

    internal class Pool
    {
        private Dictionary<int, ConcurrentBag<Poolable>> _pools =
            new Dictionary<int, ConcurrentBag<Poolable>>();

        public TPoolable Get<TPoolable>() where TPoolable : Poolable, new()
        {
            var pool = GetPool(TypeId<TPoolable>.Get());
            var poolHadItem = pool.TryTake(out var item);
            if (!poolHadItem)
            {
                item = new TPoolable();
            }
            return (TPoolable)item;
        }

        public void Recycle<TPoolable>(TPoolable item) where TPoolable : Poolable, new()
        {
            Recycle(TypeId<TPoolable>.Get(), item);
        }

        public void Recycle(int typeId, Poolable item)
        {
            var pool = GetPool(typeId);
            item.Reset();
            pool.Add(item);
        }

        private ConcurrentBag<Poolable> GetPool(int typeId)
        {
            var poolExists = _pools.TryGetValue(typeId, out var pool);
            if (!poolExists)
            {
                pool = new ConcurrentBag<Poolable>();
                _pools.Add(typeId, pool);
            }
            return pool;
        }
    }
}

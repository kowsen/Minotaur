using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace Minotaur
{
    public abstract class Component
    {
        public Component()
        {
            Reset();
        }

        public Action Recycle;

        public abstract void Reset();
    }

    public static class ComponentPool<TComponent> where TComponent : Component, new()
    {
        private static ConcurrentBag<TComponent> pool = new ConcurrentBag<TComponent>();

        public static TComponent Get()
        {
            TComponent component;
            var didTake = pool.TryTake(out component);
            if (!didTake)
            {
                component = new TComponent();
                component.Recycle = () =>
                {
                    Recycle(component);
                };
            }
            return component;
        }

        public static void Recycle(TComponent component)
        {
            component.Reset();
            pool.Add(component);
        }
    }
}

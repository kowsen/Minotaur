using System;
using System.Collections.Generic;

namespace Minotaur
{
    public interface Entity : IEquatable<Entity>
    {
        int Id { get; }
        TComponent AddComponent<TComponent>() where TComponent : Component, new();
        void RemoveComponent<TComponent>() where TComponent : Component;
        bool HasComponent<TComponent>() where TComponent : Component;
        TComponent GetComponent<TComponent>() where TComponent : Component;
        void Delete();
    }

    internal class BackingEntity : Poolable, Entity
    {
        public int Id { get; private set; }

        private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

        private Queue<Component> _componentAddQueue = new Queue<Component>();
        private Queue<Type> _componentRemoveQueue = new Queue<Type>();
        private bool _markedForDelete = false;

        public void Init(int id)
        {
            Id = id;
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component, new()
        {
            var component = Pool.Get<TComponent>();
            _componentAddQueue.Enqueue(component);
            return component;
        }

        public void RemoveComponent<TComponent>() where TComponent : Component
        {
            _componentRemoveQueue.Enqueue(typeof(TComponent));
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component
        {
            var success = _components.TryGetValue(typeof(TComponent), out var component);
            if (!success)
            {
                throw new Exception(
                    $"Trying to get nonexistent component of type {typeof(TComponent)} from entity with id {Id}"
                );
            }

            return component as TComponent;
        }

        public bool HasComponent<TComponent>() where TComponent : Component
        {
            return HasComponent(typeof(TComponent));
        }

        public bool HasComponent(Type type)
        {
            return _components.ContainsKey(type);
        }

        public void Delete()
        {
            _markedForDelete = true;
        }

        public bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override void Reset()
        {
            Id = -1;
            _markedForDelete = false;
        }

        public bool CommitComponentChanges()
        {
            bool didChange = false;

            while (_componentAddQueue.Count > 0)
            {
                var component = _componentAddQueue.Dequeue();
                if (_components.ContainsKey(component.GetType()))
                {
                    throw new Exception(
                        $"Inserting duplicate component of type {component.GetType()} into entity with id {Id}"
                    );
                }
                _components.Add(component.GetType(), component);

                didChange = true;
            }

            while (_componentRemoveQueue.Count > 0)
            {
                var type = _componentRemoveQueue.Dequeue();
                var success = _components.TryGetValue(type, out var component);
                if (success)
                {
                    component.Recycle();
                    _components.Remove(type);
                }

                didChange = true;
            }

            return didChange;
        }

        public bool CommitDeleteChanges()
        {
            if (_markedForDelete)
            {
                foreach (var component in _components.Values)
                {
                    component.Recycle();
                }
                _components.Clear();
                _componentAddQueue.Clear();
                _componentRemoveQueue.Clear();

                return true;
            }

            return false;
        }
    }
}

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

        private Dictionary<int, Component> _components = new Dictionary<int, Component>();

        private Queue<ComponentAddInfo> _componentAddQueue = new Queue<ComponentAddInfo>();
        private Queue<int> _componentRemoveQueue = new Queue<int>();
        private bool _markedForDelete = false;
        private Pool _pool;

        public void Init(int id, Pool pool)
        {
            Id = id;
            _pool = pool;
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component, new()
        {
            var component = _pool.Get<TComponent>();
            _componentAddQueue.Enqueue(new ComponentAddInfo(component, TypeId<TComponent>.Get()));
            return component;
        }

        public void RemoveComponent<TComponent>() where TComponent : Component
        {
            _componentRemoveQueue.Enqueue(TypeId<TComponent>.Get());
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component
        {
            var success = _components.TryGetValue(TypeId<TComponent>.Get(), out var component);
            if (!success)
            {
                throw new Exception(
                    $"Trying to get nonexistent component of type {TypeId<TComponent>.Get()} from entity with id {Id}"
                );
            }

            return (TComponent)component;
        }

        public bool HasComponent<TComponent>() where TComponent : Component
        {
            return HasComponent(TypeId<TComponent>.Get());
        }

        public bool HasComponent(int typeId)
        {
            return _components.ContainsKey(typeId);
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
                var addInfo = _componentAddQueue.Dequeue();
                if (_components.ContainsKey(addInfo.TypeId))
                {
                    throw new Exception(
                        $"Inserting duplicate component of type {addInfo.TypeId} into entity with id {Id}"
                    );
                }
                _components.Add(addInfo.TypeId, addInfo.Component);

                didChange = true;
            }

            while (_componentRemoveQueue.Count > 0)
            {
                var typeId = _componentRemoveQueue.Dequeue();
                var success = _components.TryGetValue(typeId, out var component);
                if (success)
                {
                    _pool.Recycle(typeId, component);
                    _components.Remove(typeId);
                }

                didChange = true;
            }

            return didChange;
        }

        public bool CommitDeleteChanges()
        {
            if (_markedForDelete)
            {
                foreach (var item in _components)
                {
                    _pool.Recycle(item.Key, item.Value);
                }
                _components.Clear();
                _componentAddQueue.Clear();
                _componentRemoveQueue.Clear();

                return true;
            }

            return false;
        }

        private struct ComponentAddInfo
        {
            public Component Component;
            public int TypeId;

            public ComponentAddInfo(Component component, int typeId)
            {
                Component = component;
                TypeId = typeId;
            }
        }
    }
}

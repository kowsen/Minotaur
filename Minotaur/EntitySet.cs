using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Minotaur;

namespace Minotaur
{
    public interface EntitySet : IEnumerable<Entity>
    {
        int Count { get; }
        bool HasEntity(int id);
        Entity GetEntity(int id);
        void RemoveComponentFromAll<TComponent>() where TComponent : Component;
    }

    public class BackingEntitySet : EntitySet
    {
        public int Count
        {
            get => _entities.Count;
        }

        private List<Entity> _entities = new List<Entity>();
        private Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        private Signature _signature;

        public BackingEntitySet(Signature signature)
        {
            _signature = signature;
        }

        public bool HasEntity(int id)
        {
            return _entityMap.ContainsKey(id);
        }

        public Entity GetEntity(int id)
        {
            return _entityMap[id];
        }

        public void RemoveComponentFromAll<TComponent>() where TComponent : Component
        {
            if (!_signature.IsTypeInRequirements(typeof(TComponent)))
            {
                throw new Exception(
                    "Trying to remove non-required component from all in EntitySet"
                );
            }
            foreach (var entity in _entities)
            {
                entity.RemoveComponent<TComponent>();
            }
        }

        public void CheckAndAddOrRemoveEntity(BackingEntity entity)
        {
            if (HasEntity(entity.Id))
            {
                if (!_signature.Check(entity))
                {
                    RemoveEntity(entity);
                }
            }
            else
            {
                if (_signature.Check(entity))
                {
                    _entities.Add(entity);
                    _entityMap.Add(entity.Id, entity);
                }
            }
        }

        public void RemoveEntity(BackingEntity entity)
        {
            var wasInSet = _entityMap.Remove(entity.Id);
            if (wasInSet)
            {
                _entities.Remove(entity);
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

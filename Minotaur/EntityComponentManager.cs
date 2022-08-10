using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, BackingEntity> _entities;
        private Dictionary<Signature, BackingEntitySet> _entitySets;
        private int _nextEntityId = 0;

        public EntityComponentManager()
        {
            _entities = new Dictionary<int, BackingEntity>();
            _entitySets = new Dictionary<Signature, BackingEntitySet>();
        }

        public Entity Create()
        {
            var entity = Pool<BackingEntity>.Get();
            entity.Init(_nextEntityId);
            _nextEntityId += 1;
            _entities.Add(entity.Id, entity);
            return entity;
        }

        public void CommitChanges()
        {
            foreach (var entity in _entities.Values)
            {
                var didComponentsChange = entity.CommitComponentChanges();
                if (didComponentsChange)
                {
                    foreach (var entitySet in _entitySets.Values)
                    {
                        entitySet.CheckAndAddOrRemoveEntity(entity);
                    }
                }

                var isEntityDeleted = entity.CommitDeleteChanges();
                if (isEntityDeleted)
                {
                    foreach (var entitySet in _entitySets.Values)
                    {
                        entitySet.RemoveEntity(entity);
                    }
                    _entities.Remove(entity.Id);
                    Pool<BackingEntity>.Recycle(entity);
                }
            }
        }

        public EntitySet Get(Signature signature)
        {
            var success = _entitySets.TryGetValue(signature, out var entitySet);
            if (!success)
            {
                entitySet = new BackingEntitySet(signature);
                foreach (var entity in _entities.Values)
                {
                    entitySet.CheckAndAddOrRemoveEntity(entity);
                }
                _entitySets[signature] = entitySet;
            }
            return entitySet;
        }

        public Entity GetById(int id)
        {
            return _entities[id];
        }
    }
}

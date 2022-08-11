using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, BackingEntity> _entities = new Dictionary<int, BackingEntity>();
        private Dictionary<Signature, BackingEntitySet> _entitySets =
            new Dictionary<Signature, BackingEntitySet>();
        private int _nextEntityId = 0;

        public Entity Create()
        {
            var entity = Pool.Get<BackingEntity>();
            entity.Init(_nextEntityId);
            _entities.Add(entity.Id, entity);
            _nextEntityId += 1;
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
                    entity.Recycle();
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

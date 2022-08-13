using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, BackingEntity> _entities = new Dictionary<int, BackingEntity>();
        private Dictionary<Signature, BackingEntitySet> _entitySets =
            new Dictionary<Signature, BackingEntitySet>();
        private int _nextEntityId = 0;
        private Pool _pool = new Pool();

        private Queue<BackingEntity> _entitiesToRemoveAfterCommit = new Queue<BackingEntity>();

        public Entity Create()
        {
            var entity = _pool.Get<BackingEntity>();
            entity.Init(_nextEntityId, _pool);
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
                    // We can't modify the entity map while iterating over it, so we build
                    // a queue of entities to remove instead.
                    _entitiesToRemoveAfterCommit.Enqueue(entity);
                }
            }

            while (_entitiesToRemoveAfterCommit.Count > 0)
            {
                var entity = _entitiesToRemoveAfterCommit.Dequeue();
                _entities.Remove(entity.Id);
                _pool.Recycle(entity);
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

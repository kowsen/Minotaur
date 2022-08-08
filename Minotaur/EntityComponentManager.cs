using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, Entity> _entities;
        private Dictionary<int, Dictionary<Type, Component>> _entityComponentMap;
        private Dictionary<BitSet, EntitySet> _entitySets;

        private List<Tuple<int, Component, Type>> _addQueue;
        private List<Tuple<int, Type>> _removalQueue;
        private List<int> _entityDeleteQueue;

        private EntityFactory _entityFactory;

        public EntityComponentManager()
        {
            _entities = new Dictionary<int, Entity>();
            _entityComponentMap = new Dictionary<int, Dictionary<Type, Component>>();
            _entitySets = new Dictionary<BitSet, EntitySet>();

            _addQueue = new List<Tuple<int, Component, Type>>();
            _removalQueue = new List<Tuple<int, Type>>();
            _entityDeleteQueue = new List<int>();

            _entityFactory = new EntityFactory(this);
        }

        public Entity Create()
        {
            return _entityFactory.Create();
        }

        public TComponent AddComponent<TComponent>(int entityId) where TComponent : Component, new()
        {
            var component = Pool<TComponent>.Get();
            _addQueue.Add(new Tuple<int, Component, Type>(entityId, component, typeof(TComponent)));
            return component;
        }

        public TComponent AddComponentImmediately<TComponent>(int entityId)
            where TComponent : Component, new()
        {
            var component = Pool<TComponent>.Get();
            AddComponentWithConcreteType(entityId, component, typeof(TComponent));
            return component;
        }

        private void AddComponentWithConcreteType(int entityId, Component component, Type type)
        {
            if (!_entities.ContainsKey(entityId))
            {
                _entities[entityId] = new Entity(entityId, this);
            }

            var success = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!success)
            {
                components = new Dictionary<Type, Component>();
                _entityComponentMap[entityId] = components;
            }

            if (components.ContainsKey(type))
            {
                throw new Exception(
                    $"Inserting duplicate component into entity with Id {entityId}"
                );
            }

            components[type] = component;

            // update cached entity sets
            var entity = _entities[entityId];
            foreach (var pair in _entitySets)
            {
                var signature = pair.Key;
                var entitySet = pair.Value;
                if (
                    ComponentSignatureManager.IsTypeInSignatureRequirements(signature, type)
                    && DoesEntityMatchSignature(entityId, signature)
                )
                {
                    entitySet.Entities.Add(entity);
                }
                if (
                    ComponentSignatureManager.IsTypeInSignatureRestrictions(signature, type)
                    && entitySet.Entities.Contains(entity)
                    && !DoesEntityMatchSignature(entityId, signature)
                )
                {
                    entitySet.Entities.Remove(entity);
                }
            }
        }

        public void RemoveComponent<TComponent>(int entityId) where TComponent : Component
        {
            var type = typeof(TComponent);
            _removalQueue.Add(new Tuple<int, Type>(entityId, type));
        }

        public void RemoveComponentImmediately<TComponent>(int entityId)
            where TComponent : Component
        {
            RemoveComponentWithConcreteType(entityId, typeof(TComponent));
        }

        public void RemoveComponentWithConcreteType(int entityId, Type type)
        {
            var getEntitySuccess = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!getEntitySuccess)
            {
                throw new Exception(
                    $"Trying to remove component from nonexistant entity with Id {entityId}"
                );
            }

            var getComponentSuccess = components.TryGetValue(type, out var component);
            if (getComponentSuccess)
            {
                Pool.Recycle(type, component);
            }

            components.Remove(type);

            // update cached entity sets
            var entity = _entities[entityId];
            foreach (var pair in _entitySets)
            {
                var signature = pair.Key;
                var entitySet = pair.Value;
                if (
                    ComponentSignatureManager.IsTypeInSignatureRequirements(signature, type)
                    && entitySet.Entities.Contains(entity)
                )
                {
                    entitySet.Entities.Remove(entity);
                }
                if (
                    ComponentSignatureManager.IsTypeInSignatureRestrictions(signature, type)
                    && DoesEntityMatchSignature(entityId, signature)
                )
                {
                    entitySet.Entities.Add(entity);
                }
            }
        }

        public TComponent GetComponent<TComponent>(int entityId) where TComponent : Component
        {
            var success = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!success)
            {
                throw new Exception(
                    $"Trying to get component of type {typeof(TComponent)} from nonexistent entity with Id {entityId}"
                );
            }

            success = components.TryGetValue(typeof(TComponent), out var component);
            if (!success)
            {
                throw new Exception(
                    $"Trying to get nonexistent component of type {typeof(TComponent)} from entity with Id {entityId}"
                );
            }

            return (TComponent)component;
        }

        public bool HasComponent<TComponent>(int entityId)
        {
            var success = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!success)
            {
                throw new Exception(
                    $"Trying to get component of type {typeof(TComponent)} from nonexistent entity with Id {entityId}"
                );
            }

            return components.ContainsKey(typeof(TComponent));
        }

        public void Delete(int entityId)
        {
            _entityDeleteQueue.Add(entityId);
        }

        public void DeleteImmediately(int entityId)
        {
            var getEntitySuccess = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!getEntitySuccess)
            {
                throw new Exception($"Trying to delete nonexistent entity with Id {entityId}");
            }

            foreach (var pair in components)
            {
                Pool.Recycle(pair.Key, pair.Value);
            }

            _entityComponentMap.Remove(entityId);

            var entity = _entities[entityId];
            foreach (var entitySet in _entitySets.Values)
            {
                if (entitySet.Entities.Contains(entity))
                {
                    entitySet.Entities.Remove(entity);
                }
            }
            _entities.Remove(entityId);
        }

        public void CommitComponentChanges()
        {
            for (var i = 0; i < _addQueue.Count; i++)
            {
                AddComponentWithConcreteType(
                    _addQueue[i].Item1,
                    _addQueue[i].Item2,
                    _addQueue[i].Item3
                );
            }
            for (var i = 0; i < _removalQueue.Count; i++)
            {
                RemoveComponentWithConcreteType(_removalQueue[i].Item1, _removalQueue[i].Item2);
            }
            for (var i = 0; i < _entityDeleteQueue.Count; i++)
            {
                DeleteImmediately(_entityDeleteQueue[i]);
            }
            _addQueue.Clear();
            _removalQueue.Clear();
            _entityDeleteQueue.Clear();
        }

        public EntitySet Get(BitSet signature)
        {
            var success = _entitySets.TryGetValue(signature, out var entitySet);
            if (!success)
            {
                entitySet = new EntitySet(signature, this);
                foreach (var entityId in _entityComponentMap.Keys)
                {
                    if (DoesEntityMatchSignature(entityId, signature))
                    {
                        entitySet.Entities.Add(_entities[entityId]);
                    }
                }
                _entitySets[signature] = entitySet;
            }
            return entitySet;
        }

        public Entity GetById(int id)
        {
            var success = _entities.TryGetValue(id, out var entity);
            if (success)
            {
                return entity;
            }
            else
            {
                return Entity.Default;
            }
        }

        public bool IsValid(int id)
        {
            return _entities.ContainsKey(id);
        }

        private bool DoesEntityMatchSignature(int entityId, BitSet signature)
        {
            var components = _entityComponentMap[entityId];
            var componentTypes = new List<Type>(components.Keys);
            return ComponentSignatureManager.CheckAgainstComponentSignature(
                signature,
                componentTypes
            );
        }
    }
}

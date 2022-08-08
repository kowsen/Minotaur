using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, Entity> _entities;
        private Dictionary<int, Dictionary<Type, IComponent>> _entityComponentMap;
        private Dictionary<BitSet, EntitySet> _entitySets;

        private List<Tuple<int, IComponent, Type>> _addQueue;
        private List<Tuple<int, Type>> _removalQueue;
        private List<int> _entityDeleteQueue;

        private EntityFactory _entityFactory;

        public EntityComponentManager()
        {
            _entities = new Dictionary<int, Entity>();
            _entityComponentMap = new Dictionary<int, Dictionary<Type, IComponent>>();
            _entitySets = new Dictionary<BitSet, EntitySet>();

            _addQueue = new List<Tuple<int, IComponent, Type>>();
            _removalQueue = new List<Tuple<int, Type>>();
            _entityDeleteQueue = new List<int>();

            _entityFactory = new EntityFactory(this);
        }

        public Entity Create()
        {
            return _entityFactory.Create();
        }

        public void AddComponent<TComponent>(int entityId, TComponent component)
            where TComponent : IComponent
        {
            _addQueue.Add(
                new Tuple<int, IComponent, Type>(entityId, component, typeof(TComponent))
            );
        }

        public void AddComponentImmediately<TComponent>(int entityId, TComponent component)
            where TComponent : IComponent
        {
            AddComponentWithConcreteType(entityId, component, typeof(TComponent));
        }

        private void AddComponentWithConcreteType(int entityId, IComponent component, Type type)
        {
            if (!_entities.ContainsKey(entityId))
            {
                _entities[entityId] = new Entity(entityId, this);
            }

            var success = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!success)
            {
                components = new Dictionary<Type, IComponent>();
                _entityComponentMap[entityId] = components;
            }

            if (components.ContainsKey(type))
            {
                throw new Exception(
                    $"Inserting duplicate component into entity with Id {entityId}"
                );
            }

            component.Attach(entityId);
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

        public void RemoveComponent<TComponent>(int entityId) where TComponent : IComponent
        {
            var type = typeof(TComponent);
            _removalQueue.Add(new Tuple<int, Type>(entityId, type));
        }

        public void RemoveComponentImmediately<TComponent>(int entityId)
            where TComponent : IComponent
        {
            RemoveComponentWithConcreteType(entityId, typeof(TComponent));
        }

        public void RemoveComponentWithConcreteType(int entityId, Type type)
        {
            var success = _entityComponentMap.TryGetValue(entityId, out var components);
            if (!success)
            {
                throw new Exception(
                    $"Trying to remove component from nonexistant entity with Id {entityId}"
                );
            }

            components.Remove(type);

            // update cached entity sets
            var entity = _entities[entityId];
            foreach (var pair in _entitySets)
            {
                var signature = pair.Key;
                var entities = pair.Value;
                if (
                    ComponentSignatureManager.IsTypeInSignatureRequirements(signature, type)
                    && entities.Entities.Contains(entity)
                )
                {
                    entities.Entities.Remove(entity);
                }
                if (
                    ComponentSignatureManager.IsTypeInSignatureRestrictions(signature, type)
                    && DoesEntityMatchSignature(entityId, signature)
                )
                {
                    entities.Entities.Add(entity);
                }
            }
        }

        public TComponent GetComponent<TComponent>(int entityId) where TComponent : IComponent
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
            var success = _entityComponentMap.Remove(entityId);
            if (!success)
            {
                throw new Exception($"Trying to delete nonexistent entity with Id {entityId}");
            }

            var entity = _entities[entityId];
            foreach (var pair in _entitySets)
            {
                var entities = pair.Value;
                if (entities.Entities.Contains(entity))
                {
                    entities.Entities.Remove(entity);
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

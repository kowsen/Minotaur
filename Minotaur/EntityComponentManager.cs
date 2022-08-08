using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, Entity> _entities;
        private Dictionary<int, BackingEntity> _entityComponentMap;
        private Dictionary<BitSet, EntitySet> _entitySets;

        private List<ComponentAddQueueItem> _addQueue;
        private List<ComponentRemoveQueueItem> _removalQueue;
        private List<int> _entityDeleteQueue;

        private EntityFactory _entityFactory;

        public EntityComponentManager()
        {
            _entities = new Dictionary<int, Entity>();
            _entityComponentMap = new Dictionary<int, BackingEntity>();
            _entitySets = new Dictionary<BitSet, EntitySet>();

            _addQueue = new List<ComponentAddQueueItem>();
            _removalQueue = new List<ComponentRemoveQueueItem>();
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
            _addQueue.Add(new ComponentAddQueueItem(entityId, component));
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
                components = Pool<BackingEntity>.Get();
                _entityComponentMap[entityId] = components;
            }

            if (components.HasComponent(type))
            {
                throw new Exception(
                    $"Inserting duplicate component into entity with Id {entityId}"
                );
            }

            components.AddComponent(type, component);

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
            _removalQueue.Add(new ComponentRemoveQueueItem(entityId, type));
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

            components.RemoveComponent(type);

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

            return (TComponent)components.GetComponent(typeof(TComponent));
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

            return components.HasComponent(typeof(TComponent));
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

            components.Recycle();
        }

        public void CommitComponentChanges()
        {
            foreach (var itemToAdd in _addQueue)
            {
                AddComponentWithConcreteType(
                    itemToAdd.EntityId,
                    itemToAdd.Component,
                    itemToAdd.Component.GetType()
                );
            }

            foreach (var itemToRemove in _removalQueue)
            {
                RemoveComponentWithConcreteType(itemToRemove.EntityId, itemToRemove.ComponentType);
            }

            foreach (var entityToDelete in _entityDeleteQueue)
            {
                DeleteImmediately(entityToDelete);
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
            return ComponentSignatureManager.CheckAgainstComponentSignature(signature, components);
        }
    }

    public class BackingEntity : Poolable
    {
        private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

        public void AddComponent(Type type, Component component)
        {
            if (_components.ContainsKey(type))
            {
                throw new Exception($"Inserting duplicate component of type {type} into entity");
            }
            _components.Add(type, component);
        }

        public void RemoveComponent(Type type)
        {
            var component = GetComponent(type, true);
            if (component != null)
            {
                Pool.Recycle(type, component);
            }

            _components.Remove(type);
        }

        public Component GetComponent(Type type, bool ignoreMissing = false)
        {
            var success = _components.TryGetValue(type, out var component);
            if (!success && !ignoreMissing)
            {
                throw new Exception(
                    $"Trying to get nonexistent component of type {type} from entity"
                );
            }

            return component;
        }

        public bool HasComponent(Type type)
        {
            return _components.ContainsKey(type);
        }

        public void Recycle()
        {
            foreach (var pair in _components)
            {
                Pool.Recycle(pair.Key, pair.Value);
            }
            Pool<BackingEntity>.Recycle(this);
        }

        public override void Reset()
        {
            _components.Clear();
        }
    }

    struct ComponentAddQueueItem
    {
        public int EntityId;
        public Component Component;

        public ComponentAddQueueItem(int entityId, Component component)
        {
            EntityId = entityId;
            Component = component;
        }
    }

    struct ComponentRemoveQueueItem
    {
        public int EntityId;
        public Type ComponentType;

        public ComponentRemoveQueueItem(int entityId, Type componentType)
        {
            EntityId = entityId;
            ComponentType = componentType;
        }
    }
}

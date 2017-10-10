using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, Entity> _entities;
        private Dictionary<int, Dictionary<Type, IComponent>> _entityComponentMap;
        private Dictionary<BitSet, EntitySet> _matchers;

        private List<Tuple<int, IComponent, Type>> _addQueue;
        private List<Tuple<int, Type>> _removalQueue;

        private EntityFactory _entityFactory;

        public EntityComponentManager()
        {
            _entities = new Dictionary<int, Entity>();
            _entityComponentMap = new Dictionary<int, Dictionary<Type, IComponent>>();
            _matchers = new Dictionary<BitSet, EntitySet>();

            _addQueue = new List<Tuple<int, IComponent, Type>>();
            _removalQueue = new List<Tuple<int, Type>>();

            _entityFactory = new EntityFactory(this);
        }

        public Entity CreateEntity()
        {
            return _entityFactory.Create();
        }

        public void AddComponentOnNextTick<T>(int entityId, T component) where T : IComponent
        {
            _addQueue.Add(new Tuple<int, IComponent, Type>(entityId, component, typeof(T)));
        }

        public void AddComponent<T>(int entityId, T component) where T : IComponent
        {
            AddComponentWithConcreteType(entityId, component, typeof(T));
        }

        private void AddComponentWithConcreteType(int entityId, IComponent component, Type type)
        {
            if (!_entities.ContainsKey(entityId))
            {
                _entities[entityId] = new Entity(entityId, this);
            }

            Dictionary<Type, IComponent> components;
            var success = _entityComponentMap.TryGetValue(entityId, out components);
            if (!success)
            {
                components = new Dictionary<Type, IComponent>();
                _entityComponentMap[entityId] = components;
            }

            if (components.ContainsKey(type))
            {
                throw new Exception($"Inserting duplicate component into entity with Id {entityId}");
            }

            components[type] = component;

            // update matchers
            var entity = _entities[entityId];
            foreach (var pair in _matchers)
            {
                var signature = pair.Key;
                var entities = pair.Value;
                if (ComponentSignatureManager.IsTypeInSignatureRequirements(signature, type) && DoesEntityMatchSignature(entityId, signature))
                {
                    entities.Entities.Add(entity);
                }
                if (ComponentSignatureManager.IsTypeInSignatureRestrictions(signature, type) && entities.Entities.Contains(entity) && !DoesEntityMatchSignature(entityId, signature))
                {
                    entities.Entities.Remove(entity);
                }
            }
        }

        public void RemoveComponentOnNextTick(int entityId, Type type)
        {
            _removalQueue.Add(new Tuple<int, Type>(entityId, type));
        }

        public void RemoveComponent(int entityId, Type type)
        {
            Dictionary<Type, IComponent> components;
            var success = _entityComponentMap.TryGetValue(entityId, out components);
            if (!success)
            {
                throw new Exception($"Trying to remove component from nonexistant entity with Id {entityId}");
            }

            components.Remove(type);

            // update matchers
            var entity = _entities[entityId];
            foreach (var pair in _matchers)
            {
                var signature = pair.Key;
                var entities = pair.Value;
                if (ComponentSignatureManager.IsTypeInSignatureRequirements(signature, type) && entities.Entities.Contains(entity))
                {
                    entities.Entities.Remove(entity);
                }
                if (ComponentSignatureManager.IsTypeInSignatureRestrictions(signature, type) && DoesEntityMatchSignature(entityId, signature))
                {
                    entities.Entities.Add(entity);
                }
            }
        }

        public T GetComponent<T>(int entityId)
        {
            Dictionary<Type, IComponent> components;
            var success = _entityComponentMap.TryGetValue(entityId, out components);
            if (!success)
            {
                throw new Exception($"Trying to get component of type {typeof(T)} from nonexistent entity with Id {entityId}");
            }

            IComponent component;
            success = components.TryGetValue(typeof(T), out component);
            if (!success)
            {
                throw new Exception($"Trying to get nonexistent component of type {typeof(T)} from entity with Id {entityId}");
            }

            return (T)component;
        }

        public void ProcessAddRemovalQueue()
        {
            for (var i = 0; i < _addQueue.Count; i++)
            {
                AddComponentWithConcreteType(_addQueue[i].Item1, _addQueue[i].Item2, _addQueue[i].Item3);
            }
            for (var i = 0; i < _removalQueue.Count; i++)
            {
                RemoveComponent(_removalQueue[i].Item1, _removalQueue[i].Item2);
            }
            _addQueue.Clear();
            _removalQueue.Clear();
        }

        public BitSet GetSignature(List<Type> typeRequirements, List<Type> typeRestrictions)
        {
            return ComponentSignatureManager.GenerateComponentSignature(typeRequirements, typeRestrictions);
        }

        public EntitySet GetEntities(BitSet signature)
        {
            EntitySet entities;
            var success = _matchers.TryGetValue(signature, out entities);
            if (!success)
            {
                if (!ComponentSignatureManager.ValidateSignature(signature))
                {
                    throw new Exception("Trying to fetch with invalid signature (including and excluding the same component)");
                }
                entities = new EntitySet(signature, this);
                foreach (var entityId in _entityComponentMap.Keys)
                {
                    if (DoesEntityMatchSignature(entityId, signature))
                    {
                        entities.Entities.Add(_entities[entityId]);
                    }
                }
                _matchers[signature] = entities;
            }
            return entities;
        }

        private bool DoesEntityMatchSignature(int entityId, BitSet signature)
        {
            var components = _entityComponentMap[entityId];
            var componentTypes = new List<Type>(components.Keys);
            return ComponentSignatureManager.CheckAgainstComponentSignature(signature, componentTypes);
        }
    }
}

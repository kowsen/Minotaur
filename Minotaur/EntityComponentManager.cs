using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class EntityComponentManager
    {
        private Dictionary<int, Entity> _entities;
        private Dictionary<int, Dictionary<Type, IComponent>> _entityComponentMap;
        private Dictionary<int, List<Entity>> _matchers;

        private EntityFactory _entityFactory;
        private ComponentSignatureManager _signatureManager;

        public EntityComponentManager()
        {
            _entities = new Dictionary<int, Entity>();
            _entityComponentMap = new Dictionary<int, Dictionary<Type, IComponent>>();
            _matchers = new Dictionary<int, List<Entity>>();

            _entityFactory = new EntityFactory(this);
            _signatureManager = new ComponentSignatureManager();
        }

        public Entity CreateEntity()
        {
            return _entityFactory.Create();
        }

        public void AddComponent<T>(int entityId, T component) where T: IComponent
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

            var type = typeof(T);
            if (components.ContainsKey(type))
            {
                throw new Exception($"Inserting duplicate component into entity with Id {entityId}");
            }

            components[type] = component;

            // update matchers
            foreach (var pair in _matchers)
            {
                var signature = pair.Key;
                var entities = pair.Value;
                if (_signatureManager.IsTypeInSignature(signature, typeof(T)) && DoesEntityMatchSignature(entityId, signature))
                {
                    entities.Add(_entities[entityId]);
                }
            }
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
                if (_signatureManager.IsTypeInSignature(signature, type) && entities.Contains(entity))
                {
                    entities.Remove(entity);
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

        public int GetSignature(List<Type> types)
        {
            return _signatureManager.GenerateComponentSignature(types);
        }

        public List<Entity> GetEntities(int signature)
        {
            List<Entity> entities;
            var success = _matchers.TryGetValue(signature, out entities);
            if (!success)
            {
                entities = new List<Entity>();
                foreach (var entityId in _entityComponentMap.Keys)
                {
                    if (DoesEntityMatchSignature(entityId, signature))
                    {
                        entities.Add(_entities[entityId]);
                    }
                }
                _matchers[signature] = entities;
            }
            return entities;
        }

        private bool DoesEntityMatchSignature(int entityId, int signature)
        {
            var components = _entityComponentMap[entityId];
            var componentTypes = new List<Type>(components.Keys);
            return _signatureManager.CheckAgainstComponentSignature(signature, componentTypes);
        }
    }
}

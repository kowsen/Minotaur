using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Minotaur
{
    public struct Entity : IEquatable<Entity>
    {
        public static Entity Default = new Entity(-1, null);

        public int Id { get; private set; }
        private EntityComponentManager _ecs;

        public Entity(int id, EntityComponentManager ecs)
        {
            Id = id;
            _ecs = ecs;
        }

        public void AddComponentImmediately<T>(T component) where T : IComponent
        {
            _ecs.AddComponentImmediately(Id, component);
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            _ecs.AddComponent(Id, component);
        }

        public T GetComponent<T>() where T : IComponent
        {
            return _ecs.GetComponent<T>(Id);
        }

        public bool HasComponent<T>()
        {
            return _ecs.HasComponent<T>(Id);
        }

        public void RemoveComponentImmediately<T>() where T : IComponent
        {
            _ecs.RemoveComponentImmediately<T>(Id);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            _ecs.RemoveComponent<T>(Id);
        }

        public void DeleteImmediately()
        {
            _ecs.DeleteEntityImmediately(Id);
        }

        public void Delete()
        {
            _ecs.DeleteEntity(Id);
        }

        public bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

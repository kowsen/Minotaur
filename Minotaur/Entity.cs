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

        public TComponent AddComponentImmediately<TComponent>() where TComponent : Component, new()
        {
            return _ecs.AddComponentImmediately<TComponent>(Id);
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component, new()
        {
            return _ecs.AddComponent<TComponent>(Id);
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component
        {
            return _ecs.GetComponent<TComponent>(Id);
        }

        public bool HasComponent<TComponent>()
        {
            return _ecs.HasComponent<TComponent>(Id);
        }

        public void RemoveComponentImmediately<TComponent>() where TComponent : Component, new()
        {
            _ecs.RemoveComponentImmediately<TComponent>(Id);
        }

        public void RemoveComponent<TComponent>() where TComponent : Component
        {
            _ecs.RemoveComponent<TComponent>(Id);
        }

        public void DeleteImmediately()
        {
            _ecs.DeleteImmediately(Id);
        }

        public void Delete()
        {
            _ecs.Delete(Id);
        }

        public bool IsValid()
        {
            return _ecs.IsValid(Id);
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

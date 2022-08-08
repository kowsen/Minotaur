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

        public void AddComponentImmediately<TComponent>(TComponent component)
            where TComponent : IComponent
        {
            _ecs.AddComponentImmediately(Id, component);
        }

        public void AddComponent<TComponent>(TComponent component, bool ignoreIfExists = false)
            where TComponent : IComponent
        {
            if (ignoreIfExists && HasComponent<TComponent>())
            {
                return;
            }
            _ecs.AddComponent(Id, component);
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            return _ecs.GetComponent<TComponent>(Id);
        }

        public bool HasComponent<TComponent>()
        {
            return _ecs.HasComponent<TComponent>(Id);
        }

        public void RemoveComponentImmediately<TComponent>() where TComponent : IComponent
        {
            _ecs.RemoveComponentImmediately<TComponent>(Id);
        }

        public void RemoveComponent<TComponent>(bool ignoreIfDoesntExist = false)
            where TComponent : IComponent
        {
            if (ignoreIfDoesntExist && !HasComponent<TComponent>())
            {
                return;
            }
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

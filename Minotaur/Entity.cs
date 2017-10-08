using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Entity : IEquatable<Entity>
    {
        public int Id { get; private set; }
        private EntityComponentManager _ecs;

        public Entity(int id, EntityComponentManager ecs)
        {
            Id = id;
            _ecs = ecs;
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            _ecs.AddComponent(Id, component);
        }

        public T GetComponent<T>()
        {
            return _ecs.GetComponent<T>(Id);
        }

        public void RemoveComponent(Type type)
        {
            _ecs.RemoveComponent(Id, type);
        }

        public bool Equals(Entity other)
        {
            return Id == other.Id;
        }
    }
}

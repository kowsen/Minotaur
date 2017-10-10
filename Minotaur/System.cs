using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem
    {
        public BitSet Signature;
        public List<Type> TypeRequirements = new List<Type>();
        public List<Type> TypeRestrictions = new List<Type>();

        protected void AddComponentRequirement<T>() where T : IComponent
        {
            TypeRequirements.Add(typeof(T));
        }

        protected void AddComponentRestriction<T>() where T : IComponent
        {
            TypeRestrictions.Add(typeof(T));
        }

        public virtual void Update(TimeSpan time, Entity entity)
        {

        }

        public virtual void Update(TimeSpan time, EntitySet entities)
        {

        }

        public virtual void Draw(TimeSpan time, Entity entity)
        {

        }

        public virtual void Draw(TimeSpan time, EntitySet entities)
        {

        }
    }

    public abstract class GameSystem
    {
        public virtual void Update(TimeSpan time)
        {

        }

        public virtual void Draw(TimeSpan time)
        {

        }
    }
}

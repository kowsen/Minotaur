using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class GameSystem
    {
        public int Signature;
        public List<Type> Types = new List<Type>();

        protected void AddComponentConstraint<T>() where T : IComponent
        {
            Types.Add(typeof(T));
        }

        public virtual void Update(TimeSpan time, Entity entity)
        {

        }

        public virtual void Update(TimeSpan time, List<Entity> entities)
        {

        }

        public virtual void Draw(TimeSpan time, Entity entity)
        {

        }

        public virtual void Draw(TimeSpan time, List<Entity> entities)
        {

        }
    }
}

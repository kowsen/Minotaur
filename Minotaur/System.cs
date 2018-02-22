using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<T>
    {
        public BitSet Signature;
        protected T Game;
        protected EntityComponentManager Entities;
        protected ErrandManager<T> Errands;

        public void Attach(T game, EntityComponentManager ecm, ErrandManager<T> errands)
        {
            Game = game;
            Entities = ecm;
            Errands = errands;
        }

        public virtual void Initialize() { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }

        public virtual void Update(TimeSpan time, Entity entity) { }
        public virtual void Update(TimeSpan time, EntitySet entities) { }
        public virtual void Draw(TimeSpan time, Entity entity) { }
        public virtual void Draw(TimeSpan time, EntitySet entities) { }
    }

    public abstract class GameSystem<T>
    {
        protected T Game;
        protected EntityComponentManager Entities;
        protected ErrandManager<T> Errands;

        public void Attach(T game, EntityComponentManager ecm, ErrandManager<T> errands)
        {
            Game = game;
            Entities = ecm;
            Errands = errands;
        }

        public virtual void Initialize() { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }

        public virtual void Update(TimeSpan time) { }
        public virtual void Draw(TimeSpan time) { }
    }
}

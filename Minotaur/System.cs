using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<T>
    {
        protected List<Type> _requirements = new List<Type>();
        protected List<Type> _restrictions = new List<Type>();

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

        protected void SetRequirements(params Type[] types)
        {
            _requirements = new List<Type>(types);
            UpdateSignature();
        }

        protected void setRestrictions(params Type[] types)
        {
            _restrictions = new List<Type>(types);
            UpdateSignature();
        }

        private void UpdateSignature()
        {
            Signature = ComponentSignatureManager.GenerateComponentSignature(_requirements, _restrictions);
        }
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<TGame>
    {
        protected List<Type> _requirements = new List<Type>();
        protected List<Type> _restrictions = new List<Type>();

        public BitSet Signature;
        protected TGame _game;
        protected EntityComponentManager _entities;
        protected ErrandManager<TGame> _errands;

        public void Attach(TGame game, EntityComponentManager ecm, ErrandManager<TGame> errands)
        {
            _game = game;
            _entities = ecm;
            _errands = errands;
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
            Signature = ComponentSignatureManager.GenerateComponentSignature(
                _requirements,
                _restrictions
            );
        }
    }

    public abstract class GameSystem<TGame>
    {
        protected TGame Game;
        protected EntityComponentManager Entities;
        protected ErrandManager<TGame> Errands;

        public void Attach(TGame game, EntityComponentManager ecm, ErrandManager<TGame> errands)
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

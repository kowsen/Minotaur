using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<T>
    {
        public BitSet Signature;
        protected T _game;
        private EntityComponentManager _ecm;

        public void Attach(T game, EntityComponentManager ecm)
        {
            _game = game;
            _ecm = ecm;
        }

        public virtual void Initialize() { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        protected Entity CreateEntity()
        {
            return _ecm.CreateEntity();
        }
        protected EntitySet GetEntities()
        {
            return _ecm.GetEntities(Signature);
        }
        public virtual void Update(TimeSpan time, Entity entity) { }
        public virtual void Update(TimeSpan time, EntitySet entities) { }
        public virtual void Draw(TimeSpan time, Entity entity) { }
        public virtual void Draw(TimeSpan time, EntitySet entities) { }
    }

    public abstract class GameSystem<T>
    {
        public BitSet Signature;
        protected T _game;
        private EntityComponentManager _ecm;

        public void Attach(T game, EntityComponentManager ecm)
        {
            _game = game;
            _ecm = ecm;
        }

        public virtual void Initialize() { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public EntitySet GetEntities()
        {
            return _ecm.GetEntities(Signature);
        }
        public virtual void Update(TimeSpan time) { }
        public virtual void Draw(TimeSpan time) { }
    }
}

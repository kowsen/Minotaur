using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<TGame>
    {
        public Signature Signature = new Signature();
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<TGame> : System<TGame>
    {
        public Signature Signature = new Signature();

        public virtual void Update(TimeSpan time, Entity entity) { }

        public virtual void Update(TimeSpan time, EntitySet entities) { }

        public virtual void Draw(TimeSpan time, Entity entity) { }

        public virtual void Draw(TimeSpan time, EntitySet entities) { }
    }

    public abstract class GameSystem<TGame> : System<TGame>
    {
        public virtual void Update(TimeSpan time) { }

        public virtual void Draw(TimeSpan time) { }
    }

    public abstract class System<TGame>
    {
        public TGame Game { get; private set; }
        public EntityComponentManager Entities { get; private set; }

        public void Attach(TGame game, EntityComponentManager entities)
        {
            Game = game;
            Entities = entities;
        }

        public virtual void Initialize() { }

        public virtual void OnActivate() { }

        public virtual void OnDeactivate() { }
    }
}

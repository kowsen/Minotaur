using System;

namespace Minotaur
{
    public abstract class EntitySystem<TGame> : System<TGame>
    {
        internal Signature Signature = new Signature();

        public virtual void Update(TimeSpan time, EntitySet entities) { }

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

        internal void Attach(TGame game, EntityComponentManager entities)
        {
            Game = game;
            Entities = entities;
        }

        public abstract void Initialize();

        public abstract void Cleanup();
    }
}

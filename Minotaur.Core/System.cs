using System;
using System.Collections.Generic;

namespace Minotaur
{
    public abstract class System<TGame>
    {
        private static Signature _emptySignature = new Signature();

        public TGame Game { get; private set; }
        public EntityComponentManager Entities { get; private set; }

        internal Signature Signature = _emptySignature;

        internal void Attach(TGame game, EntityComponentManager entities)
        {
            Game = game;
            Entities = entities;
        }

        public abstract void Initialize();

        public abstract void Cleanup();

        public virtual void Update(TimeSpan time, EntitySet entities) { }

        public virtual void Draw(TimeSpan time, EntitySet entities) { }
    }
}

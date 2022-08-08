using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class Errand<TGame> : Poolable
    {
        public DrawStatus DrawOrder { get; protected set; } = DrawStatus.NONE;
        protected ErrandManager<TGame> Errands;
        protected EntityComponentManager Entities;
        protected TGame Game;

        public void Attach(
            ErrandManager<TGame> errands,
            EntityComponentManager entities,
            TGame game
        )
        {
            Errands = errands;
            Entities = entities;
            Game = game;
        }

        public virtual void Update(TimeSpan time) { }

        public virtual void Draw(TimeSpan time) { }

        public virtual void OnEnd(bool isCancelled) { }

        public void End(bool isCancelled = false)
        {
            OnEnd(isCancelled);
            Errands.Remove(this, GetType());
        }

        public enum DrawStatus
        {
            NONE,
            BEFORE,
            AFTER
        }
    }
}

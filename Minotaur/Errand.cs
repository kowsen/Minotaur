using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class Errand<TGame> : Poolable
    {
        public DrawStatus DrawOrder { get; protected set; } = DrawStatus.NONE;
        protected ErrandManager<TGame> _errands;
        protected EntityComponentManager _entities;
        protected TGame _game;

        public void Attach(
            ErrandManager<TGame> errands,
            EntityComponentManager entities,
            TGame game
        )
        {
            _errands = errands;
            _entities = entities;
            _game = game;
        }

        public virtual void Update(TimeSpan time) { }

        public virtual void Draw(TimeSpan time) { }

        public virtual void OnEnd(bool isCancelled) { }

        public void End(bool isCancelled = false)
        {
            OnEnd(isCancelled);
            _errands.Remove(this, GetType());
        }

        public enum DrawStatus
        {
            NONE,
            BEFORE,
            AFTER
        }
    }
}

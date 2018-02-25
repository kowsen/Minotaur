using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class Errand<T>
    {
        public DrawStatus DrawOrder { get; protected set; } = DrawStatus.NONE;
        protected ErrandManager<T> Errands;
        protected T Game;

        public void Attach(ErrandManager<T> errands, T game)
        {
            Errands = errands;
            Game = game;
            OnCreate();
        }

        public virtual void OnCreate() { }
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

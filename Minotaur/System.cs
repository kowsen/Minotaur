using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class EntitySystem<T>
    {
        public BitSet Signature;
        protected T Game;
        protected EntityComponentManager Entities;

        public void Attach(T game, EntityComponentManager ecm)
        {
            Game = game;
            Entities = ecm;
        }

        public virtual void Initialize() { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        protected U RunErrand<U>() where U : Errand<T>
        {
            return Errand<T>.Run<U>(Game);
        }
        protected void RunErrandSpawner<U>(U errandSpawner) where U : ErrandSpawner<T>
        {
            ErrandSpawner<T>.Run(errandSpawner, Game);
        }
        protected bool IsErrandRunning<U>() where U : Errand<T>
        {
            return Errand<T>.IsRunning<U>();
        }
        protected void CancelErrand<U>() where U : Errand<T>
        {
            Errand<T>.Cancel<U>();
        }
        public virtual void Update(TimeSpan time, Entity entity) { }
        public virtual void Update(TimeSpan time, EntitySet entities) { }
        public virtual void Draw(TimeSpan time, Entity entity) { }
        public virtual void Draw(TimeSpan time, EntitySet entities) { }
    }

    public abstract class GameSystem<T>
    {
        protected T Game;
        protected EntityComponentManager Entities;

        public void Attach(T game, EntityComponentManager ecm)
        {
            Game = game;
            Entities = ecm;
        }

        public virtual void Initialize() { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        protected U RunErrand<U>() where U : Errand<T>
        {
            return Errand<T>.Run<U>(Game);
        }
        protected void RunErrandSpawner<U>(U errandSpawner) where U : ErrandSpawner<T>
        {
            ErrandSpawner<T>.Run(errandSpawner, Game);
        }
        protected bool IsErrandRunning<U>() where U : Errand<T>
        {
            return Errand<T>.IsRunning<U>();
        }
        protected void CancelErrand<U>() where U : Errand<T>
        {
            Errand<T>.Cancel<U>();
        }
        public virtual void Update(TimeSpan time) { }
        public virtual void Draw(TimeSpan time) { }
    }
}

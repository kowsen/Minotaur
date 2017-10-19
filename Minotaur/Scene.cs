using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene<T>
    {
        protected EntityComponentManager Entities;
        protected SystemManager<T> Systems;

        protected T Game;

        public virtual void Attach(T game)
        {
            Game = game;
            ResetECS();
        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {

        }

        public void Activate()
        {
            Systems.ActivateSystems();
        }

        public void Deactivate()
        {
            Systems.DeactivateSystems();
        }

        public void Reset()
        {
            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            Entities = new EntityComponentManager();
            Systems = new SystemManager<T>(Entities, Game);
        }

        public void Update(TimeSpan time)
        {
            Systems.Update(time);
        }

        public void Draw(TimeSpan time)
        {
            Systems.Draw(time);
        }
    }
}

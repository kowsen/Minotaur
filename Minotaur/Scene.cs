using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene<T>
    {
        protected EntityComponentManager Entities;
        protected SystemManager<T> Systems;
        protected ErrandManager<T> Errands;

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
            Errands?.Clear();

            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            Entities = new EntityComponentManager();
            Errands = new ErrandManager<T>(Game);
            Systems = new SystemManager<T>(Entities, Errands, Game);
        }

        public void Update(TimeSpan time)
        {
            Systems.Update(time);
            Errands.Update(time);
        }

        public void Draw(TimeSpan time)
        {
            Errands.Draw(time, Errand<T>.DrawStatus.BEFORE);
            Systems.Draw(time);
            Errands.Draw(time, Errand<T>.DrawStatus.AFTER);
        }
    }
}

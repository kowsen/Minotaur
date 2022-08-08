using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene<TGame>
    {
        protected EntityComponentManager Entities;
        protected SystemManager<TGame> Systems;
        protected ErrandManager<TGame> Errands;

        protected TGame Game;

        public virtual void Attach(TGame game)
        {
            Game = game;
            ResetECS();
        }

        public virtual void Initialize() { }

        public virtual void LoadContent() { }

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
            Errands.Clear();
            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            Entities = new EntityComponentManager();
            Errands = new ErrandManager<TGame>(Entities, Game);
            Systems = new SystemManager<TGame>(Entities, Errands, Game);
        }

        public void Update(TimeSpan time)
        {
            Systems.Update(time);
            Errands.Update(time);
        }

        public void Draw(TimeSpan time)
        {
            Errands.Draw(time, Errand<TGame>.DrawStatus.BEFORE);
            Systems.Draw(time);
            Errands.Draw(time, Errand<TGame>.DrawStatus.AFTER);
        }
    }
}

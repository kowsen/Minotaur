using System;

namespace Minotaur
{
    public class Scene<TGame>
    {
        public EntityComponentManager Entities { get; private set; }
        public SystemManager<TGame> Systems { get; private set; }
        public TGame Game { get; private set; }

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
            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            Entities = new EntityComponentManager();
            Systems = new SystemManager<TGame>(Entities, Game);
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

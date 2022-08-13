using System;
using System.Collections.Generic;

namespace Minotaur
{
    public abstract class World<TGame>
    {
        public EntityComponentManager Entities { get; private set; } = new EntityComponentManager();
        public SystemManager<TGame> Systems { get; private set; }
        public TGame Game { get; private set; }

        public World(TGame game)
        {
            Game = game;
            Systems = new SystemManager<TGame>(Entities, Game);
        }

        public virtual void Initialize() { }

        public void Cleanup()
        {
            Systems.CleanupSystems();
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

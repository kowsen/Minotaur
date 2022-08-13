using System;
using System.Collections.Generic;

namespace Minotaur
{
    public abstract class World<TGame>
    {
        public EntityComponentManager Entities { get; private set; } = new EntityComponentManager();
        public TGame Game { get; private set; }

        private List<System<TGame>> _systems = new List<System<TGame>>();

        public World(TGame game)
        {
            Game = game;
        }

        public abstract void Initialize();

        protected void AddSystem(System<TGame> system)
        {
            system.Attach(Game, Entities);
            _systems.Add(system);
            system.Initialize();
        }

        public void Cleanup()
        {
            foreach (var system in _systems)
            {
                system.Cleanup();
            }
        }

        public void Update(TimeSpan time)
        {
            foreach (var system in _systems)
            {
                var entitySet = Entities.Get(system.Signature);
                system.Update(time, entitySet);
            }

            Entities.CommitChanges();
        }

        public void Draw(TimeSpan time)
        {
            foreach (var system in _systems)
            {
                var entitySet = Entities.Get(system.Signature);
                system.Draw(time, entitySet);
            }
        }
    }
}

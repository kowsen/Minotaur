using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class World<TGame>
    {
        public EntityComponentManager Entities { get; private set; } = new EntityComponentManager();
        public TGame Game { get; private set; }

        private List<System<TGame>> _systems = new List<System<TGame>>();
        private bool _hasStarted = false;

        public World(TGame game)
        {
            Game = game;
        }

        public void AddSystem<TSystem>() where TSystem : System<TGame>, new()
        {
            if (_hasStarted)
            {
                throw new Exception("Cannot add systems after the first world update");
            }
            var system = new TSystem();
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
            if (!_hasStarted)
            {
                Entities.CommitChanges();
                _hasStarted = true;
            }

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

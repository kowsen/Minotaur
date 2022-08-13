using System;
using System.Collections.Generic;

namespace Minotaur
{
    public abstract class World<TGame>
    {
        public EntityComponentManager Entities { get; private set; } = new EntityComponentManager();
        public TGame Game { get; private set; }

        private List<EntitySystem<TGame>> _entitySystems = new List<EntitySystem<TGame>>();
        private List<GameSystem<TGame>> _gameSystems = new List<GameSystem<TGame>>();

        public World(TGame game)
        {
            Game = game;
        }

        public abstract void Initialize();

        protected void AddSystem(EntitySystem<TGame> system)
        {
            system.Attach(Game, Entities);
            _entitySystems.Add(system);
            system.Initialize();
        }

        protected void AddSystem(GameSystem<TGame> system)
        {
            system.Attach(Game, Entities);
            _gameSystems.Add(system);
            system.Initialize();
        }

        public void Cleanup()
        {
            foreach (var entitySystem in _entitySystems)
            {
                entitySystem.Cleanup();
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Cleanup();
            }
        }

        public void Update(TimeSpan time)
        {
            foreach (var entitySystem in _entitySystems)
            {
                var entitySet = Entities.Get(entitySystem.Signature);
                entitySystem.Update(time, entitySet);
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Update(time);
            }

            Entities.CommitChanges();
        }

        public void Draw(TimeSpan time)
        {
            foreach (var entitySystem in _entitySystems)
            {
                var entitySet = Entities.Get(entitySystem.Signature);
                entitySystem.Draw(time, entitySet);
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Draw(time);
            }
        }
    }
}

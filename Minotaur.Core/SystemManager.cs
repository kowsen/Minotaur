using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class SystemManager<TGame>
    {
        private EntityComponentManager _entities;
        private TGame _game;
        private List<EntitySystem<TGame>> _entitySystems = new List<EntitySystem<TGame>>();
        private List<GameSystem<TGame>> _gameSystems = new List<GameSystem<TGame>>();

        public SystemManager(EntityComponentManager entities, TGame game)
        {
            _entities = entities;
            _game = game;
        }

        public void AddSystem(EntitySystem<TGame> system)
        {
            system.Attach(_game, _entities);
            _entitySystems.Add(system);
            system.Initialize();
        }

        public void AddSystem(GameSystem<TGame> system)
        {
            system.Attach(_game, _entities);
            _gameSystems.Add(system);
            system.Initialize();
        }

        public void CleanupSystems()
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
                var entitySet = _entities.Get(entitySystem.Signature);
                entitySystem.Update(time, entitySet);
                foreach (var entity in entitySet)
                {
                    entitySystem.Update(time, entity);
                }
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Update(time);
            }

            _entities.CommitChanges();
        }

        public void Draw(TimeSpan time)
        {
            foreach (var entitySystem in _entitySystems)
            {
                var entitySet = _entities.Get(entitySystem.Signature);
                entitySystem.Draw(time, entitySet);
                foreach (var entity in entitySet)
                {
                    entitySystem.Draw(time, entity);
                }
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Draw(time);
            }
        }
    }
}

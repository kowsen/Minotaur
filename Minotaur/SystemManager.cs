using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Minotaur
{
    public class SystemManager<TGame>
    {
        private EntityComponentManager _entities;
        private List<EntitySystem<TGame>> _entitySystems;
        private List<GameSystem<TGame>> _gameSystems;

        private TGame _game;

        public SystemManager(EntityComponentManager entities, TGame game)
        {
            _entities = entities;
            _entitySystems = new List<EntitySystem<TGame>>();
            _gameSystems = new List<GameSystem<TGame>>();
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

        public void ActivateSystems()
        {
            foreach (var entitySystem in _entitySystems)
            {
                entitySystem.OnActivate();
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.OnActivate();
            }
        }

        public void DeactivateSystems()
        {
            foreach (var entitySystem in _entitySystems)
            {
                entitySystem.OnDeactivate();
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.OnDeactivate();
            }
        }

        public void Update(TimeSpan time)
        {
            foreach (var entitySystem in _entitySystems)
            {
                var entitySet = _entities.Get(entitySystem.Signature);
                if (entitySet.Count > 0)
                {
                    entitySystem.Update(time, entitySet);
                    foreach (var entity in entitySet)
                    {
                        entitySystem.Update(time, entity);
                    }
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
                if (entitySet.Count > 0)
                {
                    entitySystem.Draw(time, entitySet);
                    foreach (var entity in entitySet)
                    {
                        entitySystem.Draw(time, entity);
                    }
                }
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Draw(time);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Minotaur
{
    public class SystemManager<TGame>
    {
        private EntityComponentManager _ecs;
        private ErrandManager<TGame> _errands;
        private List<EntitySystem<TGame>> _entitySystems;
        private List<GameSystem<TGame>> _gameSystems;

        private TGame _game;

        public SystemManager(EntityComponentManager ecs, ErrandManager<TGame> errands, TGame game)
        {
            _ecs = ecs;
            _errands = errands;
            _entitySystems = new List<EntitySystem<TGame>>();
            _gameSystems = new List<GameSystem<TGame>>();
            _game = game;
        }

        public void AddSystem(EntitySystem<TGame> system)
        {
            system.Attach(_game, _ecs, _errands);
            _entitySystems.Add(system);
            system.Initialize();
        }

        public void AddSystem(GameSystem<TGame> system)
        {
            system.Attach(_game, _ecs, _errands);
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
                var entitySet = _ecs.Get(entitySystem.Signature);
                if (entitySet.Entities.Count > 0)
                {
                    entitySystem.Update(time, entitySet);
                    foreach (var entity in entitySet.Entities)
                    {
                        entitySystem.Update(time, entity);
                    }
                }
            }

            foreach (var gameSystem in _gameSystems)
            {
                gameSystem.Update(time);
            }

            _ecs.CommitChanges();
        }

        public void Draw(TimeSpan time)
        {
            foreach (var entitySystem in _entitySystems)
            {
                var entitySet = _ecs.Get(entitySystem.Signature);
                if (entitySet.Entities.Count > 0)
                {
                    entitySystem.Draw(time, entitySet);
                    foreach (var entity in entitySet.Entities)
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

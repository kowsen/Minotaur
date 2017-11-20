using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Minotaur
{
    public class SystemManager<T>
    {
        private EntityComponentManager _ecs;
        private List<EntitySystem<T>> _entitySystems;
        private List<GameSystem<T>> _gameSystems;

        private T _game;

        public SystemManager(EntityComponentManager ecs, T game)
        {
            _ecs = ecs;
            _entitySystems = new List<EntitySystem<T>>();
            _gameSystems = new List<GameSystem<T>>();
            _game = game;
        }

        public void AddSystem(EntitySystem<T> system)
        {
            system.Attach(_game, _ecs);
            _entitySystems.Add(system);
            system.Initialize();
        }

        public void AddSystem(GameSystem<T> system)
        {
            system.Attach(_game, _ecs);
            _gameSystems.Add(system);
            system.Initialize();
        }

        public void ActivateSystems()
        {
            for (var i = 0; i < _entitySystems.Count; i++)
            {
                _entitySystems[i].OnActivate();
            }
            for (var i = 0; i < _gameSystems.Count; i++)
            {
                _gameSystems[i].OnActivate();
            }
        }

        public void DeactivateSystems()
        {
            for (var i = 0; i < _entitySystems.Count; i++)
            {
                _entitySystems[i].OnDeactivate();
            }
            for (var i = 0; i < _gameSystems.Count; i++)
            {
                _gameSystems[i].OnDeactivate();
            }
        }

        public void Update(TimeSpan time)
        {
            for (var i = 0; i < _gameSystems.Count; i++)
            {
                _gameSystems[i].Update(time);
            }

            for (var i = 0; i < _entitySystems.Count; i++)
            {
                var system = _entitySystems[i];
                var entities = _ecs.GetEntities(system.Signature);
                if (entities.Entities.Count > 0)
                {
                    system.Update(time, entities);
                    for (var j = 0; j < entities.Entities.Count; j++)
                    {
                        system.Update(time, entities.Entities[j]);
                    }
                }
            }

            _ecs.ProcessAddRemovalQueue();
        }

        public void Draw(TimeSpan time)
        {
            for (var i = 0; i < _gameSystems.Count; i++)
            {
                _gameSystems[i].Draw(time);
            }

            for (var i = 0; i < _entitySystems.Count; i++)
            {
                var system = _entitySystems[i];
                var entities = _ecs.GetEntities(system.Signature);
                if (entities.Entities.Count > 0)
                {
                    system.Draw(time, entities);
                    for (var j = 0; j < entities.Entities.Count; j++)
                    {
                        system.Draw(time, entities.Entities[j]);
                    }
                }
            }

            _ecs.ProcessAddRemovalQueue();
        }
    }
}

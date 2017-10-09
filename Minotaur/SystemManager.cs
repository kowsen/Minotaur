using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class SystemManager
    {
        private EntityComponentManager _ecs;
        private List<EntitySystem> _entitySystems;
        private List<GameSystem> _gameSystems;

        public SystemManager(EntityComponentManager ecs)
        {
            _ecs = ecs;
            _entitySystems = new List<EntitySystem>();
            _gameSystems = new List<GameSystem>();
        }

        public void AddSystem(EntitySystem system)
        {
            system.Signature = _ecs.GetSignature(system.Types);
            _entitySystems.Add(system);
        }

        public void AddSystem(GameSystem system)
        {
            _gameSystems.Add(system);
        }

        public void Update(TimeSpan time)
        {
            for (var i = 0; i < _entitySystems.Count; i++)
            {
                var system = _entitySystems[i];
                var entities = _ecs.GetEntities(system.Signature);
                system.Update(time, entities);
                for (var j = 0; j < entities.Count; j++)
                {
                    system.Update(time, entities[j]);
                }
            }

            for (var i = 0; i < _gameSystems.Count; i++)
            {
                _gameSystems[i].Update(time);
            }
        }

        public void Draw(TimeSpan time)
        {
            for (var i = 0; i < _entitySystems.Count; i++)
            {
                var system = _entitySystems[i];
                var entities = _ecs.GetEntities(system.Signature);
                system.Draw(time, entities);
                for (var j = 0; j < entities.Count; j++)
                {
                    system.Draw(time, entities[j]);
                }
            }

            for (var i = 0; i < _gameSystems.Count; i++)
            {
                _gameSystems[i].Draw(time);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class SystemManager
    {
        private EntityComponentManager _ecs;
        private List<GameSystem> _systems;

        public SystemManager(EntityComponentManager ecs)
        {
            _ecs = ecs;
            _systems = new List<GameSystem>();
        }

        public void AddSystem(GameSystem system)
        {
            system.Signature = _ecs.GetSignature(system.Types);
            _systems.Add(system);
        }

        public void Update()
        {
            for (var i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];
                var entities = _ecs.GetEntities(system.Signature);
                system.Update(entities);
                for (var j = 0; j < entities.Count; j++)
                {
                    system.Update(entities[j]);
                }
            }
        }

        public void Draw()
        {
            for (var i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];
                var entities = _ecs.GetEntities(system.Signature);
                system.Draw(entities);
                for (var j = 0; j < entities.Count; j++)
                {
                    system.Draw(entities[j]);
                }
            }
        }
    }
}

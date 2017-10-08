using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EntityFactory
    {
        private int _nextEntityId;
        private EntityComponentManager _ecs;

        public EntityFactory(EntityComponentManager ecs)
        {
            _ecs = ecs;
            _nextEntityId = 0;
        }

        public Entity Create()
        {
            var entity = new Entity(_nextEntityId, _ecs);
            _nextEntityId++;
            return entity;
        }
    }
}

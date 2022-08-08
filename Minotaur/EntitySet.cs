using System;
using System.Collections.Generic;
using System.Text;
using Minotaur;

namespace Minotaur
{
    public class EntitySet
    {
        public List<Entity> Entities;
        private Signature _signature;
        private EntityComponentManager _ecm;

        public EntitySet(Signature signature, EntityComponentManager ecm)
        {
            Entities = new List<Entity>();
            _signature = signature;
            _ecm = ecm;
        }

        public void RemoveComponentFromAll<TComponent>() where TComponent : Component
        {
            if (!_signature.IsTypeInRequirements(typeof(TComponent)))
            {
                throw new Exception(
                    "Trying to remove non-required component from all in EntitySet"
                );
            }
            foreach (var entity in Entities)
            {
                entity.RemoveComponent<TComponent>();
            }
        }
    }
}

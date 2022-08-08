using System;
using System.Collections.Generic;
using System.Text;
using Minotaur;

namespace Minotaur
{
    public class EntitySet
    {
        public List<Entity> Entities;
        private BitSet _signature;
        private EntityComponentManager _ecm;

        public EntitySet(BitSet signature, EntityComponentManager ecm)
        {
            Entities = new List<Entity>();
            _signature = signature;
            _ecm = ecm;
        }

        public EntitySet Query(BitSet otherSignature)
        {
            var combinedSignature = otherSignature.Clone();
            combinedSignature.Or(_signature);
            return _ecm.Get(combinedSignature);
        }

        public void RemoveComponentFromAll<TComponent>() where TComponent : Component
        {
            if (
                !ComponentSignatureManager.IsTypeInSignatureRequirements(
                    _signature,
                    typeof(TComponent)
                )
            )
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

using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class Signature
    {
        protected List<Type> _requirements = new List<Type>();
        protected List<Type> _restrictions = new List<Type>();

        public Signature(List<Type> requirements = null, List<Type> restrictions = null)
        {
            _requirements = requirements ?? new List<Type>();
            _restrictions = restrictions ?? new List<Type>();
        }

        public void SetRequirements(params Type[] requirements)
        {
            _requirements = new List<Type>(requirements);
        }

        public void SetRestrictions(params Type[] restrictions)
        {
            _restrictions = new List<Type>(restrictions);
        }

        internal bool Check(BackingEntity entity)
        {
            foreach (var requirement in _requirements)
            {
                if (!entity.HasComponent(requirement))
                {
                    return false;
                }
            }

            foreach (var restriction in _restrictions)
            {
                if (entity.HasComponent(restriction))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

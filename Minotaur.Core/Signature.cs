using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class Signature
    {
        public bool IsEmpty
        {
            get => _requirements.Count == 0 && _restrictions.Count == 0;
        }

        private List<Type> _requirements = new List<Type>();
        private List<Type> _restrictions = new List<Type>();

        public Signature(List<Type> requirements = null, List<Type> restrictions = null)
        {
            _requirements = requirements;
            _restrictions = restrictions;
        }

        public Signature WithRequirements(List<Type> requirements)
        {
            return new Signature(requirements, _restrictions);
        }

        public Signature WithRestrictions(List<Type> restrictions)
        {
            return new Signature(_requirements, restrictions);
        }

        internal bool Check(BackingEntity entity)
        {
            if (IsEmpty)
            {
                return false;
            }

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

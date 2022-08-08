using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace Minotaur
{
    public class Signature
    {
        protected List<Type> _requirements = new List<Type>();
        protected List<Type> _restrictions = new List<Type>();

        public void SetRequirements(params Type[] requirements)
        {
            _requirements = new List<Type>(requirements);
        }

        public void SetRestrictions(params Type[] restrictions)
        {
            _restrictions = new List<Type>(restrictions);
        }

        public bool IsTypeInRequirements(Type type)
        {
            return _requirements.Contains(type);
        }

        public bool IsTypeInRestrictions(Type type)
        {
            return _restrictions.Contains(type);
        }

        public bool Check(BackingEntity entity)
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

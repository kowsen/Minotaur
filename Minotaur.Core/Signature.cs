using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class Signature : IEquatable<Signature>
    {
        private List<int> _requirements = new List<int>();
        private List<int> _restrictions = new List<int>();

        private string _stringSignature = "";

        public void AddRequirement<TComponent>() where TComponent : Component
        {
            var typeId = TypeId<TComponent>.Get();
            if (!_requirements.Contains(typeId))
            {
                _requirements.Add(TypeId<TComponent>.Get());
                UpdateStringSignature();
            }
        }

        public void AddRestriction<TComponent>() where TComponent : Component
        {
            var typeId = TypeId<TComponent>.Get();
            if (!_restrictions.Contains(typeId))
            {
                _restrictions.Add(TypeId<TComponent>.Get());
                UpdateStringSignature();
            }
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

        private void UpdateStringSignature()
        {
            _requirements.Sort();
            _restrictions.Sort();
            _stringSignature =
                $"{String.Join(",", _requirements)}|{String.Join(",", _restrictions)}";
        }

        public bool Equals(Signature other)
        {
            return _stringSignature == other._stringSignature;
        }

        public override int GetHashCode()
        {
            return _stringSignature.GetHashCode();
        }
    }
}

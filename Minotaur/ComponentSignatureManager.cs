using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public static class ComponentSignatureManager
    {
        private static Dictionary<Type, int> _componentBits;
        private static int _nextComponentBit;
        private static BitSet _requirementMask;

        static ComponentSignatureManager()
        {
            _componentBits = new Dictionary<Type, int>();
            _nextComponentBit = 0;
            _requirementMask = new BitSet();
        }

        public static BitSet GenerateComponentSignature(
            List<Type> typeRequirements,
            List<Type> typeRestrictions
        )
        {
            var signature = new BitSet();
            if (typeRequirements != null)
            {
                foreach (var requirement in typeRequirements)
                {
                    signature.Set(GetComponentBit(requirement));
                }
            }
            if (typeRestrictions != null)
            {
                foreach (var restriction in typeRestrictions)
                {
                    signature.Set(GetComponentBit(restriction) + 1);
                }
            }
            return signature;
        }

        // Returns true if the types list contains all types within signature
        public static bool CheckAgainstComponentSignature(BitSet signature, List<Type> types)
        {
            var requirementSignature = GenerateComponentSignature(types, null);
            var restrictionSignature = GenerateComponentSignature(null, types);

            var signatureClone = signature.Clone();
            signatureClone.And(_requirementMask);

            return requirementSignature.ContainsAll(signatureClone)
                && !restrictionSignature.Intersects(signature);
        }

        public static bool IsTypeInSignatureRequirements(BitSet signature, Type type)
        {
            return signature.Get(GetComponentBit(type));
        }

        public static bool IsTypeInSignatureRestrictions(BitSet signature, Type type)
        {
            return signature.Get(GetComponentBit(type) + 1);
        }

        private static int GetComponentBit(Type type)
        {
            int signature;
            var success = _componentBits.TryGetValue(type, out signature);
            if (!success)
            {
                signature = _nextComponentBit;
                _componentBits[type] = signature;
                _requirementMask.Set(signature);
                _nextComponentBit += 2;
            }
            return signature;
        }
    }
}

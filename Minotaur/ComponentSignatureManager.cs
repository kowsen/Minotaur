using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public static class ComponentSignatureManager
    {
        private static Dictionary<Type, int> _signatures;
        private static int _nextSignature;
        private static BitSet _requirementMask;

        static ComponentSignatureManager()
        {
            _signatures = new Dictionary<Type, int>();
            _nextSignature = 0;
            _requirementMask = new BitSet();
        }

        public static BitSet GenerateComponentSignature(List<Type> typeRequirements, List<Type> typeRestrictions)
        {
            var signature = new BitSet();
            if (typeRequirements != null)
            {
                for (var i = 0; i < typeRequirements.Count; i++)
                {
                    signature.Set(GetComponentBit(typeRequirements[i]));
                }
            }
            if (typeRestrictions != null)
            {
                for (var i = 0; i < typeRestrictions.Count; i++)
                {
                    signature.Set(GetComponentBit(typeRestrictions[i]) + 1);
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

            return requirementSignature.ContainsAll(signatureClone) && !restrictionSignature.Intersects(signature);
        }

        // Returns true if otherSignature contains all types within signature
        private static bool CheckAgainstComponentSignature(BitSet signature, BitSet otherSignature)
        {
            var signatureClone = signature.Clone();
            signatureClone.And(otherSignature);
            return signatureClone.Equals(signature);
        }

        public static bool IsTypeInSignatureRequirements(BitSet signature, Type type)
        {
            var typeBits = GetComponentBit(type);
            return signature.Get(typeBits);
        }

        public static bool IsTypeInSignatureRestrictions(BitSet signature, Type type)
        {
            var typeBits = GetComponentBit(type);
            return signature.Get(typeBits + 1);
        }

        private static int GetComponentBit(Type type)
        {
            int signature;
            var success = _signatures.TryGetValue(type, out signature);
            if (!success)
            {
                signature = _nextSignature;
                _signatures[type] = signature;
                _requirementMask.Set(signature);
                _nextSignature += 2;
            }
            return signature;
        }
    }
}

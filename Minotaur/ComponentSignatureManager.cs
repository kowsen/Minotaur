using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class ComponentSignatureManager
    {
        private Dictionary<Type, int> _signatures;
        private int _nextSignature;

        public ComponentSignatureManager()
        {
            _signatures = new Dictionary<Type, int>();
            _nextSignature = 1;
        }

        public int GenerateComponentSignature(List<Type> types)
        {
            int signature = 0;
            for (var i = 0; i < types.Count; i++)
            {
                signature |= GetComponentBits(types[i]);
            }
            return signature;
        }

        // Returns true if the types list contains all types within signature
        public bool CheckAgainstComponentSignature(int signature, List<Type> types)
        {
            return CheckAgainstComponentSignature(signature, GenerateComponentSignature(types));
        }

        // Returns true if otherSignature contains all types within signature
        public bool CheckAgainstComponentSignature(int signature, int otherSignature)
        {
            return (signature & otherSignature) == signature;
        }

        public bool IsTypeInSignature(int signature, Type type)
        {
            var typeBits = GetComponentBits(type);
            return (signature & typeBits) == typeBits;
        }

        private int GetComponentBits(Type type)
        {
            int signature;
            var success = _signatures.TryGetValue(type, out signature);
            if (!success)
            {
                signature = _nextSignature;
                _signatures[type] = signature;
                _nextSignature *= 2;
            }
            return signature;
        }
    }
}

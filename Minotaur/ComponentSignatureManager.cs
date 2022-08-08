using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public static class ComponentSignatureManager
    {
        private static Dictionary<Type, int> _componentToBit;
        private static Dictionary<int, Type> _bitToComponent;
        private static int _nextComponentBit;

        static ComponentSignatureManager()
        {
            _componentToBit = new Dictionary<Type, int>();
            _bitToComponent = new Dictionary<int, Type>();
            _nextComponentBit = 0;
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
        public static bool CheckAgainstComponentSignature(BitSet signature, BackingEntity entity)
        {
            for (var i = 0; i < _nextComponentBit; i += 2)
            {
                if (signature.Get(i) && !entity.HasComponent(GetBitComponent(i)))
                {
                    return false;
                }
                if (signature.Get(i + 1) && entity.HasComponent(GetBitComponent(i)))
                {
                    return false;
                }
            }

            return true;
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
            var success = _componentToBit.TryGetValue(type, out var componentBit);
            if (!success)
            {
                componentBit = _nextComponentBit;
                _componentToBit[type] = componentBit;
                _bitToComponent[componentBit] = type;
                _nextComponentBit += 2;
            }
            return componentBit;
        }

        private static Type GetBitComponent(int componentBit)
        {
            var success = _bitToComponent.TryGetValue(componentBit, out var type);
            if (!success)
            {
                throw new Exception($"Tried to get type for unregistered bit: {componentBit}");
            }
            return type;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Minotaur
{
    public static class TypeId<TClass> where TClass : class
    {
        private static int _id;

        static TypeId()
        {
            _id = TypeId.Next();
        }

        public static int Get()
        {
            return _id;
        }
    }

    public static class TypeId
    {
        private static int _nextId = 0;

        public static int Next()
        {
            var id = _nextId;
            _nextId += 1;
            return id;
        }
    }
}

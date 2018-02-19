using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public abstract class IComponent
    {
        public int EntityId { get; private set; }

        public void Attach(int id)
        {
            EntityId = id;
        }
    }
}

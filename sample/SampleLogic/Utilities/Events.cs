using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace SampleLogic.Utilities
{
    public class BounceEvent : IEvent
    {
        public int Id { get; set; }

        public BounceEvent(int id)
        {
            Id = id;
        }
    }

    public class SpacePressEvent : IEvent { }

    public class StartEvent : IEvent { }
}

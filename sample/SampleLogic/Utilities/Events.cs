using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace SampleLogic.Utilities
{
    public enum Events
    {
        BOUNCE,
        SPACE_PRESS,
        START
    }

    public class BounceArgs : EventBusArgs
    {
        public int Id { get; set; }

        public BounceArgs(int id)
        {
            Id = id;
        }
    }
}

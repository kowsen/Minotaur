using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace BounceLogic.Systems
{
    public class EndMessengerSystem : GameSystem
    {
        private EventBus _bus;
        private string _message;
        private int _ms;
        private double _counter = 0;

        public EndMessengerSystem(EventBus bus, string message, int ms)
        {
            _bus = bus;
            _message = message;
            _ms = ms;
        }

        public override void Update(TimeSpan time)
        {
            _counter += time.TotalMilliseconds;
            if (_counter > _ms)
            {
                _bus.Notify(_message);
                _counter = 0;
            }
        }
    }
}

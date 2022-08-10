using System;

namespace SampleLogic.Utilities
{
    public class EventWithArgs<TArgs>
    {
        public delegate void EventWithArgsHandler(TArgs args);

        public event EventWithArgsHandler Event;

        public void Emit(TArgs args)
        {
            Event?.Invoke(args);
        }
    }

    public class EventWithoutArgs
    {
        public delegate void EventWithoutArgsHandler();

        public event EventWithoutArgsHandler Event;

        public void Emit()
        {
            Event?.Invoke();
        }
    }

    public class EventBus
    {
        public EventWithArgs<int> Bounce = new EventWithArgs<int>();
        public EventWithoutArgs SpacePress = new EventWithoutArgs();
        public EventWithoutArgs Start = new EventWithoutArgs();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public static class ActionDelayer
    {
        private static List<DelayedAction> Actions;

        static ActionDelayer()
        {
            Actions = new List<DelayedAction>();
        }

        public static void Schedule(Action action, int frames)
        {
            Actions.Add(new DelayedAction(action, frames));
        }

        public static void Update()
        {
            for (var i = 0; i < Actions.Count; i++)
            {
                var action = Actions[i];
                action.Frames--;
                if (action.Frames <= 0)
                {
                    action.Action.Invoke();
                    Actions.RemoveAt(i);
                    i--;
                }
            }
        }

        class DelayedAction
        {
            public Action Action;
            public int Frames;

            public DelayedAction(Action action, int frames)
            {
                Action = action;
                Frames = frames;
            }
        }
    }
}

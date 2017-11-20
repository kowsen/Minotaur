using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EaseValue
    {
        private static List<EaseValue> _allVals = new List<EaseValue>();

        private float _val;
        private float _increment;
        private int _ticks;

        public EaseValue() : this(0) { }

        public EaseValue(float initial)
        {
            _val = initial;
            _allVals.Add(this);
        }

        public float Get()
        {
            return _val;
        }

        public void Set(float val, int frames, ConflictMode mode = ConflictMode.OVERRIDE)
        {
            if (frames == 0)
            {
                Reset(val);
            }
            else
            {
                _increment = (val - _val) / frames;
                _ticks = frames;
            }
        }

        public void Reset(float val)
        {
            _increment = 0;
            _ticks = 0;
            _val = val;
        }

        public void Destroy()
        {
            _allVals.Remove(this);
        }

        public void Update()
        {
            if (_ticks > 0)
            {
                _val += _increment;
                _ticks--;
            }
        }

        public static void UpdateAll()
        {
            for (var i = 0; i < _allVals.Count; i++)
            {
                _allVals[i].Update();
            }
            for (var i = 0; i < Actions.Count; i++)
            {
                var action = Actions[i];
                action.FramesLeft--;
                if (action.FramesLeft == 0)
                {
                    action.Act.Invoke();
                    Actions.Remove(action);
                    i--;
                }
            }
        }

        private static List<DeferredAction> Actions = new List<DeferredAction>();

        public static void DeferAction(Action action, int frames)
        {
            Actions.Add(new DeferredAction(action, frames));
        }

        public enum ConflictMode
        {
            IGNORE,
            OVERRIDE,
            WAIT
        }
    }

    public class DeferredAction
    {
        public Action Act;
        public int FramesLeft;

        public DeferredAction(Action act, int frames)
        {
            Act = act;
            FramesLeft = frames;
        }
    }
}

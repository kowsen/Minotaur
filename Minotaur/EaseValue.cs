using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class EaseValue
    {
        private static List<EaseValue> _allVals = new List<EaseValue>();

        private float _val;
        private float _diff;
        private float _target;
        private int _ticks;
        private int _totalTicks;
        private EaseMode _mode;

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

        public void Set(float val, int frames, EaseMode mode = EaseMode.LINEAR)
        {
            if (frames == 0)
            {
                Reset(val);
            }
            else
            {
                _target = val;
                _diff = _val - _target;
                _ticks = 0;
                _totalTicks = frames;
                _mode = mode;
            }
        }

        public void Reset(float val)
        {
            _diff = 0;
            _totalTicks = 0;
            _ticks = 0;
            _target = 0;
            _mode = 0;
            _val = val;
        }

        public void Destroy()
        {
            _allVals.Remove(this);
        }

        public void Update(TimeSpan time)
        {
            if (_ticks < _totalTicks)
            {
                _ticks++;
                if (_ticks >= _totalTicks)
                {
                    _val = _target;
                }
                else
                {
                    _val = _target + (_diff * (1 - GetTime()));
                }
            }
        }

        public float GetVelocity()
        {
            if (_ticks >= _totalTicks)
            {
                return 0;
            }
            return _diff * (GetTime() - GetTime(_ticks + 1));
        }

        private float GetTime()
        {
            return GetTime(_ticks);
        }

        private float GetTime(float ticks)
        {
            var t = 1f * ticks / _totalTicks;
            if (_mode == EaseMode.LINEAR)
            {
                return t;
            }
            else if (_mode == EaseMode.QUAD_OUT)
            {
                return t * (2 - t);
            }
            return t;
        }

        public static void UpdateAll(TimeSpan time)
        {
            for (var i = 0; i < _allVals.Count; i++)
            {
                _allVals[i].Update(time);
            }
        }

        public enum EaseMode
        {
            LINEAR,
            QUAD_OUT
        }

        public enum ConflictMode
        {
            IGNORE,
            OVERRIDE,
            WAIT
        }
    }
}

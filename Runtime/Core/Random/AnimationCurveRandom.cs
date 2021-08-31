using UnityEngine;
using d4160.Collections;

namespace d4160.RandomM31 
{
    public class AnimationCurveRandom 
    {
        public AnimationCurve AnimationCurve { get; set; }

        private float _lowerValue = float.MaxValue;

        AnimationCurve DefaultCurve() => AnimationCurve.EaseInOut (0f, 0f, 100f, 100f);

        public AnimationCurveRandom()
        {
        }

        public AnimationCurveRandom(AnimationCurve curve)
        {
            AnimationCurve = curve;
        }

        public void Setup () 
        {
            CheckForValidCurve();
            CalculateLowerValue();
        }

        public void Reset() 
        {
            _lowerValue = float.MaxValue;
            Setup();
        }

        private void CalculateLowerValue() 
        {
            if (_lowerValue == float.MaxValue)
            {
                var min = float.MaxValue;
                for (var i = 0; i < AnimationCurve.keys.Length; i++)
                {
                    Keyframe key = AnimationCurve.keys[i];
                    if (key.value < min) min = key.value;
                }
                _lowerValue = min;
            }
        }

        private void CheckForValidCurve() 
        {
            if (AnimationCurve == null || AnimationCurve.keys.Length < 2) 
            {
                AnimationCurve = DefaultCurve();
            }
        }

        private float GetRandom() 
        {
            Setup();
            Keyframe first = AnimationCurve.keys.First();
            Keyframe last = AnimationCurve.keys.Last();

            float time = Random.Range(first.time, last.time);
            float value = Random.Range(_lowerValue, AnimationCurve.Evaluate(time));

            return value;
        }

        public int RandomInt() => (int)GetRandom();
        public float RandomFloat() => GetRandom();
    }
}
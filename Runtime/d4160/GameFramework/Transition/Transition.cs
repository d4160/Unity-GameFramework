using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.GameFramework.Transition
{
    [Serializable]
    public class Transition
    {
        [SerializeField] protected bool _autoPlayOnStart;
        [Tooltip("X: Tween To (Fade in for example), Y: Wait for, Z: Reverse tween (Fade out for example)")]
        [SerializeField] protected Vector3 _duration;
        [SerializeField] protected float _delay;
        [SearchableEnum]
        [SerializeField] protected Ease _ease;
        [ShowIf("UseCustomEase")]
        [AllowNesting]
        [SerializeField] protected AnimationCurve _easeCurve;

        private bool UseCustomEase => _ease == Ease.INTERNAL_Custom;

        protected float _timer = 0f;
        protected Sequence _sequence;

        public void Start(Action completed = null, Action<float> onUpdate = null, Action onInterval1 = null, Action onInterval2 = null)
        {
            DOTween.defaultAutoPlay = _autoPlayOnStart ? AutoPlay.All : AutoPlay.None;
            
            _sequence = DOTween.Sequence();

            if(_delay > 0)
                _sequence.AppendInterval(_delay);

            var tween1 = 
                DOTween.To(() => _timer, 
                    (f) =>
                    {
                        onUpdate?.Invoke(f);
                        _timer = f;
                    }, 1f, _duration.x);
            
            if (_duration.x > 0)
            {
                if (UseCustomEase)
                {
                    _sequence.SetEase(_easeCurve);
                }
                else
                {
                    _sequence.SetEase(_ease);
                }
            }

            _sequence.Append(tween1);

            if (onInterval1 != null) _sequence.AppendCallback(onInterval1.Invoke);

            _sequence.AppendInterval(_duration.y);

            if (onInterval2 != null) _sequence.AppendCallback(onInterval2.Invoke);

            var tween2 = 
                DOTween.To(() => _timer, 
                    (f) =>
                    {
                        onUpdate?.Invoke(f);
                        _timer = f;
                    }, 0f, _duration.z);

            if (_duration.z > 0)
            {
                if (UseCustomEase)
                {
                    _sequence.SetEase(_easeCurve);
                }
                else
                {
                    _sequence.SetEase(_ease);
                }
            }

            _sequence.Append(tween2);
            
            if (completed != null)
                _sequence.OnComplete(completed.Invoke);

            DOTween.defaultAutoPlay = AutoPlay.All;
        }

        public void Play()
        {
            if (_sequence != null && !_sequence.IsPlaying())
            {
                _sequence.Play();
            }
        }

        public void Pause()
        {
            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Pause();
            }
        }

        public void TogglePause()
        {
            _sequence?.TogglePause();
        }

        public void Kill()
        {
            _sequence?.Kill();
        }
    }
}
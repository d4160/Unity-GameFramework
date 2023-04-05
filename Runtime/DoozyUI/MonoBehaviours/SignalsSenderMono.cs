using d4160.Collections;
using d4160.Core;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace d4160.DoozyUI
{
    public class SignalsSenderMono : MonoBehaviour
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private SignalStreamSO[] _signals;
        [SerializeField] private int _signalIndex;
        [SerializeField] private UnityLifetimeMethodType _sendAt;
        [SerializeField, Range(0, 6)] private float _sendDelay = 1f;

        private void Awake()
        {
            if (_sendAt == UnityLifetimeMethodType.Awake)
            {
                SendSignalWithDelay();
            }
        }

        private void Start()
        {
            if (_sendAt == UnityLifetimeMethodType.Start)
            {
                SendSignalWithDelay();
            }
        }

        private void OnEnable()
        {
            if (_sendAt == UnityLifetimeMethodType.OnEnable)
            {
                SendSignalWithDelay();
            }
        }

        private void SendSignalWithDelay()
        {
            if (_sendDelay > 0)
            {
                StartCoroutine(SendSignalCo());
            }
            else
            {
                SendSignal();
            }
        }

        private IEnumerator SendSignalCo()
        {
            yield return new WaitForSeconds(_sendDelay);

            SendSignal();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SendSignal()
        {
            if (_signals.IsValidIndex(_signalIndex))
            {
                _signals[_signalIndex].SendSignal();
            }
        }
    }
}
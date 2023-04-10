using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.DoozyUI
{
    public class SignalsConnectorMono : MonoBehaviour
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private SignalStreamSO[] _streams;

        private void Awake()
        {
            for (int i = 0; i < _streams.Length; i++)
            {
                _streams[i].Setup();
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < _streams.Length; i++)
            {
                //if (i == 1)
                //    Debug.Log($"[SignalsConnector] Registered: {_streams[i].category}, {_streams[i]._name}");
                _streams[i].RegisterEvents();
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _streams.Length; i++)
            {
                _streams[i].UnregisterEvents();
            }
        }
    }
}
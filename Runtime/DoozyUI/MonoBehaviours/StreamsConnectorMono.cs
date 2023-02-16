using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.DoozyUI
{
    public class StreamsConnectorMono : MonoBehaviour
    {
        [SerializeField] private StreamSO[] _streams;

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
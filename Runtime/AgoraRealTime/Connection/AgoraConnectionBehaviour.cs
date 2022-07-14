#if AGORA
using System;
using d4160.MonoBehaviourData;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Agora
{

    public class AgoraConnectionBehaviour : MonoBehaviourUnityData<AgoraConnectionSO>
    {
        [SerializeField] private UltEvent<int, string> _onEngineError;
        [SerializeField] private UltEvent<int, string> _onEngineWarning;

        void Awake() {
            LoadEngine();
        }

        void OnEnable(){
            if (_data)
            {
                _data.RegisterEvents();
                AgoraConnectionService.OnEngineWarning += _onEngineWarning.Invoke;
                AgoraConnectionService.OnEngineError += _onEngineError.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.UnregisterEvents();
                AgoraConnectionService.OnEngineWarning -= _onEngineWarning.Invoke;
                AgoraConnectionService.OnEngineError -= _onEngineError.Invoke;
            }
        }

        void OnApplicationQuit() {
            UnloadEngine();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void LoadEngine(){
            if (_data) _data.LoadEngine();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UnloadEngine(){
            if (_data) _data.UnloadEngine();
        }
    }
}
#endif
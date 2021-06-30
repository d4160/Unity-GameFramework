#if AGORA
using System;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;

namespace d4160.Auth.Agora
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

        [Button]
        public void LoadEngine(){
            if (_data) _data.LoadEngine();
        }

        [Button]
        public void UnloadEngine(){
            if (_data) _data.UnloadEngine();
        }
    }
}
#endif
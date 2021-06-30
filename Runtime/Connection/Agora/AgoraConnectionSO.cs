#if AGORA
using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Auth.Agora
{

    [CreateAssetMenu(menuName = "d4160/Agora/Connection")]
    public class AgoraConnectionSO : ScriptableObject
    {
        [Expandable, SerializeField] private AgoraAuthSettingsSO _agoraSettings;

        private readonly AgoraConnectionService _authService = AgoraConnectionService.Instance;

        public event Action<int, string> OnEngineError;
        public event Action<int, string> OnEngineWarning;

        private void CallOnEngineWarningEvent(int code, string message) => OnEngineWarning?.Invoke(code, message);
        private void CallOnEngineErrorEvent(int code, string message) => OnEngineError?.Invoke(code, message);


        public void RegisterEvents(){
            AgoraConnectionService.OnEngineWarning += CallOnEngineWarningEvent;
            AgoraConnectionService.OnEngineError += CallOnEngineErrorEvent;
        }

        public void UnregisterEvents(){
            AgoraConnectionService.OnEngineWarning -= CallOnEngineWarningEvent;
            AgoraConnectionService.OnEngineError -= CallOnEngineErrorEvent;
        }

        [Button]
        public void LoadEngine(){
            _authService.LoadEngine(_agoraSettings);
        }

        [Button]
        public void UnloadEngine(){
            _authService.UnloadEngine();
        }
    }
}
#endif
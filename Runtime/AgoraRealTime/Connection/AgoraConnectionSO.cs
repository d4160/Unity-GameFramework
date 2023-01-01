#if AGORA
using System;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Agora_
{

    [CreateAssetMenu(menuName = "d4160/Agora/Connection")]
    public class AgoraConnectionSO : ScriptableObject
    {
        [Expandable, SerializeField] private AgoraAuthSettingsSO _agoraSettings;

        private readonly AgoraConnectionService _authService = AgoraConnectionService.Instance;

        public event Action<int, string> OnEngineError;

        private void CallOnEngineErrorEvent(int code, string message) => OnEngineError?.Invoke(code, message);

        public void RegisterEvents(){
            AgoraConnectionService.OnError += CallOnEngineErrorEvent;
        }

        public void UnregisterEvents(){
            AgoraConnectionService.OnError -= CallOnEngineErrorEvent;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void LoadEngine(){
            _authService.InitEngine(_agoraSettings);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UnloadEngine(){
            _authService.ShutdownEngine();
        }
    }
}
#endif
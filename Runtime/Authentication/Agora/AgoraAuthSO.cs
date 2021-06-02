#if AGORA
using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Auth.Agora
{

    [CreateAssetMenu(menuName = "d4160/Authentication/Agora")]
    public class AgoraAuthSO : ScriptableObject
    {
        [SerializeField] private AgoraAuthSettingsSO _agoraSettings;
        [SerializeField] private string _userAccount;

        private readonly AgoraAuthService _authService = AgoraAuthService.Instance;

        public event Action<int, string> OnAuthError;
        public event Action<int, string> OnAuthWarning;


        public void RegisterEvents(){
            AgoraAuthService.OnAuthWarning += OnAuthWarning.Invoke;
            AgoraAuthService.OnAuthError += OnAuthError.Invoke;
        }

        public void UnregisterEvents(){
            AgoraAuthService.OnAuthWarning -= OnAuthWarning.Invoke;
            AgoraAuthService.OnAuthError -= OnAuthError.Invoke;
        }

        [Button]
        public void Login(){
            _authService.AgoraSettings = _agoraSettings;
            // AuthManager.Login(_authService);
            _authService.Login(default);
        }

        [Button]
        public void Register(){
            _authService.AgoraSettings = _agoraSettings;
            _authService.UserAccount = _userAccount;

            _authService.Register(default);
        }

        [Button]
        public void Logout(){
            _authService.Logout(default);
        }
    }
}
#endif
#if AGORA
using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Auth.Agora
{

    [CreateAssetMenu(menuName = "d4160/Agora/Authentication")]
    public class AgoraAuthSO : ScriptableObject
    {
        [SerializeField] private string _userAccount;

        private readonly AgoraAuthService _authService = AgoraAuthService.Instance;

        public event Action<uint, string> OnLocalUserRegisteredEvent;

        private void CallOnLocalUserRegisteredEvent(uint code, string message) => OnLocalUserRegisteredEvent?.Invoke(code, message);

        public void RegisterEvents(){
            _authService.RegisterEvents();
            AgoraAuthService.OnLocalUserRegistered += CallOnLocalUserRegisteredEvent;
        }

        public void UnregisterEvents(){
            _authService.UnregisterEvents();
            AgoraAuthService.OnLocalUserRegistered -= CallOnLocalUserRegisteredEvent;
        }

        [Button]
        public void Login(){
            _authService.Login(default);
        }

        [Button]
        public void Register(){
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
#if PHOTON_UNITY_NETWORKING
using System;
using d4160.Authentication;
using NaughtyAttributes;
using Photon.Realtime;
using UnityEngine;

namespace d4160.Auth.Photon
{

    [CreateAssetMenu(menuName = "d4160/Authentication/Photon")]
    public class PhotonAuthSO : ScriptableObject
    {
        [SerializeField] private CustomAuthenticationType _authType;
        [ShowIf("_authType", CustomAuthenticationType.Custom)]
        [SerializeField] private string _customServiceId;
        [ShowIf("_authType", CustomAuthenticationType.Custom)]
        [SerializeField] private string _token;
        [SerializeField] private string _nickname;

        private readonly PhotonAuthService _authService = PhotonAuthService.Instance;

        public event Action OnCancelAuthentication;
        public event Action OnLoginSuccess;
        public event Action OnLogoutSuccess;
        public event Action<Exception> OnAuthError;

        public void RegisterEvents(){
            PhotonAuthService.OnCancelAuthentication += OnCancelAuthentication.Invoke;
            PhotonAuthService.OnLoginSuccess += OnLoginSuccess.Invoke;
            PhotonAuthService.OnLogoutSuccess += OnLogoutSuccess.Invoke;
            PhotonAuthService.OnAuthError += OnAuthError.Invoke;
        }

        public void UnregisterEvents(){
            PhotonAuthService.OnCancelAuthentication -= OnCancelAuthentication.Invoke;
            PhotonAuthService.OnLoginSuccess -= OnLoginSuccess.Invoke;
            PhotonAuthService.OnLogoutSuccess -= OnLogoutSuccess.Invoke;
            PhotonAuthService.OnAuthError -= OnAuthError.Invoke;
        }

        [Button]
        public void Login(){
            _authService.Id = _customServiceId;
            _authService.SessionTicket = _token;
            _authService.DisplayName = _nickname;

            CheckAndExecute(() => { 
                AuthManager.Login(_authService);
            });
        }

        [Button]
        public void SetNickname(string nickname = null){
            CheckAndExecute(() => { 
                _authService.DisplayName = nickname ?? _nickname;
            });
        }

        public string GetNickname() {
            if (Application.isPlaying) {
                return _authService.DisplayName;
            }

            return string.Empty;
        }

        [Button]
        public void Logout(){
            CheckAndExecute(() => { 
                AuthManager.Logout();
            });
        }

        private void CheckAndExecute(Action toExecute){
            toExecute.Invoke();
        }
    }
}
#endif
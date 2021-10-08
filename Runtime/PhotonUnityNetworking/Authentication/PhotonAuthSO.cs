#if PHOTON_UNITY_NETWORKING
using System;
using d4160.Authentication;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using Photon.Realtime;
using UnityEngine;
using CodeMonkey.Utils;

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
        [SerializeField] private bool _useRandomNickname;
        [HideIf("_useRandomNickname")]
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

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Login(){
            _authService.Id = _customServiceId;
            _authService.SessionTicket = _token;
            _authService.DisplayName = _nickname;

            CheckAndExecute(() => { 
                AuthManager.Login(_authService);
            });
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetNickname(string nickname = null){
            CheckAndExecute(() => { 
                _authService.DisplayName = string.IsNullOrEmpty(nickname) ? (_useRandomNickname ? UtilsClass.GetRandomName(true) : _nickname) : nickname;
            });
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetRandomNickname(){
            CheckAndExecute(() => { 
                _authService.DisplayName = UtilsClass.GetRandomName(true);
            });
        }

        public string GetNickname() {
            if (Application.isPlaying) {
                return _authService.DisplayName;
            }

            return string.Empty;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
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
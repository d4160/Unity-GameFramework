#if VIVOX
using System;
using System.ComponentModel;
using d4160.Authentication;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Auth.Vivox
{

    [CreateAssetMenu(menuName = "d4160/Authentication/Vivox")]
    public class VivoxAuthSO : ScriptableObject
    {
        [SerializeField] private VivoxAuthSettingsSO _vivoxSettings;
        [SerializeField] private string _uniqueId;
        [SerializeField] private string _displayName;

        private readonly VivoxAuthService _authService = VivoxAuthService.Instance;

        public event Action<object, PropertyChangedEventArgs> OnLoginSessionPropertyChanged;
        public event Action OnLoginSuccess;
        public event Action OnLogoutSuccess;
        public event Action<Exception> OnAuthError;

        public string UniqueId { get => _uniqueId; set => _uniqueId = value; }
        public string DisplayName { get => _displayName; set => _displayName = value; }

        public void RegisterEvents(){
            VivoxAuthService.OnLoginSessionPropertyChanged += OnLoginSessionPropertyChanged.Invoke;
            VivoxAuthService.OnLoginSuccess += OnLoginSuccess.Invoke;
            VivoxAuthService.OnLogoutSuccess += OnLogoutSuccess.Invoke;
            VivoxAuthService.OnAuthError += OnAuthError.Invoke;
        }

        public void UnregisterEvents(){
            VivoxAuthService.OnLoginSessionPropertyChanged -= OnLoginSessionPropertyChanged.Invoke;
            VivoxAuthService.OnLoginSuccess -= OnLoginSuccess.Invoke;
            VivoxAuthService.OnLogoutSuccess -= OnLogoutSuccess.Invoke;
            VivoxAuthService.OnAuthError -= OnAuthError.Invoke;
        }

        [Button]
        public void Login(){
            _authService.AuthSettings = _vivoxSettings;
            _authService.Id = _uniqueId;
            _authService.DisplayName = _displayName;

            // AuthManager.Login(_authService);
            _authService.Login(default);
        }

        [Button]
        public void Logout(){
            _authService.Logout(default);
        }
    }
}
#endif
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
        [Expandable, SerializeField] private VivoxAuthSettingsSO _vivoxSettings;
        [Tooltip("Valid characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890=+-_.!~()%")]
        [SerializeField] private string _uniqueId;
        [SerializeField] private string _displayName;

        public event Action<object, PropertyChangedEventArgs> OnLoginSessionPropertyChanged;
        public event Action OnLoginSuccess;
        public event Action OnLogoutSuccess;
        public event Action<Exception> OnAuthError;

        public string UniqueId { get => _uniqueId; set => _uniqueId = value; }
        public string DisplayName { get => _displayName; set => _displayName = value; }

        private void CallOnLoginSessionPropertyChanged (object obj, PropertyChangedEventArgs args) => OnLoginSessionPropertyChanged?.Invoke (obj, args);
        private void CallOnLoginSuccess () => OnLoginSuccess?.Invoke ();
        private void CallOnLogoutSuccess () => OnLogoutSuccess?.Invoke ();
        private void CallOnAuthError (Exception ex) => OnAuthError?.Invoke (ex);

        private readonly VivoxAuthService _authService = VivoxAuthService.Instance;

        public void RegisterEvents(){
            VivoxAuthService.OnLoginSessionPropertyChanged += CallOnLoginSessionPropertyChanged;
            VivoxAuthService.OnLoginSuccess += CallOnLoginSuccess;
            VivoxAuthService.OnLogoutSuccess += CallOnLogoutSuccess;
            VivoxAuthService.OnAuthError += CallOnAuthError;
        }

        public void UnregisterEvents(){
            VivoxAuthService.OnLoginSessionPropertyChanged -= CallOnLoginSessionPropertyChanged;
            VivoxAuthService.OnLoginSuccess -= CallOnLoginSuccess;
            VivoxAuthService.OnLogoutSuccess -= CallOnLogoutSuccess;
            VivoxAuthService.OnAuthError -= CallOnAuthError;
        }

        public void SetUniqueIdAndDisplayName(string uniqueId, string displayName) {
            UniqueId = uniqueId;;
            DisplayName = displayName;
        }

        [Button]
        public void Login(){
            
            _authService.AuthSettings = _vivoxSettings;
            _authService.Id = _uniqueId;
            _authService.DisplayName = _displayName;

            _authService.Login(default);
        }

        [Button]
        public void Logout(){
            _authService.Logout(default);
        }

        [Button]
        public void CleanUp(){
            _authService.CleanUp();
        }
    }
}
#endif
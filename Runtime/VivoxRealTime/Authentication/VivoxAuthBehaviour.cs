#if VIVOX
using System;
using System.ComponentModel;
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Auth.Vivox
{

    public class VivoxAuthBehaviour : MonoBehaviourUnityData<VivoxAuthSO>
    {
        [SerializeField] private UltEvent<object, PropertyChangedEventArgs> _onLoginSessionPropertyChanged;
        [SerializeField] private UltEvent _onLoginSuccess;
        [SerializeField] private UltEvent _onLogoutSuccess;
        [SerializeField] private UltEvent<Exception> _onAuthError;

        void OnEnable(){
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnLoginSessionPropertyChanged += _onLoginSessionPropertyChanged.Invoke;
                _data.OnLoginSuccess += _onLoginSuccess.Invoke;
                _data.OnLogoutSuccess += _onLogoutSuccess.Invoke;
                _data.OnAuthError += _onAuthError.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnLoginSessionPropertyChanged -= _onLoginSessionPropertyChanged.Invoke;
                _data.OnLoginSuccess -= _onLoginSuccess.Invoke;
                _data.OnLogoutSuccess -= _onLogoutSuccess.Invoke;
                _data.OnAuthError -= _onAuthError.Invoke;
            }
        }

        void OnApplicationQuit(){
            CleanUp();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Login(){
            if(_data) {
                _data.Login();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Logout(){
            if(_data) {
                _data.Logout();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void CleanUp(){
            if(_data) {
                _data.CleanUp();
            }
        }

        public void SetUniqueIdAndDisplayName(string uniqueId, string displayName) {
            if (_data)
                _data.SetUniqueIdAndDisplayName(uniqueId, displayName);
        }
    }
}
#endif
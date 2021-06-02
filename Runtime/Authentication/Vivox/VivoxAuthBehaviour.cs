#if VIVOX
using System;
using System.ComponentModel;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
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

        public void RegisterEvents(){
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnLoginSessionPropertyChanged += _onLoginSessionPropertyChanged.Invoke;
                _data.OnLoginSuccess += _onLoginSuccess.Invoke;
                _data.OnLogoutSuccess += _onLogoutSuccess.Invoke;
                _data.OnAuthError += _onAuthError.Invoke;
            }
        }

        public void UnregisterEvents(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnLoginSessionPropertyChanged -= _onLoginSessionPropertyChanged.Invoke;
                _data.OnLoginSuccess -= _onLoginSuccess.Invoke;
                _data.OnLogoutSuccess -= _onLogoutSuccess.Invoke;
                _data.OnAuthError -= _onAuthError.Invoke;
            }
        }

        [Button]
        public void Login(){
            if(_data) {
                _data.Login();
            }
        }

        [Button]
        public void Logout(){
            if(_data) {
                _data.Logout();
            }
        }
    }
}
#endif
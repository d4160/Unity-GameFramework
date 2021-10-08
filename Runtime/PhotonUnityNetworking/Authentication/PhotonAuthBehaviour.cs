#if PHOTON_UNITY_NETWORKING
using System;
using d4160.MonoBehaviourData;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Auth.Photon
{

    public class PhotonAuthBehaviour : MonoBehaviourUnityData<PhotonAuthSO>
    {
        [SerializeField] private UltEvent _onCancelAuthentication;
        [SerializeField] private UltEvent _onLoginSuccess;
        [SerializeField] private UltEvent _onLogoutSuccess;
        [SerializeField] private UltEvent<Exception> _onAuthError;

        public void RegisterEvents(){
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnCancelAuthentication += _onCancelAuthentication.Invoke;
                _data.OnLoginSuccess += _onLoginSuccess.Invoke;
                _data.OnLogoutSuccess += _onLogoutSuccess.Invoke;
                _data.OnAuthError += _onAuthError.Invoke;
            }
        }

        public void UnregisterEvents(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnCancelAuthentication -= _onCancelAuthentication.Invoke;
                _data.OnLoginSuccess -= _onLoginSuccess.Invoke;
                _data.OnLogoutSuccess -= _onLogoutSuccess.Invoke;
                _data.OnAuthError -= _onAuthError.Invoke;
            }
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
        public void SetNickname(string nickname = null){
            if(_data) {
                _data.SetNickname(nickname);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetRandomNickname() => _data?.SetRandomNickname();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Logout(){
            if(_data) {
                _data.Logout();
            }
        }
    }
}
#endif
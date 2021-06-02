#if PLAYFAB
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UltEvents;
using UnityEngine;

namespace d4160.Auth.PlayFab {
    public class PlayFabAuthBehaviour : MonoBehaviourUnityData<PlayFabAuthSO> {

        [Header ("EVENTS")]
        [SerializeField] private UltEvent _onCancelAuthentication;
        [SerializeField] private UltEvent<RegisterPlayFabUserResult> _onRegisterSuccess;
        [SerializeField] private UltEvent<AddUsernamePasswordResult> _onAddAccountSuccess;
        [SerializeField] private UltEvent<LoginResult> _onLoginSuccess;
        [SerializeField] private UltEvent<PlayFabResultCommon> _onLinkSuccess;
        [SerializeField] private UltEvent<PlayFabResultCommon> _onUnlinkSuccess;
        [SerializeField] private UltEvent _onLogoutSuccess;
        [SerializeField] private UltEvent<PlayFabError> _onPlayFabError;

#if PHOTON_UNITY_NETWORKING
        [SerializeField] private UltEvent<GetPhotonAuthenticationTokenResult> _onPhotonTokenObtained;
#endif

        void OnEnable() {
            if (_data) {
                _data.RegisterEvents();
                _data.OnCancelAuthentication += _onCancelAuthentication.Invoke;
                _data.OnLoginSuccess += _onLoginSuccess.Invoke;
                _data.OnRegisterSuccess += _onRegisterSuccess.Invoke;
                _data.OnAddAccountSuccess += _onAddAccountSuccess.Invoke;
                _data.OnLinkSuccess += _onLinkSuccess.Invoke;
                _data.OnUnlinkSuccess += _onUnlinkSuccess.Invoke;
                _data.OnLogoutSuccess += _onLogoutSuccess.Invoke;
                _data.OnPlayFabError += _onPlayFabError.Invoke;
            }
        }

        void OnDisable() {
            if (_data) {
                _data.UnregisterEvents();
                _data.OnCancelAuthentication -= _onCancelAuthentication.Invoke;
                _data.OnLoginSuccess -= _onLoginSuccess.Invoke;
                _data.OnRegisterSuccess -= _onRegisterSuccess.Invoke;
                _data.OnAddAccountSuccess -= _onAddAccountSuccess.Invoke;
                _data.OnLinkSuccess -= _onLinkSuccess.Invoke;
                _data.OnUnlinkSuccess -= _onUnlinkSuccess.Invoke;
                _data.OnLogoutSuccess -= _onLogoutSuccess.Invoke;
                _data.OnPlayFabError -= _onPlayFabError.Invoke;
            }
        }

        [Button]
        public void Login(){
            if(_data) {
                _data.Login();
            }
        }

        [Button]
        public void Register() {
            if(_data) {
                _data.Register();
            }
        }

        [Button]
        public void Logout() {
            if(_data) {
                _data.Logout();
            }
        }
    }
}
#endif
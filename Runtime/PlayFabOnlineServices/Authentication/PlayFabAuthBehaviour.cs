#if PLAYFAB
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
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

        public string PhotonCustomAuthenticationToken => _data?.PhotonCustomAuthenticationToken;
#endif
        public string PlayFabId => _data?.Id;
        public string LoginResultDisplayName => _data?.LoginResultPayload?.PlayerProfile?.DisplayName;

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
#if PHOTON_UNITY_NETWORKING
                _data.OnPhotonTokenObtained += _onPhotonTokenObtained.Invoke;
#endif
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
#if PHOTON_UNITY_NETWORKING
                _data.OnPhotonTokenObtained -= _onPhotonTokenObtained.Invoke;
#endif
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
        public void Register() {
            if(_data) {
                _data.Register();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Logout() {
            if(_data) {
                _data.Logout();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetLogLevel() {
            if(_data) {
                _data.SetLogLevel();
            }
        }
    }
}
#endif
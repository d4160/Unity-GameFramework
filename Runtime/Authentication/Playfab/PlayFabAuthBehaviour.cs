#if PLAYFAB
using System;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using PlayFab.ClientModels;
using UltEvents;
using UnityEngine;

namespace d4160.Auth.PlayFab {
    public class PlayFabAuthBehaviour : MonoBehaviourUnityData<PlayFabAuthSO> {

        [Header ("EVENTS")]
        [SerializeField] private UltEvent _onAuthSuccess;
        [SerializeField] private UltEvent _onAuthFail;
        [SerializeField] private UltEvent<string> _onDisplayNameSaved;
        [SerializeField] private UltEvent _onContactEmailSaved;
        [SerializeField] private UltEvent _onContactEmailRemoved;

        void OnEnable() {
            if (_data) {
                _data.onAuthSuccess += _onAuthSuccess.Invoke;
                _data.onAuthFail += _onAuthFail.Invoke;
                _data.onDisplayNameSaved += _onDisplayNameSaved.Invoke;
                _data.onContactEmailSaved += _onContactEmailSaved.Invoke;
                _data.onContactEmailRemoved += _onContactEmailRemoved.Invoke;
            }
        }

        void OnDisable() {
            if (_data) {
                _data.onAuthSuccess -= _onAuthSuccess.Invoke;
                _data.onAuthFail -= _onAuthFail.Invoke;
                _data.onDisplayNameSaved -= _onDisplayNameSaved.Invoke;
                _data.onContactEmailSaved -= _onContactEmailSaved.Invoke;
                _data.onContactEmailRemoved -= _onContactEmailRemoved.Invoke;
            }
        }

        public void AuthenticateEmailPassword (string email, string password, GetPlayerCombinedInfoRequestParams infoRequestParams, bool rememberMe = false, bool forceLink = false, Action onAuthSuccess = null, Action onAuthFail = null) {
            _data.AuthenticateEmailPassword (email, password, infoRequestParams, rememberMe, forceLink, onAuthSuccess, onAuthFail);
        }

        public void RegisterPlayFabAccount (string email, string username, string password, string displayName, bool requireBothUsernameAndEmail = false, Action onAuthSuccess = null, Action onAuthFail = null) {
            _data.RegisterPlayFabAccount (email, username, password, displayName, requireBothUsernameAndEmail, onAuthSuccess, onAuthFail);
        }

        [Button]
        public void Unauthenticate () {
            _data.Unauthenticate ();
        }

        public void UpdateDisplayName (string displayName, Action onSuccess = null, Action onFail = null) {
            _data.UpdateDisplayName (displayName, onSuccess, onFail);
        }

        [Button]
        public void AddOrUpdateContactEmail () {
            _data.AddOrUpdateContactEmail ();
        }

        [Button]
        public void RemoveContactEmail () {
            _data.RemoveContactEmail ();
        }
    }
}
#endif
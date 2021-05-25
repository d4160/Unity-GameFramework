#if PLAYFAB
using System;
using System.Collections;
using System.Collections.Generic;
using d4160.Authentication;
using d4160.Coroutines;
using NaughtyAttributes;
using PlayFab.ClientModels;
using UltEvents;
using UnityEngine;
using UnityEngine.Promise;

namespace d4160.Auth.PlayFab {

    [CreateAssetMenu (menuName = "d4160/Authentication/PlayFab")]
    public class PlayFabAuthSO : ScriptableObject {

        [SerializeField] private PlayFabAuthTypes _authType;
        [SerializeField] private string _email;
        [SerializeField] private string _username;
        [SerializeField] private string _password;
        [SerializeField] private GetPlayerCombinedInfoRequestParams _infoRequestParams;

        [Space]
        [SerializeField] private bool _rememberMe;
        [Tooltip ("When remember me flag is true")]
        [SerializeField] private bool _forceLink;
        [SerializeField] private bool _requireBothUsernameAndEmail;

        [Space]
        [SerializeField] private string _displayName;

#if PHOTON_UNITY_NETWORKING
        [Space]
        [SerializeField] private bool _authenticateToPhotonAfterSuccess;
#endif

        public event Action onAuthSuccess;
        public event Action onAuthFail;
        public event Action<string> onDisplayNameSaved;
        public event Action onContactEmailSaved;
        public event Action onContactEmailRemoved;

        private readonly PlayFabAuthService _authService = PlayFabAuthService.Instance;

        public PlayFabAuthTypes PlayFabAuthTypes { get => _authType; set => _authType = value; }
        public string DisplayName { get => _displayName; set => _displayName = value; }
        public GetPlayerCombinedInfoRequestParams InfoRequestParams { get => _infoRequestParams; set => _infoRequestParams = value; }
#if PHOTON_UNITY_NETWORKING
        public bool AuthenticateToPhotonAfterSuccess { get => _authenticateToPhotonAfterSuccess; set => _authenticateToPhotonAfterSuccess = value; }
#endif

        [Button]
        public void Authenticate () {
            Authenticate (null, null);
        }

        public void Authenticate (Action onAuthSuccess, Action onAuthFail) {
            Authenticate (_authType, _email, _username, _password, _infoRequestParams, _rememberMe, _forceLink, _requireBothUsernameAndEmail, _displayName, onAuthSuccess, onAuthFail);
        }

        public void AuthenticateEmailPassword (string email, string password, GetPlayerCombinedInfoRequestParams infoRequestParams, bool rememberMe = false, bool forceLink = false, Action onAuthSuccess = null, Action onAuthFail = null) {
            _authType = PlayFabAuthTypes.EmailAndPassword;
            _email = email;
            _password = password;
            _infoRequestParams = infoRequestParams;
            _rememberMe = rememberMe;
            _forceLink = forceLink;

            Authenticate (onAuthSuccess, onAuthFail);
        }

        public void RegisterPlayFabAccount (string email, string username, string password, string displayName, bool requireBothUsernameAndEmail = false, Action onAuthSuccess = null, Action onAuthFail = null) {
            _authType = PlayFabAuthTypes.RegisterPlayFabAccount;
            _email = email;
            _password = password;
            _username = username;
            _displayName = displayName;
            _requireBothUsernameAndEmail = requireBothUsernameAndEmail;

            Authenticate (onAuthSuccess, onAuthFail);
        }

        public void Authenticate (PlayFabAuthTypes authType, string email, string username, string password, GetPlayerCombinedInfoRequestParams infoRequestParams, bool rememberMe, bool forceLink, bool requireBothUsernameAndEmail, string displayName, Action onAuthSuccess = null, Action onAuthFail = null) {
            _authService.AuthType = authType;
            _authService.email = email;
            _authService.username = username;
            _authService.password = password;
            _authService.infoRequestParams = infoRequestParams;
            _authService.RememberMe = rememberMe;
            _authService.forceLink = forceLink;
            _authService.requireBothUsernameAndEmail = requireBothUsernameAndEmail;
            _authService.SetDisplayName (_displayName);
#if PHOTON_UNITY_NETWORKING
            _authService.authenticateToPhotonAfterSuccess = _authenticateToPhotonAfterSuccess;
#endif

            AuthenticateInternal (onAuthSuccess, onAuthFail);
        }

        private void AuthenticateInternal (Action onAuthSuccess = null, Action onAuthFail = null) {
            Deferred def = GameAuthSdk.Authenticate (_authService);

            if (def.isDone) {
                if (def.isFulfilled) {
                    onAuthSuccess?.Invoke ();
                    onAuthSuccess?.Invoke ();
                } else {
                    Debug.LogError (def.error.Message);
                    onAuthFail?.Invoke ();
                    onAuthFail?.Invoke ();
                }
            } else {
                IEnumerator Routine (Deferred aDef) {
                    yield return aDef.Wait ();

                    if (aDef.isFulfilled) {
                        onAuthSuccess?.Invoke ();
                        onAuthSuccess?.Invoke ();
                    } else {
                        Debug.LogError (aDef.error.Message);
                        onAuthFail?.Invoke ();
                        onAuthFail?.Invoke ();
                    }
                }

                Routine (def).StartCoroutine ();
            }
        }

        [Button]
        public void Unauthenticate () {
            GameAuthSdk.Unauthenticate ();
        }

        [Button]
        public void UpdateDisplayName () {
            UpdateDisplayName (_displayName, null, null);
        }

        public void UpdateDisplayName (Action onSuccess, Action onFail) {
            UpdateDisplayName (_displayName, onSuccess, onFail);
        }

        public void UpdateDisplayName (string displayName, Action onSuccess = null, Action onFail = null) {

            _authService.UpdateDisplayName (displayName,
                (r) => {
                    Debug.Log ($"DisplayName updated to: {r.DisplayName}");
                    onSuccess?.Invoke ();
                    onDisplayNameSaved?.Invoke (r.DisplayName);
                }, (e) => {
                    Debug.LogError (e.GenerateErrorReport ());
                    onFail?.Invoke ();
                });
        }

        [Button]
        public void GetDisplayName () {
            GetDisplayName (null);
        }

        public void GetDisplayName (Action<string> onSuccess, Action onFail = null) {
            _authService.GetDisplayName ((r) => {
                onSuccess?.Invoke (r);
                _displayName = r;
            }, (e) => {
                Debug.Log (e.ErrorMessage);
                onFail?.Invoke ();
            });
        }

        [Button]
        public void AddOrUpdateContactEmail () {
            AddOrUpdateContactEmail (_email, null, null);
        }

        public void AddOrUpdateContactEmail (Action onSuccess, Action onFail) {
            AddOrUpdateContactEmail (_email, onSuccess, onFail);
        }

        public void AddOrUpdateContactEmail (string email, Action onSuccess = null, Action onFail = null) {

            _authService.email = email;
            _authService.AddOrUpdateContactEmail ((r) => {
                Debug.Log ($"Contact email saved: {r.ToJson()}");
                onSuccess?.Invoke ();
                onContactEmailSaved?.Invoke ();
            }, (e) => {
                Debug.LogError (e.GenerateErrorReport ());
                onFail?.Invoke ();
            });
        }

        [Button]
        public void RemoveContactEmail () {
            RemoveContactEmail (null, null);
        }

        public void RemoveContactEmail (Action onSuccess, Action onFail = null) {

            _authService.RemoveContactEmail ((r) => {
                Debug.Log ($"Contact email removed: {r.ToJson()}");
                onSuccess?.Invoke ();
                onContactEmailSaved?.Invoke ();
            }, (e) => {
                Debug.LogError (e.GenerateErrorReport ());
                onFail?.Invoke ();
            });
        }
    }
}
#endif
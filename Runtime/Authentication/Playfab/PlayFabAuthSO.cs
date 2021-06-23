#if PLAYFAB
using System;
using System.Diagnostics;
using d4160.Authentication;
using d4160.Core;
using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;

namespace d4160.Auth.PlayFab {

        [CreateAssetMenu (menuName = "d4160/Authentication/PlayFab")]
        public class PlayFabAuthSO : ScriptableObject {

                [SerializeField] private AuthType _authType;
                [ShowIf ("_authType", AuthType.Login)]
                [SerializeField] private PlayFabLoginType _loginType;
                [ShowIf ("_authType", AuthType.Register)]
                [SerializeField] private PlayFabRegisterType _registerType;

                [ShowIf (EConditionOperator.Or, "IsEmailAndPasswordAuthType", "IsRegisterPlayFabAccountAuthType", "IsAddPlayFabAccountAuthType")]
                [SerializeField] private string _email;
                [ShowIf (EConditionOperator.Or, "IsUsernameAndPasswordAuthType", "IsRegisterPlayFabAccountAuthType", "IsAddPlayFabAccountAuthType")]
                [SerializeField] private string _username;
                [ShowIf ("IsRegisterPlayFabAccountAuthType")]
                [SerializeField] private bool _requireBothUsernameAndEmail;
                [ShowIf (EConditionOperator.Or, "IsEmailUsernameAddPlayFabAccountAuthTypes", "IsRegisterPlayFabAccountAuthType")]
                [SerializeField] private string _password;
                [ShowIf (EConditionOperator.Or, "IsEmailAndPasswordAuthType", "IsUsernameAndPasswordAuthType", "IsSilentAuthType")]
                [SerializeField] private GetPlayerCombinedInfoRequestParams _infoRequestParams;

                [Space]
                [ShowIf ("IsEmailUsernameAddPlayFabAccountAuthTypes")]
                [SerializeField] private bool _rememberMe;
                [Tooltip ("When remember me flag is true")]
                [ShowIf (EConditionOperator.And, "_rememberMe", "IsEmailUsernameAddPlayFabAccountAuthTypes")]
                [SerializeField] private bool _forceLink;

                [Space]
                [ShowIf ("IsRegisterPlayFabAccountAuthType")]
                [SerializeField] private string _displayName;

#if PHOTON_UNITY_NETWORKING
                [Space]
                [SerializeField] private bool _authenticateToPhotonAfterSuccess;
#endif
                [Header ("AFTER LOGIN")]
                [SerializeField] private GetPlayerCombinedInfoResultPayload _loginResultPayload;

                [Header ("DEBUG")]
                [SerializeField] private LogLevelType _logLevel = LogLevelType.Debug;

#if UNITY_EDITOR
                private bool IsEmailUsernameAddPlayFabAccountAuthTypes => IsEmailAndPasswordAuthType || IsUsernameAndPasswordAuthType || IsAddPlayFabAccountAuthType;
                private bool IsEmailAndPasswordAuthType => _authType == AuthType.Login && _loginType == PlayFabLoginType.EmailAndPassword;
                private bool IsUsernameAndPasswordAuthType => _authType == AuthType.Login && _loginType == PlayFabLoginType.UsernameAndPassword;
                private bool IsRegisterPlayFabAccountAuthType => _authType == AuthType.Register && _registerType == PlayFabRegisterType.RegisterPlayFabAccount;
                private bool IsAddPlayFabAccountAuthType => _authType == AuthType.Register && _registerType == PlayFabRegisterType.AddPlayFabAccount;
                private bool IsSilentAuthType => _authType == AuthType.Login && _loginType == PlayFabLoginType.Silent;
#endif

                public event Action OnCancelAuthentication;
                public event Action<RegisterPlayFabUserResult> OnRegisterSuccess;
                public event Action<AddUsernamePasswordResult> OnAddAccountSuccess;
                public event Action<LoginResult> OnLoginSuccess;
                public event Action<PlayFabResultCommon> OnLinkSuccess;
                public event Action<PlayFabResultCommon> OnUnlinkSuccess;
                public event Action OnLogoutSuccess;
                public event Action<PlayFabError> OnPlayFabError;

#if PHOTON_UNITY_NETWORKING
                public event Action<GetPhotonAuthenticationTokenResult> OnPhotonTokenObtained;
#endif

                private readonly PlayFabAuthService _authService = PlayFabAuthService.Instance;

                public string Id => _authService.Id;
                public PlayFabLoginType LoginType { get => _loginType; set => _loginType = value; }
                public PlayFabRegisterType RegisterType { get => _registerType; set => _registerType = value; }
                public string DisplayName { get => _displayName; set => _displayName = value; }
                public string Email { get => _email; set => _email = value; }
                public string Username { get => _username; set => _username = value; }
                public string Password { get => _password; set => _password = value; }
                public bool RequireBothUsernameAndEmail { get => _requireBothUsernameAndEmail; set => _requireBothUsernameAndEmail = value; }
                public GetPlayerCombinedInfoResultPayload LoginResultPayload => _loginResultPayload;
                public GetPlayerCombinedInfoRequestParams InfoRequestParams { get => _infoRequestParams; set => _infoRequestParams = value; }
#if PHOTON_UNITY_NETWORKING
                public bool AuthenticateToPhotonAfterSuccess { get => _authenticateToPhotonAfterSuccess; set => _authenticateToPhotonAfterSuccess = value; }
                public string PhotonCustomAuthenticationToken => _authService.PhotonCustomAuthenticationToken;
#endif

                private void CallOnCancelAuthentication () => OnCancelAuthentication?.Invoke ();
                private void CallOnLoginSuccess (LoginResult result) => OnLoginSuccess?.Invoke (result);
                private void CallOnRegisterSuccess (RegisterPlayFabUserResult result) => OnRegisterSuccess?.Invoke (result);
                private void CallOnAddAccountSuccess (AddUsernamePasswordResult result) => OnAddAccountSuccess?.Invoke (result);
                private void CallOnLinkSuccess (PlayFabResultCommon result) => OnLinkSuccess?.Invoke (result);
                private void CallOnUnlinkSuccess (PlayFabResultCommon result) => OnUnlinkSuccess?.Invoke (result);
                private void CallOnLogoutSuccess () => OnLogoutSuccess?.Invoke ();
                private void CallOnPlayFabError (PlayFabError error) => OnPlayFabError?.Invoke (error);

#if PHOTON_UNITY_NETWORKING
                private void CallOnPhotonTokenObtained (GetPhotonAuthenticationTokenResult result) => OnPhotonTokenObtained?.Invoke (result);
#endif

        public void RegisterEvents () {
                PlayFabAuthService.OnCancelAuthentication += CallOnCancelAuthentication;
                PlayFabAuthService.OnLoginSuccess += CallOnLoginSuccess;
                PlayFabAuthService.OnRegisterSuccess += CallOnRegisterSuccess;
                PlayFabAuthService.OnAddAccountSuccess += CallOnAddAccountSuccess;
                PlayFabAuthService.OnLinkSuccess += CallOnLinkSuccess;
                PlayFabAuthService.OnUnlinkSuccess += CallOnUnlinkSuccess;
                PlayFabAuthService.OnLogoutSuccess += CallOnLogoutSuccess;
                PlayFabAuthService.OnPlayFabError += CallOnPlayFabError;

#if PHOTON_UNITY_NETWORKING
                PlayFabAuthService.OnPhotonTokenObtained += CallOnPhotonTokenObtained;
#endif
        }

        public void UnregisterEvents () {
                PlayFabAuthService.OnCancelAuthentication -= CallOnCancelAuthentication;
                PlayFabAuthService.OnLoginSuccess -= CallOnLoginSuccess;
                PlayFabAuthService.OnRegisterSuccess -= CallOnRegisterSuccess;
                PlayFabAuthService.OnAddAccountSuccess -= CallOnAddAccountSuccess;
                PlayFabAuthService.OnLinkSuccess -= CallOnLinkSuccess;
                PlayFabAuthService.OnUnlinkSuccess -= CallOnUnlinkSuccess;
                PlayFabAuthService.OnLogoutSuccess -= CallOnLogoutSuccess;
                PlayFabAuthService.OnPlayFabError -= CallOnPlayFabError;

#if PHOTON_UNITY_NETWORKING
                PlayFabAuthService.OnPhotonTokenObtained -= CallOnPhotonTokenObtained;
#endif
        }

        [Button]
        public void Login () {
                _authService.LoginType = _loginType;
                _authService.email = _email;
                _authService.username = _username;
                _authService.password = _password;
                _authService.infoRequestParams = _infoRequestParams;
                _authService.RememberMe = _rememberMe;
                _authService.forceLink = _forceLink;
#if PHOTON_UNITY_NETWORKING
                _authService.authenticateToPhotonAfterSuccess = _authenticateToPhotonAfterSuccess;
#endif
                AuthManager.Login (_authService);
                AuthManager.OnLoggedIn += OnLoggedIn;
        }

        private void OnLoggedIn () {
                _loginResultPayload = _authService.LoginResultPayload;
                // UnityEngine.Debug.Log (_loginResultPayload.TitleData.Count);
                AuthManager.OnLoggedIn -= OnLoggedIn;
        }

                [Button]
                public void Register () {
                        _authService.RegisterType = _registerType;
                        _authService.email = _email;
                        _authService.username = _username;
                        _authService.password = _password;
                        _authService.requireBothUsernameAndEmail = _requireBothUsernameAndEmail;
                        _authService.SetDisplayName (_displayName);
        #if PHOTON_UNITY_NETWORKING
                        _authService.authenticateToPhotonAfterSuccess = _authenticateToPhotonAfterSuccess;
        #endif

                        AuthManager.Register (_authService);
                }

                [Button]
                public void Logout () {
                        AuthManager.Logout ();
                }

                [Button]
                public void SetLogLevel () {
                        AuthManager.LogLevel = _logLevel;
                }
        }
}
#endif
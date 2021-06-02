#if PLAYFAB
using System;
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
        [ShowIf("_authType", AuthType.Login)]
        [SerializeField] private PlayFabLoginType _loginType;
        [ShowIf("_authType", AuthType.Register)]
        [SerializeField] private PlayFabRegisterType _registerType;

        [ShowIf(EConditionOperator.Or, "IsEmailAndPasswordAuthType", "IsRegisterPlayFabAccountAuthType", "IsAddPlayFabAccountAuthType")]
        [SerializeField] private string _email;
        [ShowIf(EConditionOperator.Or, "IsUsernameAndPasswordAuthType", "IsRegisterPlayFabAccountAuthType", "IsAddPlayFabAccountAuthType")]
        [SerializeField] private string _username;
        [ShowIf("IsRegisterPlayFabAccountAuthType")]
        [SerializeField] private bool _requireBothUsernameAndEmail;
        [ShowIf(EConditionOperator.Or, "IsEmailUsernameAddPlayFabAccountAuthTypes", "IsRegisterPlayFabAccountAuthType")]
        [SerializeField] private string _password;
        [ShowIf(EConditionOperator.Or, "IsEmailAndPasswordAuthType", "IsUsernameAndPasswordAuthType", "IsSilentAuthType")]
        [SerializeField] private GetPlayerCombinedInfoRequestParams _infoRequestParams;

        [Space]
        [ShowIf("IsEmailUsernameAddPlayFabAccountAuthTypes")]
        [SerializeField] private bool _rememberMe;
        [Tooltip ("When remember me flag is true")]
        [ShowIf(EConditionOperator.And, "_rememberMe", "IsEmailUsernameAddPlayFabAccountAuthTypes")]
        [SerializeField] private bool _forceLink;
    
        [Space]
        [ShowIf("IsRegisterPlayFabAccountAuthType")]
        [SerializeField] private string _displayName;

#if PHOTON_UNITY_NETWORKING
        [Space]
        [SerializeField] private bool _authenticateToPhotonAfterSuccess;
#endif

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

        public PlayFabLoginType LoginType { get => _loginType; set => _loginType = value; }
        public string DisplayName { get => _displayName; set => _displayName = value; }
        public string Email { get => _email; set => _email = value; }
        public string Password { get => _password; set => _password = value; }
        public GetPlayerCombinedInfoRequestParams InfoRequestParams { get => _infoRequestParams; set => _infoRequestParams = value; }
#if PHOTON_UNITY_NETWORKING
        public bool AuthenticateToPhotonAfterSuccess { get => _authenticateToPhotonAfterSuccess; set => _authenticateToPhotonAfterSuccess = value; }
#endif

        public void RegisterEvents() {
            PlayFabAuthService.OnCancelAuthentication += OnCancelAuthentication.Invoke;
            PlayFabAuthService.OnLoginSuccess += OnLoginSuccess.Invoke;
            PlayFabAuthService.OnRegisterSuccess += OnRegisterSuccess.Invoke;
            PlayFabAuthService.OnAddAccountSuccess += OnAddAccountSuccess.Invoke;
            PlayFabAuthService.OnLinkSuccess += OnLinkSuccess.Invoke;
            PlayFabAuthService.OnUnlinkSuccess += OnUnlinkSuccess.Invoke;
            PlayFabAuthService.OnLogoutSuccess += OnLogoutSuccess.Invoke;
            PlayFabAuthService.OnPlayFabError += OnPlayFabError.Invoke;

#if PHOTON_UNITY_NETWORKING
            PlayFabAuthService.OnPhotonTokenObtained += OnPhotonTokenObtained.Invoke;
#endif
        }

        public void UnregisterEvents() {
            PlayFabAuthService.OnCancelAuthentication -= OnCancelAuthentication.Invoke;
            PlayFabAuthService.OnLoginSuccess -= OnLoginSuccess.Invoke;
            PlayFabAuthService.OnRegisterSuccess -= OnRegisterSuccess.Invoke;
            PlayFabAuthService.OnAddAccountSuccess -= OnAddAccountSuccess.Invoke;
            PlayFabAuthService.OnLinkSuccess -= OnLinkSuccess.Invoke;
            PlayFabAuthService.OnUnlinkSuccess -= OnUnlinkSuccess.Invoke;
            PlayFabAuthService.OnLogoutSuccess -= OnLogoutSuccess.Invoke;
            PlayFabAuthService.OnPlayFabError -= OnPlayFabError.Invoke;

#if PHOTON_UNITY_NETWORKING
            PlayFabAuthService.OnPhotonTokenObtained -= OnPhotonTokenObtained.Invoke;
#endif
        }

        [Button]
        public void Login(){
            _authService.LoginType = _loginType;
            _authService.email = _email;
            _authService.username = _username;
            _authService.password = _password;
            _authService.infoRequestParams = _infoRequestParams;
            _authService.RememberMe = _rememberMe;
            _authService.forceLink = _forceLink;

            AuthManager.Login(_authService);
        }

        [Button]
        public void Register() {
            _authService.RegisterType = _registerType;
            _authService.email = _email;
            _authService.username = _username;
            _authService.password = _password;
            _authService.requireBothUsernameAndEmail = _requireBothUsernameAndEmail;
            _authService.SetDisplayName(_displayName);

            AuthManager.Register(_authService);
        }

        [Button]
        public void Logout() {
            AuthManager.Logout();
        }
    }
}
#endif
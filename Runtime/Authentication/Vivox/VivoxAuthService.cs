#if VIVOX
using System;
using System.ComponentModel;
using d4160.Authentication;
using d4160.Core;
using UnityEngine;
using UnityEngine.Promise;
using VivoxUnity;
using M31Logger = d4160.Logging.M31Logger;

namespace d4160.Auth.Vivox
{
    public class VivoxAuthService : BaseAuthService
    {
        public static event Action<object, PropertyChangedEventArgs> OnLoginSessionPropertyChanged;
        public static event Action OnLoginSuccess;
        public static event Action OnLogoutSuccess;
        public static event Action<Exception> OnAuthError;

        private const string VivoxDisplayNameKey = "VivoxDisplayNameKey";

        public override string DisplayName
        {
            get
            {
                if (!PlayerPrefs.HasKey(VivoxDisplayNameKey)) return _displayName;

                _displayName = PlayerPrefs.GetString(VivoxDisplayNameKey);

                return _displayName;
            }
            set
            {
                _displayName = value;

                PlayerPrefs.SetString(VivoxDisplayNameKey, value);
            }
        }

        private ILoginSession _loginSession;
        private string _displayName;
        private Completer _completer;
        private AccountId _accountId;
        private string _id;

        public VivoxAuthSettingsSO AuthSettings { get; set; }
        public ILoginSession LoginSession => _loginSession;
        public LoginState LoginState { get; private set; }
        public AccountId AccountId => _accountId;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public override bool HasSession => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(SessionTicket);
        public override string Id { get => _accountId.ToString(); set => _id = value; }
        public override string SessionTicket => _loginSession.LoginSessionId.ToString();

        public static Client Client {
            get {
                if (_client == null)
                {
                    if (!Application.isPlaying) {
                        M31Logger.LogWarning("VIVOX: Client can only be set in playing mode", Instance.LogLevel);
                        return _client;
                    }
                    _client = new Client();

                    _client.Uninitialize();
                    _client.Initialize();
                }

                return _client;
            }
        }
        public static VivoxAuthService Instance => _instance ?? (_instance = new VivoxAuthService());

        private static VivoxAuthService _instance;
        private static Client _client;
        
        private VivoxAuthService()
        {
            _instance = this;
        }

        public override void Login(Completer completer)
        {
            if (!Application.isPlaying) {
                M31Logger.LogWarning("VIVOX: Only can use this function in playing mode", LogLevel);
                return;
            }

            if (!AuthSettings)
            {
                M31Logger.LogWarning("VIVOX: Auth Settings is null, set the property before authenticate.", LogLevel);
                return;
            }

            _completer = completer;

            string uniqueId = string.IsNullOrEmpty(_id) ? Guid.NewGuid().ToString() : _id;
            string displayName = string.IsNullOrEmpty(DisplayName) ? "Maranatha 2031" : DisplayName;

            // Logger.LogInfo($"VIVOX: AccountId: TokenIssuer: {AuthSettings.TokenIssuer}, UniqueId: {uniqueId}, Domain: {AuthSettings.Domain}, DisplayName: {displayName}");

            //for proto purposes only, need to get a real token from server eventually
            _accountId = new AccountId(AuthSettings.TokenIssuer, uniqueId, AuthSettings.Domain, displayName);
            _loginSession = Client.GetLoginSession(_accountId);
            _loginSession.PropertyChanged += OnLoginSessionPropertyChangedCallback;
            _loginSession.PropertyChanged += CallOnLoginSessionPropertyChanged;

            _loginSession.BeginLogin(AuthSettings.ServerUri, _loginSession.GetLoginToken(AuthSettings.TokenKey, AuthSettings.TokenExpiration),
                SubscriptionMode.Accept, null, null, null, ar =>
                {
                    try
                    {
                        _loginSession.EndLogin(ar);
                    }
                    catch (Exception e)
                    {
                        // Handle error 
                        VivoxLogError(nameof(e));
                        OnAuthError?.Invoke(e);
                        // Unbind if we failed to login.
                        _loginSession.PropertyChanged -= OnLoginSessionPropertyChangedCallback;
                        _loginSession.PropertyChanged -= CallOnLoginSessionPropertyChanged;
                        return;
                    }
                });
        }

        public override void Logout(Completer completer)
        {
            if (!Application.isPlaying) {
                M31Logger.LogWarning("VIVOX: Only can use this function in playing mode", Instance.LogLevel);
                return;
            }

            if (_loginSession != null && LoginState != LoginState.LoggedOut && LoginState != LoginState.LoggingOut)
            {
                OnLogoutSuccess?.Invoke();
                _loginSession.PropertyChanged -= OnLoginSessionPropertyChangedCallback;
                _loginSession.PropertyChanged -= CallOnLoginSessionPropertyChanged;
                _loginSession.Logout();

                VivoxLog("Logged out");
            }
        }

        private void CallOnLoginSessionPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            OnLoginSessionPropertyChanged?.Invoke(sender, propertyChangedEventArgs);
        }

        private void OnLoginSessionPropertyChangedCallback(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "State")
            {
                return;
            }

            var loginSession = (ILoginSession)sender;
            LoginState = loginSession.State;
            VivoxLog("Detecting login session change");
            switch (LoginState)
            {
                case LoginState.LoggingIn:
                {
                    VivoxLog("Logging in");
                    break;
                }
                case LoginState.LoggedIn:
                {
                    VivoxLog("Connected to voice server and logged in.");
                    OnLoginSuccess?.Invoke();
                    break;
                }
                case LoginState.LoggingOut:
                {
                    VivoxLog("Logging out");
                    break;
                }
                case LoginState.LoggedOut:
                {
                    VivoxLog("Logged out");
                    _loginSession.PropertyChanged -= OnLoginSessionPropertyChangedCallback;
                    _loginSession.PropertyChanged -= CallOnLoginSessionPropertyChanged;
                    break;
                }
                default:
                    break;
            }
        }

        private void VivoxLog(string msg)
        {
            Debug.Log("<color=green>VivoxVoice: </color>: " + msg);
        }

        private void VivoxLogError(string msg)
        {
            Debug.LogError("<color=green>VivoxVoice: </color>: " + msg);
        }

        public override void Register(Completer completer)
        {
            completer.Reject(new Exception("Vivox cannot support register of new users"));
        }

        public void CleanUp()
        {
            // Needed to add this to prevent some unsuccessful uninit, we can revisit to do better -carlo
            Client.Cleanup();
            if (_client != null)
            {
                VivoxLog("Uninitializing client.");
                _client.Uninitialize();
                _client = null;
            }
        }
    }
}
#endif
#if VIVOX
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Promise;
using VivoxUnity;

namespace d4160.GameFramework.Authentication
{
    public class VivoxAuthService : IAuthService
    {
        public delegate void LoginStatusChangedHandler();

        public static event LoginStatusChangedHandler OnUserLoggedInEvent;
        public static event LoginStatusChangedHandler OnUserLoggedOutEvent;

        private const string VivoxDisplayNameKey = "VivoxDisplayNameKey";

        public string DisplayName
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

        public string Id { get; set; }
        public ILoginSession LoginSession;

        private string _displayName;
        private Completer _completer;
        private AccountId _accountId;

        public Client Client { get; set; }
        public VivoxAuthSettings AuthSettings { get; set; }
        public LoginState LoginState { get; private set; }
        public AccountId AccountId => _accountId;
        
        public static VivoxAuthService Instance => _instance ?? (_instance = new VivoxAuthService());

        private static VivoxAuthService _instance;

        public VivoxAuthService()
        {
            _instance = this;
        }

        public void Authenticate()
        {
            if (!AuthSettings)
            {
                Debug.LogError("Vivox Auth Settings is null, set the property before authenticate.");
                return;
            }

            string uniqueId = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToString() : Id;
            string displayName = string.IsNullOrEmpty(DisplayName) ? "Bot 2031" : DisplayName;

            //for proto purposes only, need to get a real token from server eventually
            _accountId = new AccountId(AuthSettings.TokenIssuer, uniqueId, AuthSettings.Domain, displayName);
            LoginSession = Client.GetLoginSession(_accountId);
            LoginSession.PropertyChanged += OnLoginSessionPropertyChanged;
            LoginSession.BeginLogin(AuthSettings.ServerUri, LoginSession.GetLoginToken(AuthSettings.TokenKey, AuthSettings.TokenExpiration),
                SubscriptionMode.Accept, null, null, null, ar =>
                {
                    try
                    {
                        LoginSession.EndLogin(ar);
                    }
                    catch (Exception e)
                    {
                        // Handle error 
                        VivoxLogError(nameof(e));
                        // Unbind if we failed to login.
                        LoginSession.PropertyChanged -= OnLoginSessionPropertyChanged;
                        return;
                    }
                });
        }

        public void Authenticate(Completer completer)
        {
            _completer = completer;
        }

        public void Unauthenticate()
        {
            if (LoginSession != null && LoginState != LoginState.LoggedOut && LoginState != LoginState.LoggingOut)
            {
                OnUserLoggedOutEvent?.Invoke();
                LoginSession.PropertyChanged -= OnLoginSessionPropertyChanged;
                LoginSession.Logout();

                VivoxLog("Logged out");
            }
        }

        private void OnLoginSessionPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
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
                    OnUserLoggedInEvent?.Invoke();
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
                    LoginSession.PropertyChanged -= OnLoginSessionPropertyChanged;
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
    }
}
#endif
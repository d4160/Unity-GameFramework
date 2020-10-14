#if PHOTON_UNITY_NETWORKING
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Promise;

namespace d4160.GameFramework.Authentication
{
    public class PhotonAuthService : IAuthService
    {
        public delegate void DisplayAuthenticationEvent();

        public static event DisplayAuthenticationEvent OnDisplayAuthentication;

        public static event Action OnAuthSuccess;

        public static event Action<Exception> OnAuthError;

        public string Token;
        public string CustomServiceId;

        public static string AppIdRealtime => PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;

        private const string PhotonAuthTypeKey = "PhotonAuthType";
        private const string PhotonNickNameKey = "PhotonNickNameKey";

        public string DisplayName
        {
            get
            {
                if (!PlayerPrefs.HasKey(PhotonNickNameKey)) return PhotonNetwork.NickName;
                
                var nickname = PlayerPrefs.GetString(PhotonNickNameKey);
                PhotonNetwork.NickName = nickname;

                return nickname;
            }
            set
            {
                PhotonNetwork.NickName = value;

                PlayerPrefs.SetString(PhotonNickNameKey, value);
            }
        }

        public string Id => Token;
        
        private Completer _completer;

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public CustomAuthenticationType AuthType
        {
            get => (CustomAuthenticationType)PlayerPrefs.GetInt(PhotonAuthTypeKey, 0);
            set => PlayerPrefs.SetInt(PhotonAuthTypeKey, (int)value);
        }

        public static PhotonAuthService Instance => _instance ?? (_instance = new PhotonAuthService());

        private static PhotonAuthService _instance;

        private PhotonAuthService()
        {
            _instance = this;
        }

        public void Authenticate()
        {
            var authType = AuthType;

            switch (authType)
            {
                case CustomAuthenticationType.None:
                    OnDisplayAuthentication?.Invoke();
                    break;
                case CustomAuthenticationType.Custom:
                    AuthenticateWithCustomService();
                    break;
                default:
                    OnDisplayAuthentication?.Invoke();
                    break;
            }
        }

        public void Authenticate(Completer completer)
        {
            var authType = AuthType;

            _completer = completer;

            switch (authType)
            {
                case CustomAuthenticationType.None:
                    OnDisplayAuthentication?.Invoke();
                    break;
                case CustomAuthenticationType.Custom:
                    AuthenticateWithCustomService();
                    break;
                default:
                    OnDisplayAuthentication?.Invoke();
                    break;
            }
        }

        protected void AuthenticateWithCustomService()
        {
            if (string.IsNullOrEmpty(CustomServiceId) || string.IsNullOrEmpty(Token))
            {
                Exception e = new Exception("CustomId or Token is empty. Can't authenticate with null values.");
                _completer.Reject(e);
                OnAuthError?.Invoke(e);

                return;
            }

            //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
            var customAuth = new AuthenticationValues {AuthType = AuthType};
            //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
            customAuth.AddAuthParameter("username", CustomServiceId); // expected by PlayFab custom auth service
            //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
            customAuth.AddAuthParameter("token", Token);

            //We finally tell Photon to use this authentication parameters throughout the entire application.
            PhotonNetwork.AuthValues = customAuth;

            _completer.Resolve();

            OnAuthSuccess?.Invoke();
        }

        public void Unauthenticate()
        {
            Token = null;
            CustomServiceId = null;
        }
    }
}
#endif
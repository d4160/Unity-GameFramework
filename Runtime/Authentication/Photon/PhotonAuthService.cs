#if PHOTON_UNITY_NETWORKING
using System;
using d4160.Authentication;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Promise;

namespace d4160.Auth.Photon {
    public class PhotonAuthService : BaseAuthService {
        public static event Action OnCancelAuthentication;
        public static event Action OnLoginSuccess;
        public static event Action OnLogoutSuccess;
        public static event Action<Exception> OnAuthError;

        public Player LocalPlayer => PhotonNetwork.LocalPlayer;

        private string _token;
        private string _customServiceId;
        private Completer _completer;

        public static string AppIdRealtime => PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;

        private const string PhotonAuthTypeKey = "PhotonAuthType";
        private const string PhotonNickNameKey = "PhotonNickNameKey";

        public override string DisplayName {
            get {
                if (!PlayerPrefs.HasKey (PhotonNickNameKey)) return PhotonNetwork.NickName;

                var nickname = PlayerPrefs.GetString (PhotonNickNameKey);
                PhotonNetwork.NickName = nickname;

                return nickname;
            }
            set {
                PlayerPrefs.SetString (PhotonNickNameKey, value);

                if (Application.isPlaying) {
                    PhotonNetwork.NickName = value;

                    Debug.Log ("Nickname is set correctly");
                } else {
                    Debug.LogWarning ("The nickname are saved in PlayerPrefs, but Photon cannot work in edit mode");
                }
            }
        }
        public override string Id { get => _customServiceId; set => _customServiceId = value; }
        public override string SessionTicket { get => _token; set => _token = value; }

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public CustomAuthenticationType AuthType {
            get => (CustomAuthenticationType) PlayerPrefs.GetInt (PhotonAuthTypeKey, 0);
            set => PlayerPrefs.SetInt (PhotonAuthTypeKey, (int) value);
        }

        public static PhotonAuthService Instance => _instance ?? (_instance = new PhotonAuthService ());
        private static PhotonAuthService _instance;

        private PhotonAuthService () {
            _instance = this;
        }

        public void Login () {
            switch (AuthType) {
                case CustomAuthenticationType.None:
                    OnCancelAuthentication?.Invoke ();
                    break;
                case CustomAuthenticationType.Custom:
                    AuthenticateWithCustomService ();
                    break;
                default:
                    OnCancelAuthentication?.Invoke ();
                    break;
            }
        }

        public override void Login (Completer completer) {
            _completer = completer;

            switch (AuthType) {
                case CustomAuthenticationType.None:
                    OnCancelAuthentication?.Invoke ();
                    break;
                case CustomAuthenticationType.Custom:
                    AuthenticateWithCustomService ();
                    break;
                default:
                    OnCancelAuthentication?.Invoke ();
                    break;
            }
        }

        public override void Register (Completer completer) {
            Exception exc = new Exception ("Photon cannot manage register of users.");
            completer.Reject (exc);
            OnAuthError?.Invoke (exc);
        }

        protected void AuthenticateWithCustomService () {
            if (string.IsNullOrEmpty (_customServiceId) || string.IsNullOrEmpty (_token)) {
                Exception e = new Exception ("CustomId or Token is empty. Can't authenticate with null values.");
                _completer.Reject (e);
                OnAuthError?.Invoke (e);

                return;
            }

            //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
            var customAuth = new AuthenticationValues { AuthType = AuthType };
            //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
            customAuth.AddAuthParameter ("username", _customServiceId); // expected by PlayFab custom auth service
            //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
            customAuth.AddAuthParameter ("token", _token);

            //We finally tell Photon to use this authentication parameters throughout the entire application.
            PhotonNetwork.AuthValues = customAuth;

            _completer.Resolve ();
            OnLoginSuccess?.Invoke ();
        }

        public override void Logout (Completer completer) {
            PhotonNetwork.AuthValues = null;
            _token = null;
            _customServiceId = null;

            OnLogoutSuccess?.Invoke ();
        }
    }
}
#endif
#if PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using LoginResult = PlayFab.ClientModels.LoginResult;
using PlayFab.SharedModels;
using System;
using d4160.Authentication;

#if PHOTON_UNITY_NETWORKING
using d4160.Auth.Photon;
using Photon.Realtime;
#endif

using UnityEngine.Promise;

#if FACEBOOK
using Facebook.Unity;
#endif

namespace d4160.Auth.PlayFab {
    /// <summary>
    /// Supported Authentication types
    /// Note: Add types to there to support more AuthTypes
    /// See - https://api.playfab.com/documentation/client#Authentication
    /// </summary>
    public enum PlayFabLoginType {
        None,
        Silent,
        UsernameAndPassword,
        EmailAndPassword,
        Steam,
        Facebook,
        Google
    }

    public enum PlayFabRegisterType {
        None,
        RegisterPlayFabAccount,
        AddPlayFabAccount
    }

    public enum PlayFabLinkType {
        None,
        Device,
        Steam,
        Facebook,
        Google
    }

    public class PlayFabAuthService : BaseAuthService 
    {
        public static event Action OnCancelAuthentication;
        public static event Action<RegisterPlayFabUserResult> OnRegisterSuccess;
        public static event Action<AddUsernamePasswordResult> OnAddAccountSuccess;
        public static event Action<LoginResult> OnLoginSuccess;
        public static event Action<PlayFabResultCommon> OnLinkSuccess;
        public static event Action<PlayFabResultCommon> OnUnlinkSuccess;
        public static event Action OnLogoutSuccess;
        public static event Action<PlayFabError> OnPlayFabError;

#if PHOTON_UNITY_NETWORKING
        public static event Action<GetPhotonAuthenticationTokenResult> OnPhotonTokenObtained;
#endif

        //These are fields that we set when we are using the service.
        public string email;
        public string username;
        public string password;

#if PHOTON_UNITY_NETWORKING
        public bool authenticateToPhotonAfterSuccess;
#endif

        public GetPlayerCombinedInfoRequestParams infoRequestParams;
        //this is a force link flag for custom ids for rememberme
        public bool forceLink = false;
        public bool requireBothUsernameAndEmail = false;

        private Completer _completer;

        public override bool HasSession => !string.IsNullOrEmpty (Id) && !string.IsNullOrEmpty (SessionTicket);
#if PHOTON_UNITY_NETWORKING
        public string PhotonCustomAuthenticationToken { get; private set; }
#endif

        private const string LOGIN_REMEMBER_KEY = "PlayFabLoginRemember";
        private const string PLAYFAB_REMEMBERME_IDKEY = "PlayFabIdPassGuid";
        private const string PLAYFAB_LOGINTYPE_KEY = "PlayFabLoginType";

        public static PlayFabAuthService Instance => _instance ?? (_instance = new PlayFabAuthService ());
        private static PlayFabAuthService _instance;

        private PlayFabAuthService () {
            _instance = this;
        }

        /// <summary>
        /// Remember the user next time they log in
        /// This is used for Auto-Login purpose.
        /// </summary>
        public bool RememberMe {
            get => PlayerPrefs.GetInt (LOGIN_REMEMBER_KEY, 0) != 0;
            set => PlayerPrefs.SetInt (LOGIN_REMEMBER_KEY, value ? 1 : 0);
        }

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public PlayFabLoginType LoginType {
            get => (PlayFabLoginType) PlayerPrefs.GetInt (PLAYFAB_LOGINTYPE_KEY, 0);
            set => PlayerPrefs.SetInt (PLAYFAB_LOGINTYPE_KEY, (int) value);
        }

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public PlayFabRegisterType RegisterType { get; set; }

        /// <summary>
        /// Generated Remember Me ID
        /// Pass Null for a value to have one auto-generated.
        /// </summary>
        private string RememberMeId {
            get => PlayerPrefs.GetString (PLAYFAB_REMEMBERME_IDKEY, "");
            set {
                var guid = string.IsNullOrEmpty (value) ? Guid.NewGuid ().ToString () : value;
                PlayerPrefs.SetString (PLAYFAB_REMEMBERME_IDKEY, guid);
            }
        }

        public void ClearRememberMe () {
            PlayerPrefs.DeleteKey (LOGIN_REMEMBER_KEY);
            PlayerPrefs.DeleteKey (PLAYFAB_REMEMBERME_IDKEY);
        }

        /// <summary>
        /// Kick off the authentication process by specific authtype.
        /// </summary>
        /// <param name="loginType"></param>
        public void Login (PlayFabLoginType loginType) {
            LoginType = loginType;
            Login ();
        }

        public override void Login (Completer completer) {
            _completer = completer;
            Login ();
        }

        /// <summary>
        /// Authenticate the user by the Auth Type that was defined.
        /// </summary>
        public void Login () {
            var loginType = LoginType;

            switch (loginType) {
                case PlayFabLoginType.None:
                    OnCancelAuthentication?.Invoke ();
                    break;
                case PlayFabLoginType.Silent:
                    SilentlyAuthenticate ();
                    break;
                case PlayFabLoginType.EmailAndPassword:
                    AuthenticateEmailPassword ();
                    break;
                case PlayFabLoginType.Steam:
                    AuthenticateSteam ();
                    break;
                case PlayFabLoginType.Facebook:
                    AuthenticateFacebook ();
                    break;
                case PlayFabLoginType.Google:
                    AuthenticateGooglePlayGames ();
                    break;
                case PlayFabLoginType.UsernameAndPassword:
                    AuthenticateUsernamePassword ();
                    break;
                default:
                    OnCancelAuthentication?.Invoke ();
                    break;
            }
        }

        public override void Register(Completer completer)
        {
            _completer = completer;
            switch(RegisterType) {
                case PlayFabRegisterType.None:
                    break;
                case PlayFabRegisterType.RegisterPlayFabAccount:
                    RegisterPlayFabAccount();
                    break;
                case PlayFabRegisterType.AddPlayFabAccount:
                    AddPlayFabAccount();
                    break;
            }
        }

        public override void Logout (Completer completer) {
            Id = null;
            SessionTicket = null;
            ClearRememberMe ();

            PlayFabClientAPI.ForgetAllCredentials ();

            completer.Resolve();
            OnLogoutSuccess?.Invoke();
        }

        public void SetDisplayName (string displayName) {
            DisplayName = displayName;
        }

        /// <summary>
        /// Authenticate a user in PlayFab using an Email & Password combo
        /// </summary>
        private void AuthenticateEmailPassword () {
            //Check if the users has opted to be remembered.
            if (RememberMe && !string.IsNullOrEmpty (RememberMeId)) {

                //If the user is being remembered, then log them in with a customid that was 
                //generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID (new LoginWithCustomIDRequest () {
                    TitleId = PlayFabSettings.TitleId,
                        CustomId = RememberMeId,
                        CreateAccount = true,
                        InfoRequestParameters = infoRequestParams
                }, (result) => {
                    //Store identity and session
                    Id = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    //report login result back to subscriber
                    _completer.Resolve ();
                    OnLoginSuccess?.Invoke (result);

#if PHOTON_UNITY_NETWORKING
                    if (authenticateToPhotonAfterSuccess) {
                        RequestPhotonToken ();
                    }
#endif
                }, (error) => {
                    //report error back to subscriber
                    _completer.Reject (new Exception (error.GenerateErrorReport()));
                    // Debug.Log("Hi Error RememberMe");
                    OnPlayFabError?.Invoke (error);
                });
                return;
            }

            //a good catch: If username & password is empty, then do not continue, and Call back to Authentication UI Display 
            if (!RememberMe && string.IsNullOrEmpty (email) && string.IsNullOrEmpty (password)) {
                OnCancelAuthentication?.Invoke ();
                return;
            }

            //We have not opted for remember me in a previous session, so now we have to login the user with email & password.
            PlayFabClientAPI.LoginWithEmailAddress (new LoginWithEmailAddressRequest () {
                TitleId = PlayFabSettings.TitleId,
                Email = email,
                Password = password,
                InfoRequestParameters = infoRequestParams
            }, (result) => {
                //store identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                //Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                //If RememberMe is checked, then generate a new Guid for Login with CustomId.
                if (RememberMe) {
                    RememberMeId = Guid.NewGuid ().ToString ();
                    LoginType = PlayFabLoginType.EmailAndPassword;
                    //Fire and forget, but link a custom ID to this PlayFab Account.
                    PlayFabClientAPI.LinkCustomID (new LinkCustomIDRequest () {
                        CustomId = RememberMeId,
                            ForceLink = forceLink
                    }, null, null);
                }

                //report login result back to subscriber
                _completer.Resolve ();
                OnLoginSuccess?.Invoke (result);

#if PHOTON_UNITY_NETWORKING
                if (authenticateToPhotonAfterSuccess) {
                    RequestPhotonToken ();
                }
#endif

            }, (error) => {
                //Report error back to subscriber
                _completer.Reject (new Exception (error.GenerateErrorReport()));
                OnPlayFabError?.Invoke (error);
            });
        }

        /// <summary>
        /// Authenticate a user in PlayFab using an Email & Password combo
        /// </summary>
        private void AuthenticateUsernamePassword () {
            //Check if the users has opted to be remembered.
            if (RememberMe && !string.IsNullOrEmpty (RememberMeId)) {
                //If the user is being remembered, then log them in with a customid that was 
                //generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID (new LoginWithCustomIDRequest () {
                    TitleId = PlayFabSettings.TitleId,
                        CustomId = RememberMeId,
                        CreateAccount = true,
                        InfoRequestParameters = infoRequestParams
                }, (result) => {
                    //Store identity and session
                    Id = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    //report login result back to subscriber
                    _completer.Resolve ();
                    OnLoginSuccess?.Invoke (result);

#if PHOTON_UNITY_NETWORKING
                    if (authenticateToPhotonAfterSuccess) {
                        RequestPhotonToken ();
                    }
#endif

                }, (error) => {
                    //report error back to subscriber
                    _completer.Reject (new Exception (error.GenerateErrorReport()));
                    OnPlayFabError?.Invoke (error);
                });
                return;
            }

            //a good catch: If username & password is empty, then do not continue, and Call back to Authentication UI Display 
            if (!RememberMe && string.IsNullOrEmpty (username) && string.IsNullOrEmpty (password)) {
                OnCancelAuthentication?.Invoke ();
                return;
            }

            //We have not opted for remember me in a previous session, so now we have to login the user with email & password.
            PlayFabClientAPI.LoginWithPlayFab (new LoginWithPlayFabRequest () {
                TitleId = PlayFabSettings.TitleId,
                    Username = username,
                    Password = password,
                    InfoRequestParameters = infoRequestParams
            }, (result) => {
                //store identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                //Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                //If RememberMe is checked, then generate a new Guid for Login with CustomId.
                if (RememberMe) {
                    RememberMeId = Guid.NewGuid ().ToString ();
                    LoginType = PlayFabLoginType.UsernameAndPassword;
                    //Fire and forget, but link a custom ID to this PlayFab Account.
                    PlayFabClientAPI.LinkCustomID (new LinkCustomIDRequest () {
                        CustomId = RememberMeId,
                            ForceLink = forceLink
                    }, null, null);
                }

                //report login result back to subscriber
                _completer.Resolve ();
                OnLoginSuccess?.Invoke (result);

#if PHOTON_UNITY_NETWORKING
                if (authenticateToPhotonAfterSuccess) {
                    RequestPhotonToken ();
                }
#endif

            }, (error) => {
                //Report error back to subscriber
                _completer.Reject (new Exception (error.GenerateErrorReport()));
                OnPlayFabError?.Invoke (error);
            });
        }

        /// <summary>
        /// Add username, email and password to a custom id auth
        /// </summary>
        private void AddPlayFabAccount () {
            //Any time we attempt to register a player, first silently authenticate the player.
            //This will retain the players True Origination (Android, iOS, Desktop)
            SilentlyAuthenticate ((result) => {

                if (result == null) {
                    //something went wrong with Silent Authentication, Check the debug console.
                    OnPlayFabError?.Invoke (new PlayFabError () {
                        Error = PlayFabErrorCode.UnknownError,
                            ErrorMessage = "Silent Authentication by Device failed, called when trying to AddPlayFabAccount"
                    });

                    return;
                }

                //Note: If silent auth is success, which is should always be and the following 
                //below code fails because of some error returned by the server ( like invalid email or bad password )
                //this is okay, because the next attempt will still use the same silent account that was already created.

                //Now add our username & password.
                PlayFabClientAPI.AddUsernamePassword (new AddUsernamePasswordRequest () {
                    Username = !string.IsNullOrEmpty (username) ?
                        username :
                        result.PlayFabId, //Because it is required & Unique and not supplied by User.
                        Email = email, // Required
                        Password = password, // Required
                }, (addResult) => {
                    //Store identity and session
                    Id = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    //If they opted to be remembered on next login.
                    if (RememberMe) {
                        //Generate a new Guid 
                        RememberMeId = Guid.NewGuid ().ToString ();
                        //Fire and forget, but link the custom ID to this PlayFab Account.
                        PlayFabClientAPI.LinkCustomID (new LinkCustomIDRequest () {
                            CustomId = RememberMeId,
                                ForceLink = forceLink
                        }, null, null);
                    }

                    //Override the auth type to ensure next login is using this auth type.
                    LoginType = PlayFabLoginType.EmailAndPassword;

                    //Report login result back to subscriber.
                    _completer.Resolve ();
                    OnLoginSuccess?.Invoke (result);

#if PHOTON_UNITY_NETWORKING
                    if (authenticateToPhotonAfterSuccess) {
                        RequestPhotonToken ();
                    }
#endif
                }, (error) => {
                    //Report error result back to subscriber
                    _completer.Reject (new Exception (error.GenerateErrorReport()));
                    OnPlayFabError?.Invoke (error);
                });

            });
        }

        /// <summary>
        /// Register a user with an Email & Password
        /// Note: We are not using the RegisterPlayFab API
        /// </summary>
        private void RegisterPlayFabAccount () {
            var registerRequest = new RegisterPlayFabUserRequest () {
                Username = string.IsNullOrEmpty (this.username) ? null : this.username,
                Email = email, // Required
                Password = password, // Required
                DisplayName = string.IsNullOrEmpty (DisplayName) ? null : DisplayName,
                RequireBothUsernameAndEmail = requireBothUsernameAndEmail
            };

            PlayFabClientAPI.RegisterPlayFabUser (registerRequest, (result) => {
                //Store identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                //Report register result back to subscriber.
                _completer.Resolve ();
                OnRegisterSuccess?.Invoke (result);

#if PHOTON_UNITY_NETWORKING
                if (authenticateToPhotonAfterSuccess) {
                    RequestPhotonToken ();
                }
#endif
            }, (error) => {
                //Report error result back to subscriber
                _completer.Reject (new Exception (error.GenerateErrorReport()));
                OnPlayFabError?.Invoke (error);
            });
        }

        private void AuthenticateFacebook () {
#if FACEBOOK
            if (FB.IsInitialized && FB.IsLoggedIn && !string.IsNullOrEmpty (AuthTicket)) {
                PlayFabClientAPI.LoginWithFacebook (new LoginWithFacebookRequest () {
                    TitleId = PlayFabSettings.TitleId,
                        AccessToken = AuthTicket,
                        CreateAccount = true,
                        InfoRequestParameters = InfoRequestParams
                }, (result) => {
                    //Store Identity and session
                    Id = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    //check if we want to get this callback directly or send to event subscribers.
                    if (OnLoginSuccess != null) {
                        //report login result back to the subscriber
                        OnLoginSuccess.Invoke (result);
                    }
                }, (error) => {

                    //report error back to the subscriber
                    if (OnPlayFabError != null) {
                        OnPlayFabError.Invoke (error);
                    }
                });
            } else {
                if (OnDisplayAuthentication != null) {
                    OnDisplayAuthentication.Invoke ();
                }
            }
#endif
        }

        private void AuthenticateGooglePlayGames () {
#if GOOGLEGAMES
            PlayFabClientAPI.LoginWithGoogleAccount (new LoginWithGoogleAccountRequest () {
                TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = AuthTicket,
                    InfoRequestParameters = InfoRequestParams,
                    CreateAccount = true
            }, (result) => {
                //Store Identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                //check if we want to get this callback directly or send to event subscribers.
                if (OnLoginSuccess != null) {
                    //report login result back to the subscriber
                    OnLoginSuccess.Invoke (result);
                }
            }, (error) => {

                //report errro back to the subscriber
                if (OnPlayFabError != null) {
                    OnPlayFabError.Invoke (error);
                }
            });
#endif
        }

        private void AuthenticateSteam () {

        }

        private void SilentlyAuthenticate(Action<LoginResult> onResult = null, Action<PlayFabError> onError = null) {
#if UNITY_ANDROID && !UNITY_EDITOR

            // Get the device id from native android
            AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
            string deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");

            // Login with the android device ID
            PlayFabClientAPI.LoginWithAndroidDeviceID (new LoginWithAndroidDeviceIDRequest () {
                TitleId = PlayFabSettings.TitleId,
                AndroidDevice = SystemInfo.deviceModel,
                OS = SystemInfo.operatingSystem,
                AndroidDeviceId = deviceId,
                CreateAccount = true,
                InfoRequestParameters = InfoRequestParams
            }, (result) => {

                // Store Identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                OnLoginSuccess?.Invoke(result);
                onResult?.Invoke(result);

            }, (error) => {

                OnPlayFabError?.Invoke(error);
                onError?.Invoke(null);
                //Output what went wrong to the console.
                Debug.LogError (error.GenerateErrorReport ());
            });

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
            PlayFabClientAPI.LoginWithIOSDeviceID (new LoginWithIOSDeviceIDRequest () {
                TitleId = PlayFabSettings.TitleId,
                    DeviceModel = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
            }, (result) => {
                //Store Identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                OnLoginSuccess?.Invoke (result);
                callback?.Invoke (result);
            }, (error) => {

                OnPlayFabError?.Invoke (error);
                callback?.Invoke (null);

                Debug.LogError (error.GenerateErrorReport ());
            });
#else
            PlayFabClientAPI.LoginWithCustomID (new LoginWithCustomIDRequest () {
                TitleId = PlayFabSettings.TitleId,
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = infoRequestParams
            }, (result) => {
                //Store Identity and session
                Id = result.PlayFabId;
                SessionTicket = result.SessionTicket;

                OnLoginSuccess?.Invoke (result);
                onResult?.Invoke (result);
                _completer.Resolve ();

#if PHOTON_UNITY_NETWORKING
                if (authenticateToPhotonAfterSuccess) {
                    RequestPhotonToken ();
                }
#endif
            }, (error) => {
                OnPlayFabError?.Invoke (error);
                onResult?.Invoke (null);
                Debug.LogError (error.GenerateErrorReport ());

                _completer.Reject (new Exception (error.GenerateErrorReport()));
            });
#endif
        }

        public void UnlinkSilentAuth () {
            SilentlyAuthenticate ((result) => {

#if UNITY_ANDROID && !UNITY_EDITOR
                //Get the device id from native android
                AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
                string deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");

                //Fire and forget, unlink this android device.
                PlayFabClientAPI.UnlinkAndroidDeviceID (new UnlinkAndroidDeviceIDRequest () {
                    AndroidDeviceId = deviceId
                }, null, null);

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
                PlayFabClientAPI.UnlinkIOSDeviceID (new UnlinkIOSDeviceIDRequest () {
                    DeviceId = SystemInfo.deviceUniqueIdentifier
                }, null, null);
#else
                PlayFabClientAPI.UnlinkCustomID (new UnlinkCustomIDRequest () {
                    CustomId = SystemInfo.deviceUniqueIdentifier
                }, null, null);
#endif

            });
        }

#if PHOTON_UNITY_NETWORKING
        public void RequestPhotonToken (string photonApplicationId, Action<GetPhotonAuthenticationTokenResult> result = null, Action<PlayFabError> onError = null) {
            PlayFabClientAPI.GetPhotonAuthenticationToken (new GetPhotonAuthenticationTokenRequest () {
                PhotonApplicationId = photonApplicationId
            }, (rc) => {
                result?.Invoke (rc);
                OnPhotonTokenObtained?.Invoke (rc);
            }, onError);
        }

        public void RequestPhotonToken (Action<GetPhotonAuthenticationTokenResult> result = null, Action<PlayFabError> onError = null) {
            PhotonAuthService photonAuth = PhotonAuthService.Instance;

            PlayFabClientAPI.GetPhotonAuthenticationToken (new GetPhotonAuthenticationTokenRequest () {
                PhotonApplicationId = PhotonAuthService.AppIdRealtime
            }, (rc) => {
                photonAuth.AuthType = CustomAuthenticationType.Custom;
                photonAuth.SessionTicket = rc.PhotonCustomAuthenticationToken;
                photonAuth.Id = Id;
                photonAuth.DisplayName = DisplayName;
                photonAuth.Login ();

                PhotonCustomAuthenticationToken = rc.PhotonCustomAuthenticationToken;
                result?.Invoke (rc);
                OnPhotonTokenObtained?.Invoke (rc);
            }, (error) => {

                onError?.Invoke(error);
                OnPlayFabError?.Invoke(error);
            });
        }
#endif
    }
}
#endif
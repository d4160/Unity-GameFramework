using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;
using System;
using Photon.Realtime;
using UnityEngine.Promise;

#if FACEBOOK
using Facebook.Unity;
#endif

namespace d4160.GameFramework.Authentication
{
    /// <summary>
    /// Supported Authentication types
    /// Note: Add types to there to support more AuthTypes
    /// See - https://api.playfab.com/documentation/client#Authentication
    /// </summary>
    public enum PlayFabAuthTypes
    {
        None,
        Silent,
        UsernameAndPassword,
        EmailAndPassword,
        RegisterPlayFabAccount,
        AddPlayFabAccount,
        Steam,
        Facebook,
        Google
    }

    public class PlayFabAuthService : IAuthService
    {

        //Events to subscribe to for this service
        public delegate void DisplayAuthenticationEvent();

        public static event DisplayAuthenticationEvent OnDisplayAuthentication;

        public delegate void RegisterSuccessEvent(RegisterPlayFabUserResult success);

        public static event RegisterSuccessEvent OnRegisterSuccess;

        public delegate void LoginSuccessEvent(LoginResult success);

        public static event LoginSuccessEvent OnLoginSuccess;

        public delegate void PlayFabErrorEvent(PlayFabError error);

        public static event PlayFabErrorEvent OnPlayFabError;

#if PHOTON_UNITY_NETWORKING
        public delegate void PhotonAuthenticationTokenEvent(GetPhotonAuthenticationTokenResult result);

        public static event PhotonAuthenticationTokenEvent OnPhotonTokenObtained;
#endif

        //These are fields that we set when we are using the service.
        public string Email;
        public string Username;
        public string Password;
        public string AuthTicket;
#if PHOTON_UNITY_NETWORKING
        public bool AuthenticateToPhotonAfterLogin;
#endif
        public GetPlayerCombinedInfoRequestParams InfoRequestParams;

        //this is a force link flag for custom ids for demoing
        public bool ForceLink = false;
        public bool RequireBothUsernameAndEmail = false;

        private Completer _completer;

        public string DisplayName { get; set; }
        public string Id => _playFabId;
        public static string SessionTicket => _sessionTicket;

        // Accessbility for PlayFab ID & Session Tickets
        private static string _playFabId;
        private static string _sessionTicket;

        private const string LoginRememberKey = "PlayFabLoginRemember";
        private const string PlayFabRememberMeIdKey = "PlayFabIdPassGuid";
        private const string PlayFabAuthTypeKey = "PlayFabAuthType";

        public static PlayFabAuthService Instance => _instance ?? (_instance = new PlayFabAuthService());

        private static PlayFabAuthService _instance;

        public PlayFabAuthService()
        {
            _instance = this;
        }

        /// <summary>
        /// Remember the user next time they log in
        /// This is used for Auto-Login purpose.
        /// </summary>
        public bool RememberMe
        {
            get => PlayerPrefs.GetInt(LoginRememberKey, 0) != 0;
            set => PlayerPrefs.SetInt(LoginRememberKey, value ? 1 : 0);
        }

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public PlayFabAuthTypes AuthType
        {
            get => (PlayFabAuthTypes) PlayerPrefs.GetInt(PlayFabAuthTypeKey, 0);
            set => PlayerPrefs.SetInt(PlayFabAuthTypeKey, (int) value);
        }

        /// <summary>
        /// Generated Remember Me ID
        /// Pass Null for a value to have one auto-generated.
        /// </summary>
        private string RememberMeId
        {
            get => PlayerPrefs.GetString(PlayFabRememberMeIdKey, "");
            set
            {
                var guid = string.IsNullOrEmpty(value) ? Guid.NewGuid().ToString() : value;
                PlayerPrefs.SetString(PlayFabRememberMeIdKey, guid);
            }
        }

        public void ClearRememberMe()
        {
            PlayerPrefs.DeleteKey(LoginRememberKey);
            PlayerPrefs.DeleteKey(PlayFabRememberMeIdKey);
        }

        /// <summary>
        /// Kick off the authentication process by specific authtype.
        /// </summary>
        /// <param name="authType"></param>
        public void Authenticate(PlayFabAuthTypes authType)
        {
            AuthType = authType;
            Authenticate();
        }

        /// <summary>
        /// Authenticate the user by the Auth Type that was defined.
        /// </summary>
        public void Authenticate()
        {
            var authType = AuthType;

            switch (authType)
            {
                case PlayFabAuthTypes.None:
                    OnDisplayAuthentication?.Invoke();
                    break;
                case PlayFabAuthTypes.Silent:
                    SilentlyAuthenticate();
                    break;
                case PlayFabAuthTypes.EmailAndPassword:
                    AuthenticateEmailPassword();
                    break;
                case PlayFabAuthTypes.AddPlayFabAccount:
                    AddPlayFabAccount();
                    break;
                case PlayFabAuthTypes.Steam:
                    AuthenticateSteam();
                    break;
                case PlayFabAuthTypes.Facebook:
                    AuthenticateFacebook();
                    break;
                case PlayFabAuthTypes.Google:
                    AuthenticateGooglePlayGames();
                    break;
                case PlayFabAuthTypes.UsernameAndPassword:
                    AuthenticateUsernamePassword();
                    break;
                case PlayFabAuthTypes.RegisterPlayFabAccount:
                    RegisterPlayFabAccount();
                    break;
                default:
                    OnDisplayAuthentication?.Invoke();
                    break;
            }
        }

        public void Authenticate(Completer completer)
        {
            var authType = AuthType;
            //Debug.Log(authType);

            _completer = completer;

            switch (authType)
            {
                case PlayFabAuthTypes.None:
                    OnDisplayAuthentication?.Invoke();
                    break;
                case PlayFabAuthTypes.Silent:
                    SilentlyAuthenticate();
                    break;
                case PlayFabAuthTypes.EmailAndPassword:
                    AuthenticateEmailPassword();
                    break;
                case PlayFabAuthTypes.AddPlayFabAccount:
                    AddPlayFabAccount();
                    break;
                case PlayFabAuthTypes.Steam:
                    AuthenticateSteam();
                    break;
                case PlayFabAuthTypes.Facebook:
                    AuthenticateFacebook();
                    break;
                case PlayFabAuthTypes.Google:
                    AuthenticateGooglePlayGames();
                    break;
                case PlayFabAuthTypes.UsernameAndPassword:
                    AuthenticateUsernamePassword();
                    break;
                case PlayFabAuthTypes.RegisterPlayFabAccount:
                    RegisterPlayFabAccount();
                    break;
                default:
                    OnDisplayAuthentication?.Invoke();
                    break;
            }
        }

        public void Unauthenticate()
        {
            _playFabId = null;
            _sessionTicket = null;

            PlayFabClientAPI.ForgetAllCredentials();
        }

        public void UpdateDisplayName(Action<UpdateUserTitleDisplayNameResult> resultCallback = null, Action<PlayFabError> errorCallback = null)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
                {
                    DisplayName = DisplayName
                },
                (result) =>
                {
                    resultCallback?.Invoke(result);
                },
                (error) =>
                {
                    errorCallback?.Invoke(error);
                });
        }

        public void GetDisplayName(Action<string> resultCallback = null, Action<PlayFabError> errorCallback = null)
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
                {
                    PlayFabId = _playFabId
                },
                (result) =>
                {
                    DisplayName = result.PlayerProfile.DisplayName;
                    resultCallback?.Invoke(DisplayName);
                },
                (error) =>
                {
                    errorCallback?.Invoke(error);
                });
        }

        public void AddOrUpdateContactEmail(Action<AddOrUpdateContactEmailResult> resultCallback = null, Action<PlayFabError> errorCallback = null)
        {
            PlayFabClientAPI.AddOrUpdateContactEmail(new AddOrUpdateContactEmailRequest()
                {
                    EmailAddress = Email
                },
                (result) =>
                {
                    resultCallback?.Invoke(result);
                },
                (error) =>
                {
                    errorCallback?.Invoke(error);
                });
        }

        public void RemoveContactEmail(Action<RemoveContactEmailResult> resultCallback = null, Action<PlayFabError> errorCallback = null)
        {
            PlayFabClientAPI.RemoveContactEmail(new RemoveContactEmailRequest(),
                (result) =>
                {
                    resultCallback?.Invoke(result);
                },
                (error) =>
                {
                    errorCallback?.Invoke(error);
                });
        }

        /// <summary>
        /// Authenticate a user in PlayFab using an Email & Password combo
        /// </summary>
        private void AuthenticateEmailPassword()
        {
            //Check if the users has opted to be remembered.
            if (RememberMe && !string.IsNullOrEmpty(RememberMeId))
            {
                //If the user is being remembered, then log them in with a customid that was 
                //generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = RememberMeId,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
                }, (result) =>
                {
                    //Store identity and session
                    _playFabId = result.PlayFabId;
                    _sessionTicket = result.SessionTicket;

                    //report login result back to subscriber
                    _completer.Resolve();
                    OnLoginSuccess?.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                    if (AuthenticateToPhotonAfterLogin)
                    {
                        RequestPhotonToken();
                    }
#endif
                }, (error) =>
                {
                    //report error back to subscriber
                    _completer.Reject(new Exception(error.ErrorMessage));
                    OnPlayFabError?.Invoke(error);
                });
                return;
            }

            //a good catch: If username & password is empty, then do not continue, and Call back to Authentication UI Display 
            if (!RememberMe && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                OnDisplayAuthentication?.Invoke();
                return;
            }

            //We have not opted for remember me in a previous session, so now we have to login the user with email & password.
            PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Email = Email,
                Password = Password,
                InfoRequestParameters = InfoRequestParams
            }, (result) =>
            {
                //store identity and session
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;

                //Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                //If RememberMe is checked, then generate a new Guid for Login with CustomId.
                if (RememberMe)
                {
                    RememberMeId = Guid.NewGuid().ToString();
                    AuthType = PlayFabAuthTypes.EmailAndPassword;
                    //Fire and forget, but link a custom ID to this PlayFab Account.
                    PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
                    {
                        CustomId = RememberMeId,
                        ForceLink = ForceLink
                    }, null, null);
                }

                //report login result back to subscriber
                _completer.Resolve();
                OnLoginSuccess?.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                if (AuthenticateToPhotonAfterLogin)
                {
                    RequestPhotonToken();
                }
#endif

            }, (error) =>
            {
                //Report error back to subscriber
                _completer.Reject(new Exception(error.ErrorMessage));
                OnPlayFabError?.Invoke(error);
            });
        }

        /// <summary>
        /// Authenticate a user in PlayFab using an Email & Password combo
        /// </summary>
        private void AuthenticateUsernamePassword()
        {
            //Check if the users has opted to be remembered.
            if (RememberMe && !string.IsNullOrEmpty(RememberMeId))
            {
                //If the user is being remembered, then log them in with a customid that was 
                //generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = RememberMeId,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
                }, (result) =>
                {
                    //Store identity and session
                    _playFabId = result.PlayFabId;
                    _sessionTicket = result.SessionTicket;

                    //report login result back to subscriber
                    _completer.Resolve();
                    OnLoginSuccess?.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                    if (AuthenticateToPhotonAfterLogin)
                    {
                        RequestPhotonToken();
                    }
#endif

                }, (error) =>
                {
                    //report error back to subscriber
                    _completer.Reject(new Exception(error.ErrorMessage));
                    OnPlayFabError?.Invoke(error);
                });
                return;
            }

            //a good catch: If username & password is empty, then do not continue, and Call back to Authentication UI Display 
            if (!RememberMe && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                OnDisplayAuthentication?.Invoke();
                return;
            }

            //We have not opted for remember me in a previous session, so now we have to login the user with email & password.
            PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Username = Username,
                Password = Password,
                InfoRequestParameters = InfoRequestParams
            }, (result) =>
            {
                //store identity and session
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;

                //Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                //If RememberMe is checked, then generate a new Guid for Login with CustomId.
                if (RememberMe)
                {
                    RememberMeId = Guid.NewGuid().ToString();
                    AuthType = PlayFabAuthTypes.UsernameAndPassword;
                    //Fire and forget, but link a custom ID to this PlayFab Account.
                    PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
                    {
                        CustomId = RememberMeId,
                        ForceLink = ForceLink
                    }, null, null);
                }

                //report login result back to subscriber
                _completer.Resolve();
                OnLoginSuccess?.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                if (AuthenticateToPhotonAfterLogin)
                {
                    RequestPhotonToken();
                }
#endif

            }, (error) =>
            {
                //Report error back to subscriber
                _completer.Reject(new Exception(error.ErrorMessage));
                OnPlayFabError?.Invoke(error);
            });
        }

        /// <summary>
        /// Add username, email and password to a custom id auth
        /// </summary>
        private void AddPlayFabAccount()
        {
            //Any time we attempt to register a player, first silently authenticate the player.
            //This will retain the players True Origination (Android, iOS, Desktop)
            SilentlyAuthenticate((result) =>
            {

                if (result == null)
                {
                    //something went wrong with Silent Authentication, Check the debug console.
                    OnPlayFabError?.Invoke(new PlayFabError()
                    {
                        Error = PlayFabErrorCode.UnknownError,
                        ErrorMessage = "Silent Authentication by Device failed, called when trying to AddPlayFabAccount"
                    });

                    return;
                }

                //Note: If silent auth is success, which is should always be and the following 
                //below code fails because of some error returned by the server ( like invalid email or bad password )
                //this is okay, because the next attempt will still use the same silent account that was already created.

                //Now add our username & password.
                PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest()
                {
                    Username = !string.IsNullOrEmpty(Username)
                        ? Username
                        : result.PlayFabId, //Because it is required & Unique and not supplied by User.
                    Email = Email, // Required
                    Password = Password, // Required
                }, (addResult) =>
                {
                    //Store identity and session
                    _playFabId = result.PlayFabId;
                    _sessionTicket = result.SessionTicket;

                    //If they opted to be remembered on next login.
                    if (RememberMe)
                    {
                        //Generate a new Guid 
                        RememberMeId = Guid.NewGuid().ToString();
                        //Fire and forget, but link the custom ID to this PlayFab Account.
                        PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
                        {
                            CustomId = RememberMeId,
                            ForceLink = ForceLink
                        }, null, null);
                    }

                    //Override the auth type to ensure next login is using this auth type.
                    AuthType = PlayFabAuthTypes.EmailAndPassword;

                    //Report login result back to subscriber.
                    _completer.Resolve();
                    OnLoginSuccess?.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                    if (AuthenticateToPhotonAfterLogin)
                    {
                        RequestPhotonToken();
                    }
#endif
                }, (error) =>
                {
                    //Report error result back to subscriber
                    _completer.Reject(new Exception(error.ErrorMessage));
                    OnPlayFabError?.Invoke(error);
                });

            });
        }

        /// <summary>
        /// Register a user with an Email & Password
        /// Note: We are not using the RegisterPlayFab API
        /// </summary>
        private void RegisterPlayFabAccount()
        {
            var username = string.IsNullOrEmpty(Username) ? null : Username;
            PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
            {
                Username = username,
                Email = Email, // Required
                Password = Password, // Required
                DisplayName = DisplayName,
                RequireBothUsernameAndEmail = RequireBothUsernameAndEmail
            }, (result) =>
            {
                //Store identity and session
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;

                //Report register result back to subscriber.
                _completer.Resolve();
                OnRegisterSuccess?.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                if (AuthenticateToPhotonAfterLogin)
                {
                    RequestPhotonToken();
                }
#endif
            }, (error) =>
            {
                //Report error result back to subscriber
                _completer.Reject(new Exception(error.ErrorMessage));
                OnPlayFabError?.Invoke(error);
            });
        }

        private void AuthenticateFacebook()
        {
#if FACEBOOK
        if (FB.IsInitialized && FB.IsLoggedIn && !string.IsNullOrEmpty(AuthTicket))
        {
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                AccessToken = AuthTicket,
                CreateAccount = true,
                InfoRequestParameters = InfoRequestParams
            }, (result) =>
            {
                //Store Identity and session
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;

                //check if we want to get this callback directly or send to event subscribers.
                if (OnLoginSuccess != null)
                {
                    //report login result back to the subscriber
                    OnLoginSuccess.Invoke(result);
                }
            }, (error) =>
            {

                //report errro back to the subscriber
                if (OnPlayFabError != null)
                {
                    OnPlayFabError.Invoke(error);
                }
            });
        }
        else
        {
            if (OnDisplayAuthentication != null)
            {
                OnDisplayAuthentication.Invoke();
            }
        }
#endif
        }

        private void AuthenticateGooglePlayGames()
        {
#if GOOGLEGAMES
        PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            ServerAuthCode = AuthTicket,
            InfoRequestParameters = InfoRequestParams,
            CreateAccount = true
        }, (result) =>
        {
            //Store Identity and session
            _playFabId = result.PlayFabId;
            _sessionTicket = result.SessionTicket;

            //check if we want to get this callback directly or send to event subscribers.
            if (OnLoginSuccess != null)
            {
                //report login result back to the subscriber
                OnLoginSuccess.Invoke(result);
            }
        }, (error) =>
        {

            //report errro back to the subscriber
            if (OnPlayFabError != null)
            {
                OnPlayFabError.Invoke(error);
            }
        });
#endif
        }

        private void AuthenticateSteam()
        {

        }


        private void SilentlyAuthenticate(System.Action<LoginResult> callback = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

        //Get the device id from native android
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
        string deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

        //Login with the android device ID
        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest() {
            TitleId = PlayFabSettings.TitleId,
            AndroidDevice = SystemInfo.deviceModel,
            OS = SystemInfo.operatingSystem,
            AndroidDeviceId = deviceId,
            CreateAccount = true,
            InfoRequestParameters = InfoRequestParams
        }, (result) => {
            
            //Store Identity and session
            _playFabId = result.PlayFabId;
            _sessionTicket = result.SessionTicket;

            //check if we want to get this callback directly or send to event subscribers.
            if (callback == null && OnLoginSuccess != null)
            {
                //report login result back to the subscriber
                OnLoginSuccess.Invoke(result);
            }else if (callback != null)
            {
                //report login result back to the caller
                callback.Invoke(result);
            }
        }, (error) => {

            //report errro back to the subscriber
            if(callback == null && OnPlayFabError != null){
                OnPlayFabError.Invoke(error);
            }else{
                //make sure the loop completes, callback with null
                callback.Invoke(null);
                //Output what went wrong to the console.
                Debug.LogError(error.GenerateErrorReport());
            }
        });

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
        PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest() {
            TitleId = PlayFabSettings.TitleId,
            DeviceModel = SystemInfo.deviceModel, 
            OS = SystemInfo.operatingSystem,
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = InfoRequestParams
        }, (result) => {
            //Store Identity and session
            _playFabId = result.PlayFabId;
            _sessionTicket = result.SessionTicket;

            //check if we want to get this callback directly or send to event subscribers.
            if (callback == null && OnLoginSuccess != null)
            {
                //report login result back to the subscriber
                OnLoginSuccess.Invoke(result);
            }else if (callback != null)
            {
                //report login result back to the caller
                callback.Invoke(result);
            }
        }, (error) => {
            //report errro back to the subscriber
            if(callback == null && OnPlayFabError != null){
                OnPlayFabError.Invoke(error);
            }else{
                //make sure the loop completes, callback with null
                callback.Invoke(null);
                //Output what went wrong to the console.
                Debug.LogError(error.GenerateErrorReport());
            }
        });
#else
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = InfoRequestParams
            }, (result) =>
            {
                //Store Identity and session
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;

                //check if we want to get this callback directly or send to event subscribers.
                if (callback == null && OnLoginSuccess != null)
                {
                    //report login result back to the subscriber
                    _completer.Resolve();
                    OnLoginSuccess.Invoke(result);

#if PHOTON_UNITY_NETWORKING
                    if (AuthenticateToPhotonAfterLogin)
                    {
                        RequestPhotonToken();
                    }
#endif
                }
                else
                {
                    if(callback != null)
                        callback.Invoke(result);
                    else
                    {
                        _completer.Resolve();

#if PHOTON_UNITY_NETWORKING
                        if (AuthenticateToPhotonAfterLogin)
                        {
                            RequestPhotonToken();
                        }
#endif
                    }
                }
            }, (error) =>
            {
                //report errro back to the subscriber
                if (callback == null && OnPlayFabError != null)
                {
                    OnPlayFabError.Invoke(error);
                }
                else
                {
                    //make sure the loop completes, callback with null
                    callback?.Invoke(null);
                    //Output what went wrong to the console.
                    Debug.LogError(error.GenerateErrorReport());
                }

                _completer.Reject(new Exception(error.ErrorMessage));
            });
#endif
        }

        public void UnlinkSilentAuth()
        {
            SilentlyAuthenticate((result) =>
            {

#if UNITY_ANDROID && !UNITY_EDITOR
            //Get the device id from native android
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

            //Fire and forget, unlink this android device.
            PlayFabClientAPI.UnlinkAndroidDeviceID(new UnlinkAndroidDeviceIDRequest() {
                AndroidDeviceId = deviceId
            }, null, null);

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
            PlayFabClientAPI.UnlinkIOSDeviceID(new UnlinkIOSDeviceIDRequest()
            {
                DeviceId = SystemInfo.deviceUniqueIdentifier
            }, null, null);
#else
                PlayFabClientAPI.UnlinkCustomID(new UnlinkCustomIDRequest()
                {
                    CustomId = SystemInfo.deviceUniqueIdentifier
                }, null, null);
#endif

            });
        }

#if PHOTON_UNITY_NETWORKING
        public void RequestPhotonToken(string photonApplicationId, Action<GetPhotonAuthenticationTokenResult> result = null, Action<PlayFabError> onError = null)
        {
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
            {
                PhotonApplicationId = photonApplicationId
            }, (rc) =>
            {
                result?.Invoke(rc);
                OnPhotonTokenObtained?.Invoke(rc);
            }, onError);
        }

        public void RequestPhotonToken(Action<GetPhotonAuthenticationTokenResult> result = null, Action<PlayFabError> onError = null)
        {
            PhotonAuthService photonAuth = PhotonAuthService.Instance;
            
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
            {
                PhotonApplicationId = PhotonAuthService.AppIdRealtime
            }, (rc) =>
            {
                photonAuth.AuthType = CustomAuthenticationType.Custom;
                photonAuth.Token = rc.PhotonCustomAuthenticationToken;
                photonAuth.CustomServiceId = _playFabId;
                photonAuth.Authenticate();

                result?.Invoke(rc);

                OnPhotonTokenObtained?.Invoke(rc);
            }, onError);
        }
#endif
    }
}
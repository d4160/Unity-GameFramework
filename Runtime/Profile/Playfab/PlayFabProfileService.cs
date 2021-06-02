#if PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using LoginResult = PlayFab.ClientModels.LoginResult;
using System;
using UnityEngine.Promise;
using Logger = d4160.Logging.Logger;
using d4160.Auth.PlayFab;
using d4160.Core;

namespace d4160.Profile.PlayFab {

    public sealed class PlayFabProfileService : BaseProfileService 
    {
        public static event Action<GetPlayerProfileResult> OnGetPlayerProfile;
        public static event Action<UpdateUserTitleDisplayNameResult> OnUpdateDisplayName;
        public static event Action<AddOrUpdateContactEmailResult> OnAddOrUpdateContactEmail;
        public static event Action<RemoveContactEmailResult> OnRemoveContactEmail;
        public static event Action<PlayFabError> OnPlayFabError;

        private string _email;
#if PHOTON_UNITY_NETWORKING
        private bool _authenticateToPhotonAfterSuccess;
#endif

        private Completer _completer;

#if PHOTON_UNITY_NETWORKING
        public bool AuthenticateToPhotonAfterSuccess { get => _authenticateToPhotonAfterSuccess; set => _authenticateToPhotonAfterSuccess = value; }
#endif
        public string Email { get => _email; set => _email = value; }
        public PlayerProfileModel PlayerProfile { get; private set; }
        public LogLevelType LogLevel { get; private set; } = LogLevelType.Debug;

        private readonly PlayFabAuthService _authService = PlayFabAuthService.Instance;
        public static PlayFabProfileService Instance => _instance ?? (_instance = new PlayFabProfileService ());
        private static PlayFabProfileService _instance;

        private PlayFabProfileService () {
        }

        public void SetDisplayName (string displayName) {
            DisplayName = displayName;
        }

        public void UpdateDisplayName (string displayName, Action<UpdateUserTitleDisplayNameResult> resultCallback = null, Action<PlayFabError> errorCallback = null) {
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest () {
                    DisplayName = displayName
                },
                (result) => {
#if PHOTON_UNITY_NETWORKING
                // TODO
                    if (_authenticateToPhotonAfterSuccess) {
                        // PhotonAuthService photonAuth = PhotonAuthService.Instance;
                        // photonAuth.SetDisplayName(DisplayName);
                    }
#endif              
                    DisplayName = result.DisplayName;
                    resultCallback?.Invoke(result);
                    OnUpdateDisplayName?.Invoke(result);
                },
                (error) => {
                    errorCallback?.Invoke(error);
                    OnPlayFabError?.Invoke(error);
                });
        }

        public void GetPlayerProfile (Action<GetPlayerProfileResult> resultCallback = null, Action<PlayFabError> errorCallback = null) {

            if (string.IsNullOrEmpty(_authService.Id))
            {
                Logger.LogWarning("PLAYFAB PROFILE: You need to login first", LogLevel);
                errorCallback?.Invoke(new PlayFabError{ Error = PlayFabErrorCode.Unknown });
                return;
            }

            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest () {
                    PlayFabId = _authService.Id
                },
                (result) => {
                    PlayerProfile = result.PlayerProfile;
                    DisplayName = PlayerProfile.DisplayName;
                    resultCallback?.Invoke(result);
                    OnGetPlayerProfile?.Invoke(result);
                },
                (error) => {
                    errorCallback?.Invoke(error);
                    OnPlayFabError?.Invoke(error);
                });
        }

        public void AddOrUpdateContactEmail (Action<AddOrUpdateContactEmailResult> resultCallback = null, Action<PlayFabError> errorCallback = null) {
            PlayFabClientAPI.AddOrUpdateContactEmail(new AddOrUpdateContactEmailRequest () {
                    EmailAddress = _email
                },
                (result) => {
                    resultCallback?.Invoke(result);
                    OnAddOrUpdateContactEmail?.Invoke(result);
                },
                (error) => {
                    errorCallback?.Invoke(error);
                    OnPlayFabError?.Invoke(error);
                });
        }

        public void RemoveContactEmail (Action<RemoveContactEmailResult> resultCallback = null, Action<PlayFabError> errorCallback = null) {
            PlayFabClientAPI.RemoveContactEmail (new RemoveContactEmailRequest (),
                (result) => {
                    resultCallback?.Invoke (result);
                    OnRemoveContactEmail?.Invoke(result);
                },
                (error) => {
                    errorCallback?.Invoke (error);
                });
        }

        public override void GetPlayerProfile(Completer completer)
        {
            GetPlayerProfile((result) =>
            {
                completer.Resolve();
            }, (error) => {
                completer.Reject(new Exception(error.GenerateErrorReport()));
            });
        }
    }
}
#endif
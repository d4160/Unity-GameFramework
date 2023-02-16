using d4160.Logging;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace d4160.PlayFab
{
    public class PlayFabService
    {
        public LoggerSO Logger { get; set; }

        public static PlayFabService Instance => _instance ??= new();
        private static PlayFabService _instance;

        public void LoginWithEmailAddress(LoginWithEmailAddressRequest request, Action<LoginResult> onResult = null, Action<PlayFabError> onError = null)
        {
            PlayFabClientAPI.LoginWithEmailAddress(request,
                (result) => {
                    if (Logger) Logger.LogInfo($"LoginWithEmailAddress(); Result: {result.ToJson()};");
                    onResult?.Invoke(result);
                }, (error) => {
                    if (Logger) Logger.LogError($"LoginWithEmailAddress(); ErrorCode: {error.Error}; ErrorMessage: {error.ErrorMessage};");
                    onError?.Invoke(error);
                });
        }

        public void Logout(Action onLogout)
        {
            PlayFabClientAPI.ForgetAllCredentials();

            if (Logger) Logger.LogInfo($"Logout(); Successful");

            onLogout?.Invoke();
        }

        public void GetUserData(GetUserDataRequest request, Action<GetUserDataResult> onResult = null, Action<PlayFabError> onError = null)
        {
            PlayFabClientAPI.GetUserData(request,
                (result) => {
                    if (Logger) Logger.LogInfo($"GetUserData(); Result: {result.ToJson()};");
                    onResult?.Invoke(result);
                }, (error) => {
                    if (Logger) Logger.LogError($"GetUserData(); ErrorCode: {error.Error}; ErrorMessage: {error.ErrorMessage};");
                    onError?.Invoke(error);
                });
        }

        public void UpdateUserData(UpdateUserDataRequest request, Action<UpdateUserDataResult> onResult = null, Action<PlayFabError> onError = null)
        {
            PlayFabClientAPI.UpdateUserData(request,
                (result) => {
                    if (Logger) Logger.LogInfo($"UpdateUserData(); Result: {result.ToJson()};");
                    onResult?.Invoke(result);
                }, (error) => {
                    if (Logger) Logger.LogError($"UpdateUserData(); ErrorCode: {error.Error}; ErrorMessage: {error.ErrorMessage};");
                    onError?.Invoke(error);
                });
        }

        public void GetPlayerProfile(GetPlayerProfileRequest request, Action<GetPlayerProfileResult> onResult = null, Action<PlayFabError> onError = null)
        {
            PlayFabClientAPI.GetPlayerProfile(request,
                (result) => {
                    if (Logger) Logger.LogInfo($"GetPlayerProfile(); Result: {result.ToJson()};");
                    onResult?.Invoke(result);
                }, (error) => {
                    if (Logger) Logger.LogError($"GetPlayerProfile(); ErrorCode: {error.Error}; ErrorMessage: {error.ErrorMessage};");
                    onError?.Invoke(error);
                });
        }

        public void UpdateUserTitleDisplayName(UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> onResult = null, Action<PlayFabError> onError = null)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                (result) => {
                    if (Logger) Logger.LogInfo($"UpdateUserTitleDisplayName(); Result: {result.ToJson()};");
                    onResult?.Invoke(result);
                }, (error) => {
                    if (Logger) Logger.LogError($"UpdateUserTitleDisplayName(); ErrorCode: {error.Error}; ErrorMessage: {error.ErrorMessage};");
                    onError?.Invoke(error);
                });
        }
    }
}

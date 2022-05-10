#if PLAYFAB
using System;
using System.Collections.Generic;
using d4160.Core;
using d4160.Logging;
using PlayFab;
using PlayFab.AdminModels;
using UnityEngine;

namespace d4160.Persistence.PlayFab
{
    public class PlayFabTitleDataService
    {
        public static event Action<SetTitleDataResult> OnSetTitleDataEvent;
        public static event Action<GetTitleDataResult> OnGetTitleDataEvent;
        public static event Action<PlayFabError> OnPlayFabErrorEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public static PlayFabTitleDataService Instance => _instance ?? (_instance = new PlayFabTitleDataService());
        private static PlayFabTitleDataService _instance;

        public void SetTitleData(string key, string value, Action<SetTitleDataResult> onResult = null, Action<PlayFabError> onError = null) {
            PlayFabAdminAPI.SetTitleData(new SetTitleDataRequest() { 
                Key = key,
                Value = value
            }, (result) => {
                OnSetTitleDataEvent?.Invoke(result);
                onResult?.Invoke(result);
                LoggerM31.LogInfo(result.ToJson(), LogLevel);
            }, (error) => {
                OnPlayFabErrorEvent?.Invoke (error);
                onError?.Invoke(error);
                LoggerM31.LogError(error.GenerateErrorReport(), LogLevel);
            });
        }

        public void GetTitleData(List<string> keys, Action<GetTitleDataResult> onResult = null, Action<PlayFabError> onError = null) {
            PlayFabAdminAPI.GetTitleData(new GetTitleDataRequest() { 
                Keys = keys
            }, (result) => {
                OnGetTitleDataEvent?.Invoke(result);
                onResult?.Invoke(result);
                LoggerM31.LogInfo(result.ToJson(), LogLevel);
            }, (error) => {
                OnPlayFabErrorEvent?.Invoke (error);
                onError?.Invoke(error);
                LoggerM31.LogError(error.GenerateErrorReport(), LogLevel);
            });
        }
    }
}
#endif
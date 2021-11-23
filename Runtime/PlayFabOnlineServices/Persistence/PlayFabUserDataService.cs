#if PLAYFAB
using System;
using System.Collections.Generic;
using d4160.Core;
using d4160.Logging;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace d4160.Persistence.PlayFab
{
    public class PlayFabUserDataService
    {
        public static event Action<UpdateUserDataResult> OnUpdateUserDataEvent;
        public static event Action<GetUserDataResult> OnGetUserDataEvent;
        public static event Action<PlayFabError> OnPlayFabErrorEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public static PlayFabUserDataService Instance => _instance ?? (_instance = new PlayFabUserDataService());
        private static PlayFabUserDataService _instance;

        public void UpdateUserData(UpdateUserDataRequestStruct updateUserDataRequest, Action<UpdateUserDataResult> onResult = null, Action<PlayFabError> onError = null) {
            UpdateUserData(updateUserDataRequest.GetData(), updateUserDataRequest.keysToRemove, updateUserDataRequest.permission, onResult, onError);
        }

        public void UpdateUserData(Dictionary<string,string> data, List<string> keysToRemove, UserDataPermission permission, Action<UpdateUserDataResult> onResult = null, Action<PlayFabError> onError = null) {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() { 
                Data = data,
                KeysToRemove = keysToRemove,
                Permission = permission
            }, (result) => {
                OnUpdateUserDataEvent?.Invoke(result);
                onResult?.Invoke(result);
                LoggerM31.LogInfo(result.ToJson(), LogLevel);
            }, (error) => {
                OnPlayFabErrorEvent?.Invoke (error);
                onError?.Invoke(error);
                LoggerM31.LogError(error.GenerateErrorReport(), LogLevel);
            });
        }

        public void GetUserData(GetUserDataRequest getUserDataRequest, Action<GetUserDataResult> onResult = null, Action<PlayFabError> onError = null) {
            GetUserData(getUserDataRequest.Keys, getUserDataRequest.PlayFabId, getUserDataRequest.IfChangedFromDataVersion, onResult, onError);
        }

        public void GetUserData(List<string> keys, string playFabId = null, uint? ifChangedFromDataVersion = null, Action<GetUserDataResult> onResult = null, Action<PlayFabError> onError = null) {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest() { 
                Keys = keys,
                PlayFabId = playFabId,
                IfChangedFromDataVersion = ifChangedFromDataVersion
            }, (result) => {
                OnGetUserDataEvent?.Invoke(result);
                onResult?.Invoke(result);
                LoggerM31.LogInfo(result.ToJson(), LogLevel);
            }, (error) => {
                OnPlayFabErrorEvent?.Invoke (error);
                onError?.Invoke(error);
                LoggerM31.LogError(error.GenerateErrorReport(), LogLevel);
            });
        }
    }

    [Serializable]
    public struct UpdateUserDataRequestStruct {
        public StringStringStruct[] data;
        public List<string> keysToRemove;
        [Tooltip("Default: Private")]
        public UserDataPermission permission;

        public void AddOrUpdateData(string key, string value){
            bool updated = false;
            for (var i = 0; i < data.Length; i++)
            {
                if (key == data[i].key) {
                    data[i].value = value;
                    updated = true;
                    break;
                }
            }

            if(!updated) {
                var list = new List<StringStringStruct>(data);
                list.Add(new StringStringStruct(key, value));
                data = list.ToArray();
            }
        }

        public string GetData(string key) {
            for (var i = 0; i < data.Length; i++)
            {
                if (key == data[i].key) {
                    return data[i].value;
                }
            }

            return string.Empty;
        }

        public Dictionary<string,string> GetData() {
            var dic = new Dictionary<string,string>();
            for (int i = 0; i < data.Length; i++)
            {
                dic.Add(data[i].key, data[i].value);
            }
            return dic;
        }
    }

    [Serializable]
    public struct StringStringStruct {
        public string key;
        public string value;

        public StringStringStruct(string key, string value){
            this.key = key;
            this.value = value;
        }
    }
}
#endif
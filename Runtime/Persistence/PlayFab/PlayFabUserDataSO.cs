#if PLAYFAB
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace d4160.Persistence.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/Persistence/PlayFab UserData")]
    public class PlayFabUserDataSO : ScriptableObject
    {
        [SerializeField] private UpdateUserDataRequestStruct _updateUserDataRequest;
        [SerializeField] private GetUserDataRequest _getUserDataRequest;

        public event Action<UpdateUserDataResult> OnUpdateUserDataEvent;
        public event Action<GetUserDataResult> OnGetUserDataEvent;
        public event Action<PlayFabError> OnPlayFabErrorEvent;

        public List<string> UserDataKeys => _getUserDataRequest.Keys;
        public string GetData(string key) => _updateUserDataRequest.GetData(key);

        private static PlayFabUserDataService _service = PlayFabUserDataService.Instance;

        private void CallOnUpdateUserDataEvent(UpdateUserDataResult result) => OnUpdateUserDataEvent?.Invoke(result);
        private void CallOnGetUserDataEvent(GetUserDataResult result) {
            foreach (var userRecord in result.Data){
                _updateUserDataRequest.AddOrUpdateData(userRecord.Key, userRecord.Value.Value);
            }
            OnGetUserDataEvent?.Invoke(result);
        }
        private void CallOnPlayFabErrorEvent(PlayFabError error) => OnPlayFabErrorEvent?.Invoke(error);

        public void RegisterEvents () {
            PlayFabUserDataService.OnUpdateUserDataEvent += CallOnUpdateUserDataEvent;
            PlayFabUserDataService.OnGetUserDataEvent += CallOnGetUserDataEvent;
            PlayFabUserDataService.OnPlayFabErrorEvent += CallOnPlayFabErrorEvent;
        }

        public void UnregisterEvents () {
            PlayFabUserDataService.OnUpdateUserDataEvent -= CallOnUpdateUserDataEvent;
            PlayFabUserDataService.OnGetUserDataEvent -= CallOnGetUserDataEvent;
            PlayFabUserDataService.OnPlayFabErrorEvent -= CallOnPlayFabErrorEvent;
        }

        [Button]
        public void UpdateUserData()
        {
            UpdateUserData(_updateUserDataRequest);
        }

        public void UpdateUserData(UpdateUserDataRequestStruct updateUserDataRequest) {
            _service.UpdateUserData(updateUserDataRequest);
        }

        [Button]
        public void GetUserData()
        {
            GetUserData(_getUserDataRequest);
        }

        public void GetUserData(GetUserDataRequest getUserDataRequest) {
            _service.GetUserData(getUserDataRequest);
        }

        public void AddOrUpdateData(string key, string value) { 
            _updateUserDataRequest.AddOrUpdateData(key, value);
        }
    }
}
#endif
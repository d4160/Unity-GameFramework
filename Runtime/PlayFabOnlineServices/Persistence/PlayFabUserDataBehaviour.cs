#if PLAYFAB
using System;
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using PlayFab;
using PlayFab.ClientModels;
using UltEvents;
using UnityEngine;

namespace d4160.Persistence.PlayFab
{
    public class PlayFabUserDataBehaviour : MonoBehaviourUnityData<PlayFabUserDataSO>
    {
        [Header ("EVENTS")]
        [SerializeField] private UltEvent<UpdateUserDataResult> _onUpdateUserDataEvent;
        [SerializeField] private UltEvent<GetUserDataResult> _onGetUserDataEvent;
        [SerializeField] private UltEvent<PlayFabError> _onPlayFabErrorEvent;

        void OnEnable () {
            if (_data)
            {
                _data.RegisterEvents();
                PlayFabUserDataService.OnUpdateUserDataEvent += _onUpdateUserDataEvent.Invoke;
                PlayFabUserDataService.OnGetUserDataEvent += _onGetUserDataEvent.Invoke;
                PlayFabUserDataService.OnPlayFabErrorEvent += _onPlayFabErrorEvent.Invoke;
            }
        }

        void OnDisable () {
            if (_data)
            {
                _data.UnregisterEvents();
                PlayFabUserDataService.OnUpdateUserDataEvent -= _onUpdateUserDataEvent.Invoke;
                PlayFabUserDataService.OnGetUserDataEvent -= _onGetUserDataEvent.Invoke;
                PlayFabUserDataService.OnPlayFabErrorEvent -= _onPlayFabErrorEvent.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UpdateUserData()
        {
            if(_data) 
                _data.UpdateUserData();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void GetUserData()
        {
            if(_data) 
                _data.GetUserData();
        }
    }
}
#endif
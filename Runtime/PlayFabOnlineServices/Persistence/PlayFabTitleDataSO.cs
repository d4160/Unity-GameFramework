#if PLAYFAB
using System;
using System.Collections.Generic;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using PlayFab;
using PlayFab.AdminModels;
using UnityEngine;

namespace d4160.Persistence.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/Persistence/PlayFab TitleData")]
    public class PlayFabTitleDataSO : ScriptableObject
    {
        [SerializeField] private string _key;
        [SerializeField, TextArea] private string _value;
        [SerializeField] private List<string> _getKeys;

        public event Action<SetTitleDataResult> OnSetTitleDataEvent;
        public event Action<GetTitleDataResult> OnGetTitleDataEvent;
        public event Action<PlayFabError> OnPlayFabErrorEvent;

        private static PlayFabTitleDataService _service = PlayFabTitleDataService.Instance;

        private void CallOnSetTitleDataEvent(SetTitleDataResult result) => OnSetTitleDataEvent?.Invoke(result);
        private void CallOnGetTitleDataEvent(GetTitleDataResult result) => OnGetTitleDataEvent?.Invoke(result);
        private void CallOnPlayFabErrorEvent(PlayFabError error) => OnPlayFabErrorEvent?.Invoke(error);

        public void RegisterEvents () {
            PlayFabTitleDataService.OnSetTitleDataEvent += CallOnSetTitleDataEvent;
            PlayFabTitleDataService.OnGetTitleDataEvent += CallOnGetTitleDataEvent;
            PlayFabTitleDataService.OnPlayFabErrorEvent += CallOnPlayFabErrorEvent;
        }

        public void UnregisterEvents () {
            PlayFabTitleDataService.OnSetTitleDataEvent -= CallOnSetTitleDataEvent;
            PlayFabTitleDataService.OnGetTitleDataEvent -= CallOnGetTitleDataEvent;
            PlayFabTitleDataService.OnPlayFabErrorEvent -= CallOnPlayFabErrorEvent;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetTitleData()
        {
            SetTitleData(_key, _value);
        }

        public void SetTitleData(string key, string value, Action<SetTitleDataResult> onResult = null, Action<PlayFabError> onError = null) {
            _service.SetTitleData(key, value, onResult, onError);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void GetTitleData()
        {
            GetTitleData(_getKeys);
        }

        public void GetTitleData(List<string> keys, Action<GetTitleDataResult> onResult = null, Action<PlayFabError> onError = null) 
        {
            _service.GetTitleData(keys, onResult, onError);
        }
    }
}
#endif
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using d4160.Collections;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Requests/GetUserData")]
    public class GetUserDataRequestSO : PlayFabRequestSOBase
    {
        public GetUserDataResultEventSO resultEvent;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [OnValueChanged("_OnRequestInspectorChanged")]
#endif
        public GetUserDataRequest request;
        public List<UserDataRecordVarSO> resultVariables;

#if UNITY_EDITOR
        private void _OnRequestInspectorChanged()
        {
            if (request.Keys == null) return;

            if (request.Keys.Count != resultVariables.Count)
            {
                while (request.Keys.Count < resultVariables.Count)
                {
                    resultVariables.RemoveLast();
                }

                while (request.Keys.Count > resultVariables.Count)
                {
                    resultVariables.Add(default);
                }
            }
        }
#endif

        public void GetUserData()
        {
            _service.GetUserData(request, (result) => {
                for (int i = 0; i < request.Keys.Count; i++)
                {
                    if (result.Data.ContainsKey(request.Keys[i]))
                    {
                        UserDataRecord record = result.Data[request.Keys[i]];
                        resultVariables[i].Value = record;
                    }
                }

                if (resultEvent) resultEvent.Invoke(result);
            }, (error) => {
                if (_errorEvent) _errorEvent.Invoke(error);
            });
        }
    }
}
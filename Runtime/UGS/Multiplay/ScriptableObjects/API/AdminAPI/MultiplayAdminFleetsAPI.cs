using System;
using d4160.UGS.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;

namespace d4160.UGS.Multiplay.AdminAPI
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Admin/Fleets")]
    public class MultiplayAdminFleetsAPI : ScriptableObject
    {
        [SerializeField] private ServiceAccountSO _serviceAccount;
        [SerializeField] private ProjectDataSO _projectData;
        [SerializeField] private MultiplayDataSO _multiplayData;
        [SerializeField] private MultiplaySO _multiplay;

        [Button]
        private void SendViewFleetRequest()
        {
            ViewFleet();
        }

        public void ViewFleet(Action<string> onResult = null, Action<string> onError = null)
        {
            string url = $"https://services.api.unity.com/multiplay/fleets/v1/projects/{_projectData.ProjectId}/environments/{_projectData.EnvironmentId}/fleets/{_multiplayData.FleetId}";

            WebRequests.Get(url,
            (UnityWebRequest unityWebRequest) =>
            {
                unityWebRequest.SetRequestHeader("Authorization", "Basic " + _serviceAccount.KeyBase64);
            },
            (string error) =>
            {
                _multiplay.LogError("Error: " + error);

                onError?.Invoke(error);
            },
            (string json) =>
            {
                _multiplay.LogInfo("Success: " + json);

                onResult?.Invoke(json);
            });
        }

        public void UpdateFleet(UpdateFleetRequest request, Action<string> onResult = null, Action<string> onError = null)
        {
            string url = $"https://services.api.unity.com/multiplay/fleets/v1/projects/{_projectData.ProjectId}/environments/{_projectData.EnvironmentId}/fleets/{_multiplayData.FleetId}";

            string jsonRequest = JsonUtility.ToJson(request);
            Debug.Log(jsonRequest);

            WebRequests.PutJson(url,
            (UnityWebRequest unityWebRequest) =>
            {
                unityWebRequest.SetRequestHeader("Authorization", "Basic " + _serviceAccount.KeyBase64);
            },
            jsonRequest,
            (string error) =>
            {
                _multiplay.LogError("Error: " + error);

                onError?.Invoke(error);
            },
            (string json) =>
            {
                _multiplay.LogInfo("Success: " + json);

                onResult?.Invoke(json);
            });
        }
    }
}
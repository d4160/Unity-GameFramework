using System;
using d4160.UGS.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Token")]
    public class MultiplayTokenAPI : ScriptableObject
    {
        [SerializeField] private ServiceAccountSO _serviceAccount;
        [SerializeField] private ProjectDataSO _projectData;
        [SerializeField] private MultiplaySO _multiplay;

        public void TokenExchange(TokenExchangeRequest request, Action<TokenExchangeResponse> onResult, Action<string> onError = null)
        {
            string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={_projectData.ProjectId}&environmentId={_projectData.EnvironmentId}";

            string jsonRequestBody = JsonUtility.ToJson(request);

            WebRequests.PostJson(url,
            (UnityWebRequest unityWebRequest) =>
            {
                unityWebRequest.SetRequestHeader("Authorization", "Basic " + _serviceAccount.KeyBase64);
            },
            jsonRequestBody,
            (string error) =>
            {
                _multiplay.LogError("Error: " + error);

                onError?.Invoke(error);
            },
            (string json) =>
            {
                _multiplay.LogInfo("Success: " + json);
                TokenExchangeResponse tokenExchangeResponse = JsonUtility.FromJson<TokenExchangeResponse>(json);

                onResult?.Invoke(tokenExchangeResponse);

                // string fleetId = "AAAAAAAAAAAAAAAA";
                // string url = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{projectId}/environments/{environmentId}/fleets/{fleetId}/allocations";

                // WebRequests.PostJson(url,
                // (UnityWebRequest unityWebRequest) =>
                // {
                //     unityWebRequest.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.accessToken);
                // },
                // JsonUtility.ToJson(new QueueAllocationRequest
                // {
                //     allocationId = "AAAAAAAAAAAAA",
                //     buildConfigurationId = 0,
                //     regionId = "AAAAAAAAAAAAAAAAA",
                // }),
                // (string error) =>
                // {
                //     Debug.Log("Error: " + error);
                // },
                // (string json) =>
                // {
                //     Debug.Log("Success: " + json);
                // }
                // );
            });
        }
    }
}
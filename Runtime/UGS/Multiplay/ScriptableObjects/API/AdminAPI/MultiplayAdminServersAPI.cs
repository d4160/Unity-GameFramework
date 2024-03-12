using System;
using d4160.UGS.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;

namespace d4160.UGS.Multiplay.AdminAPI
{
    public enum ServerStatus
    {
        AVAILABLE,
        ONLINE,
        ALLOCATED
    }

    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Admin/Servers")]
    public class MultiplayAdminServersAPI : ScriptableObject
    {
        [SerializeField] private ServiceAccountSO _serviceAccount;
        [SerializeField] private ProjectDataSO _projectData;
        [SerializeField] private MultiplaySO _multiplay;

        [Button]
        private void ListServers()
        {
            ListServers(null);
        }

        public void ListServers(Action<ServerList> onResult, Action<string> onError = null)
        {
            string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{_projectData.ProjectId}/environments/{_projectData.EnvironmentId}/servers";

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
                ServerList serverList = JsonUtility.FromJson<ServerList>("{\"serverList\":" + json + "}");

                onResult?.Invoke(serverList);
            });
        }
    }

    [Serializable]
    public class ServerList
    {
        public Server[] serverList;
    }

    [Serializable]
    public class Server
    {
        public int buildConfigurationID;
        public string buildConfigurationName;
        public string buildName;
        public bool deleted;
        public string fleetID;
        public string fleetName;
        public string hardwareType;
        public int id;
        public string ip;
        public int locationID;
        public string locationName;
        public int machineID;
        public int port;
        public string status;
    }
}
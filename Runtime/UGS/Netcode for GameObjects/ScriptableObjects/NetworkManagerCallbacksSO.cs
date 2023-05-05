using d4160.Events;
using d4160.Logging;
using Unity.Netcode;
using UnityEngine;

namespace d4160.Netcode
{
    [CreateAssetMenu(menuName = "d4160/UGS/NGO/NetworkManagerCallbacks")]
    public class NetworkManagerCallbacksSO : ScriptableObject
    {
        [SerializeField] LoggerSO _logger;
        [SerializeField] ULongEventSO _onClientDisconnect;

        public void RegisterEvents()
        {
            if (NetworkManager.Singleton)
            {
                Debug.Log("Register NetworkManager Events");
                NetworkManager.Singleton.OnClientDisconnectCallback += InvokeOnClientDisconnect;
            }
            else
            {
                Debug.Log("Register NetworkManager Events");
                var netMan = FindObjectOfType<NetworkManager>();
                netMan.OnClientDisconnectCallback += InvokeOnClientDisconnect;
            }
        }

        public void UnregisterEvents()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= InvokeOnClientDisconnect;
            }
        }

        private void InvokeOnClientDisconnect(ulong clientId)
        {
            Debug.Log("InvokeOnClientDisconnect");
            if (_logger) _logger.LogInfo($"[OnClientDisconnect] ClientId: {clientId}");

            if (_onClientDisconnect) _onClientDisconnect.Invoke(clientId);
        }
    }
}
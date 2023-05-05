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
                NetworkManager.Singleton.OnClientDisconnectCallback += InvokeOnClientDisconnect;
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
            if (_onClientDisconnect)
            {
                if (_logger) _logger.LogInfo($"[OnClientDisconnect] ClientId: {clientId}");
                _onClientDisconnect.Invoke(clientId);
            }
        }
    }
}
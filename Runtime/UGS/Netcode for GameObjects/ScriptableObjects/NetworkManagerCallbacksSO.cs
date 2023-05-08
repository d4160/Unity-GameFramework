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
        [SerializeField] VoidEventSO _onTransportFailure;

        public void RegisterEvents()
        {
            //Debug.Log("Register NetworkManager Events Singleton");
            NetworkManager.Singleton.OnClientDisconnectCallback += InvokeOnClientDisconnect;
            NetworkManager.Singleton.OnTransportFailure += InvokeOnTransportFailure;
        }

        public void UnregisterEvents()
        {
            //Debug.Log("UnregisterEvents");
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= InvokeOnClientDisconnect;
                NetworkManager.Singleton.OnTransportFailure -= InvokeOnTransportFailure;
            }
        }

        private void InvokeOnClientDisconnect(ulong clientId)
        {
            if (_logger) _logger.LogInfo($"[OnClientDisconnect] ClientId: {clientId}; LocalClientId: {NetworkManager.Singleton.LocalClientId}");

            if (_onClientDisconnect) _onClientDisconnect.Invoke(clientId);
        }

        private void InvokeOnTransportFailure()
        {
            if (_logger) _logger.LogInfo($"[OnTransportFailure]");

            if (_onTransportFailure) _onTransportFailure.Invoke();
        }
    }
}
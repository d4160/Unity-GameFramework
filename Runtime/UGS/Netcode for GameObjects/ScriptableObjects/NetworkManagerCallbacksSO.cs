using d4160.Events;
using d4160.Logging;
using Unity.Netcode;
using UnityEngine;

namespace d4160.Netcode
{
    [CreateAssetMenu(menuName = "d4160/UGS/NGO/NetworkManagerCallbacks")]
    public class NetworkManagerCallbacksSO : ScriptableObject
    {
        [SerializeField] ULongEventSO _onClientDisconnect;
        [SerializeField] BoolEventSO _onClientStopped;
        [SerializeField] VoidEventSO _onTransportFailure;
        [SerializeField] LoggerSO _logger;

        public void SubscribeEvents()
        {
            //Debug.Log("Register NetworkManager Events Singleton");
            NetworkManager.Singleton.OnClientDisconnectCallback += InvokeOnClientDisconnect;
            NetworkManager.Singleton.OnClientStopped += InvokeOnClientStopped;
            NetworkManager.Singleton.OnTransportFailure += InvokeOnTransportFailure;
        }

        public void UnsubscribeEvents()
        {
            //Debug.Log("UnregisterEvents");
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= InvokeOnClientDisconnect;
                NetworkManager.Singleton.OnTransportFailure -= InvokeOnTransportFailure;
                NetworkManager.Singleton.OnClientStopped -= InvokeOnClientStopped;
            }
        }

        private void InvokeOnClientDisconnect(ulong clientId)
        {
            if (_logger) _logger.LogInfo($"[OnClientDisconnect] ClientId: {clientId}; LocalClientId: {NetworkManager.Singleton.LocalClientId}");

            if (_onClientDisconnect) _onClientDisconnect.Invoke(clientId);
        }

        private void InvokeOnClientStopped(bool isServer)
        {
            if (_logger) _logger.LogInfo($"[OnClientStopped] IsServer: {isServer}");

            if (_onClientStopped) _onClientStopped.Invoke(isServer);
        }

        private void InvokeOnTransportFailure()
        {
            if (_logger) _logger.LogInfo($"[OnTransportFailure]");

            if (_onTransportFailure) _onTransportFailure.Invoke();
        }
    }
}
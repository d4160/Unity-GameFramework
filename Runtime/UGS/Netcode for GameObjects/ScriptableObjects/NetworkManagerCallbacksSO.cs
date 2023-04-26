using d4160.Events;
using Unity.Netcode;
using UnityEngine;

namespace d4160.Netcode
{
    [CreateAssetMenu(menuName = "d4160/UGS/NGO/NetworkManagerCallbacks")]
    public class NetworkManagerCallbacksSO : ScriptableObject
    {
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
                _onClientDisconnect.Invoke(clientId);
            }
        }
    }
}
using UnityEngine;
using d4160.MonoBehaviours;
using d4160.Netcode;
using Unity.Netcode;

namespace d4160.UGS.Authentication
{
    [DefaultExecutionOrder(-5)]
    public class NGOServiceMono : MonoBehaviourUnityData<NetworkManagerCallbacksSO>
    {
        private void Start()
        {
            // Need subscribe events always in Start since NetworkManager uses Awake-OnEnable to init
            _data.SubscribeEvents();
        }

        private void OnDestroy()
        {
            _data.UnsubscribeEvents();
        }

        [ContextMenu("LogShuttingDown")]
        private void LogShuttingDown()
        {
            Debug.Log($"DisconnectReason: {NetworkManager.Singleton.DisconnectReason}; IsConnectedClient:{NetworkManager.Singleton.IsConnectedClient}");
        }
    }
}

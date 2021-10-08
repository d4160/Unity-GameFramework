#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using d4160.Core;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using Photon.Realtime;
using UnityEngine;

namespace d4160.Networking.Photon
{
    [CreateAssetMenu (menuName = "d4160/Networking/Photon Connection")]
    public class PhotonConnectionSO : ScriptableObject
    {
        [SerializeField] private string _region;

        private readonly PhotonConnectionService _connService = PhotonConnectionService.Instance;

        public int Ping => _connService.Ping;
        public bool InConnected => _connService.InConnected;
        public bool InConnectedAndReady => _connService.InConnectedAndReady;
        public int CountOfPlayers => _connService.CountOfPlayers;

        public RegionHandler RegionHandler => _connService.RegionHandler;
        public DisconnectCause LastDisconnectCause => _connService.LastDisconnectCause;

        public event Action OnConnectedEvent;
        public event Action OnConnectedToMasterEvent;
        public event Action<DisconnectCause> OnDisconnectedEvent;
        public event Action<RegionHandler> OnRegionListReceivedEvent;
        public event Action<Dictionary<string, object>> OnCustomAuthenticationResponseEvent;
        public event Action<string> OnCustomAuthenticationFailedEvent;

        private void CallOnConnectedEvent () => OnConnectedEvent?.Invoke ();
        private void CallOnConnectedToMasterEvent () => OnConnectedToMasterEvent?.Invoke ();
        private void CallOnDisconnectedEvent (DisconnectCause cause) => OnDisconnectedEvent?.Invoke (cause);
        private void CallOnRegionListReceivedEvent (RegionHandler region) => OnRegionListReceivedEvent?.Invoke (region);
        private void CallOnCustomAuthenticationResponseEvent (Dictionary<string, object> data) => OnCustomAuthenticationResponseEvent?.Invoke (data);
        private void CallOnCustomAuthenticationFailedEvent (string debugMessage) => OnCustomAuthenticationFailedEvent?.Invoke (debugMessage);

        public void RegisterEvents() {
            PhotonConnectionService.OnConnectedEvent += CallOnConnectedEvent;
            PhotonConnectionService.OnConnectedToMasterEvent += CallOnConnectedToMasterEvent;
            PhotonConnectionService.OnDisconnectedEvent += CallOnDisconnectedEvent;
            PhotonConnectionService.OnRegionListReceivedEvent += CallOnRegionListReceivedEvent;
            PhotonConnectionService.OnCustomAuthenticationResponseEvent += CallOnCustomAuthenticationResponseEvent;
            PhotonConnectionService.OnCustomAuthenticationFailedEvent += CallOnCustomAuthenticationFailedEvent;
        }

        public void UnregisterEvents() {
            PhotonConnectionService.OnConnectedEvent -= CallOnConnectedEvent;
            PhotonConnectionService.OnConnectedToMasterEvent -= CallOnConnectedToMasterEvent;
            PhotonConnectionService.OnDisconnectedEvent -= CallOnDisconnectedEvent;
            PhotonConnectionService.OnRegionListReceivedEvent -= CallOnRegionListReceivedEvent;
            PhotonConnectionService.OnCustomAuthenticationResponseEvent -= CallOnCustomAuthenticationResponseEvent;
            PhotonConnectionService.OnCustomAuthenticationFailedEvent -= CallOnCustomAuthenticationFailedEvent;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ConnectUsingSettings()
        {
            _connService.ConnectUsingSettings();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ConnectToRegion()
        {
            _connService.ConnectToRegion(_region);
        }

        public void ConnectToRegion(string region)
        {
            _connService.ConnectToRegion(region);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Reconnect()
        {
            _connService.Reconnect();
        }

        public void CloseConnection(Player kickPlayer)
        {
            _connService.CloseConnection(kickPlayer);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Disconnect()
        {
            _connService.Disconnect();
        }
    }
}
#endif
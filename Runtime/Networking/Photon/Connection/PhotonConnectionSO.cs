#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using d4160.Core;
using NaughtyAttributes;
using Photon.Realtime;
using UnityEngine;

namespace d4160.Networking.Photon
{
    [CreateAssetMenu (menuName = "d4160/Neuworking/Photon Connection")]
    public class PhotonConnectionSO : ScriptableObject
    {
        [SerializeField] private string _region;

        private readonly PhotonConnectionService _connService = PhotonConnectionService.Instance;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
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

        public void RegisterEvents() {
            PhotonConnectionService.OnConnectedEvent += OnConnectedEvent.Invoke;
            PhotonConnectionService.OnConnectedToMasterEvent += OnConnectedToMasterEvent.Invoke;
            PhotonConnectionService.OnDisconnectedEvent += OnDisconnectedEvent.Invoke;
            PhotonConnectionService.OnRegionListReceivedEvent += OnRegionListReceivedEvent.Invoke;
            PhotonConnectionService.OnCustomAuthenticationResponseEvent += OnCustomAuthenticationResponseEvent.Invoke;
            PhotonConnectionService.OnCustomAuthenticationFailedEvent += OnCustomAuthenticationFailedEvent.Invoke;
        }

        public void UnregisterEvents() {
            PhotonConnectionService.OnConnectedEvent -= OnConnectedEvent.Invoke;
            PhotonConnectionService.OnConnectedToMasterEvent -= OnConnectedToMasterEvent.Invoke;
            PhotonConnectionService.OnDisconnectedEvent -= OnDisconnectedEvent.Invoke;
            PhotonConnectionService.OnRegionListReceivedEvent -= OnRegionListReceivedEvent.Invoke;
            PhotonConnectionService.OnCustomAuthenticationResponseEvent -= OnCustomAuthenticationResponseEvent.Invoke;
            PhotonConnectionService.OnCustomAuthenticationFailedEvent -= OnCustomAuthenticationFailedEvent.Invoke;
        }

        [Button]
        public void ConnectUsingSettings()
        {
            _connService.ConnectUsingSettings();
        }

        [Button]
        public void ConnectToRegion()
        {
            _connService.ConnectToRegion(_region);
        }

        public void ConnectToRegion(string region)
        {
            _connService.ConnectToRegion(region);
        }

        [Button]
        public void Reconnect()
        {
            _connService.Reconnect();
        }

        public void CloseConnection(Player kickPlayer)
        {
            _connService.CloseConnection(kickPlayer);
        }

        [Button]
        public void Disconnect()
        {
            _connService.Disconnect();
        }
    }
}
#endif
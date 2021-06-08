#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections;
using System.Collections.Generic;
using d4160.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Logger = d4160.Logging.Logger;

namespace d4160.Networking.Photon
{
    public class PhotonConnectionService : IConnectionCallbacks
    {
        public static PhotonConnectionService Instance => _instance ?? (_instance = new PhotonConnectionService());
        private static PhotonConnectionService _instance;

        private RegionHandler _regionHandler;
        private DisconnectCause _lastDisconnectCause;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public int Ping => PhotonNetwork.GetPing();
        public bool InConnected => PhotonNetwork.IsConnected;
        public bool InConnectedAndReady => PhotonNetwork.IsConnectedAndReady;
        public int CountOfPlayers => PhotonNetwork.CountOfPlayers;

        public RegionHandler RegionHandler => _regionHandler;
        public DisconnectCause LastDisconnectCause => _lastDisconnectCause;

        public static event Action OnConnectedEvent;
        public static event Action OnConnectedToMasterEvent;
        public static event Action<DisconnectCause> OnDisconnectedEvent;
        public static event Action<RegionHandler> OnRegionListReceivedEvent;
        public static event Action<Dictionary<string, object>> OnCustomAuthenticationResponseEvent;
        public static event Action<string> OnCustomAuthenticationFailedEvent;

        private PhotonConnectionService()
        {
            _instance = this;
        }

        public void SetUp(){
            PhotonNetwork.AutomaticallySyncScene = false;
        }

        public void RegisterEvents()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void UnregisterEvents()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void ConnectUsingSettings()
        {
            if (PhotonNetwork.IsConnected)
            {
                Logger.LogWarning("Is already connected", LogLevel);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void ConnectToRegion(string region)
        {
            if (PhotonNetwork.IsConnected)
            {
                Logger.LogWarning("Is already connected", LogLevel);
            }
            else
            {
                PhotonNetwork.ConnectToRegion(region);
            }
        }

        public void Reconnect()
        {
            PhotonNetwork.Reconnect();
        }

        public void CloseConnection(Player kickPlayer)
        {
            PhotonNetwork.CloseConnection(kickPlayer);
        }

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public void OnConnected()
        {
            OnConnectedEvent?.Invoke();
        }

        public void OnConnectedToMaster()
        {
            OnConnectedToMasterEvent?.Invoke();
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            _lastDisconnectCause = cause;
            OnDisconnectedEvent?.Invoke(cause);
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            _regionHandler = regionHandler;
            OnRegionListReceivedEvent?.Invoke(regionHandler);
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            OnCustomAuthenticationResponseEvent?.Invoke(data);
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            OnCustomAuthenticationFailedEvent?.Invoke(debugMessage);
        }
    }
}
#endif
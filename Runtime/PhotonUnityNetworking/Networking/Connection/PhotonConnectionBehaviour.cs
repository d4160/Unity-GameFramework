#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using d4160.Core;
using d4160.MonoBehaviourData;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using Photon.Realtime;
using UltEvents;
using UnityEngine;

namespace d4160.Networking.Photon
{
    public class PhotonConnectionBehaviour : MonoBehaviourUnityData<PhotonConnectionSO>
    {
        [SerializeField] private UltEvent OnConnectedEvent;
        [SerializeField] private UltEvent OnConnectedToMasterEvent;
        [SerializeField] private UltEvent<DisconnectCause> OnDisconnectedEvent;
        [SerializeField] private UltEvent<RegionHandler> OnRegionListReceivedEvent;
        [SerializeField] private UltEvent<Dictionary<string, object>> OnCustomAuthenticationResponseEvent;
        [SerializeField] private UltEvent<string> OnCustomAuthenticationFailedEvent;

        private void Start(){
            PhotonConnectionService.Instance.RegisterEvents();
            PhotonLobbyService.Instance.RegisterEvents();
            PhotonMatchmakingService.Instance.RegisterEvents();
            PhotonRoomService.Instance.RegisterEvents();
        }

        private void OnDestroy(){
            PhotonConnectionService.Instance.UnregisterEvents();
            PhotonLobbyService.Instance.UnregisterEvents();
            PhotonMatchmakingService.Instance.UnregisterEvents();
            PhotonRoomService.Instance.UnregisterEvents();
        }

        void OnEnable() {

            if (_data)
            {
                _data.RegisterEvents();
                _data.OnConnectedEvent += OnConnectedEvent.Invoke;
                _data.OnConnectedToMasterEvent += OnConnectedToMasterEvent.Invoke;
                _data.OnDisconnectedEvent += OnDisconnectedEvent.Invoke;
                _data.OnRegionListReceivedEvent += OnRegionListReceivedEvent.Invoke;
                _data.OnCustomAuthenticationResponseEvent += OnCustomAuthenticationResponseEvent.Invoke;
                _data.OnCustomAuthenticationFailedEvent += OnCustomAuthenticationFailedEvent.Invoke;
            }
        }

        void OnDisable() {
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnConnectedEvent -= OnConnectedEvent.Invoke;
                _data.OnConnectedToMasterEvent -= OnConnectedToMasterEvent.Invoke;
                _data.OnDisconnectedEvent -= OnDisconnectedEvent.Invoke;
                _data.OnRegionListReceivedEvent -= OnRegionListReceivedEvent.Invoke;
                _data.OnCustomAuthenticationResponseEvent -= OnCustomAuthenticationResponseEvent.Invoke;
                _data.OnCustomAuthenticationFailedEvent -= OnCustomAuthenticationFailedEvent.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ConnectUsingSettings()
        {
            if (_data)
                _data.ConnectUsingSettings();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ConnectToRegion()
        {
            if (_data)
                _data.ConnectToRegion();
        }

        public void ConnectToRegion(string region)
        {
            if (_data)
                _data.ConnectToRegion(region);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Reconnect()
        {
            if (_data)
                _data.Reconnect();
        }

        public void CloseConnection(Player kickPlayer)
        {
            if (_data)
                _data.CloseConnection(kickPlayer);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Disconnect()
        {
            if (_data)
                _data.Disconnect();
        }
    }
}
#endif
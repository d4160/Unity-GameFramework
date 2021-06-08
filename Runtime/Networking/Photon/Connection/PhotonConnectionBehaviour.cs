#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using d4160.Core;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using Photon.Realtime;

namespace d4160.Networking.Photon
{
    public class PhotonConnectionBehaviour : MonoBehaviourUnityData<PhotonConnectionSO>
    {
        public event Action OnConnectedEvent;
        public event Action OnConnectedToMasterEvent;
        public event Action<DisconnectCause> OnDisconnectedEvent;
        public event Action<RegionHandler> OnRegionListReceivedEvent;
        public event Action<Dictionary<string, object>> OnCustomAuthenticationResponseEvent;
        public event Action<string> OnCustomAuthenticationFailedEvent;

        private void Start(){
            PhotonConnectionService.Instance.RegisterEvents();
        }

        private void OnDestroy(){
            PhotonConnectionService.Instance.UnregisterEvents();
        }

        public void RegisterEvents() {

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

        public void UnregisterEvents() {
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

        [Button]
        public void ConnectUsingSettings()
        {
            if (_data)
                _data.ConnectUsingSettings();
        }

        [Button]
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

        [Button]
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

        [Button]
        public void Disconnect()
        {
            if (_data)
                _data.Disconnect();
        }
    }
}
#endif
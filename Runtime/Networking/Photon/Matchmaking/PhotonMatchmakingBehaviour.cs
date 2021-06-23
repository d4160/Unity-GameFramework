#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections;
using System.Collections.Generic;
using d4160.Core;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UltEvents;
using UnityEngine;
using Logger = d4160.Logging.M31Logger;

namespace d4160.Networking.Photon {
    public class PhotonMatchmakingBehaviour : MonoBehaviourUnityData<PhotonMatchmakingSO> {

        [SerializeField] private UltEvent _onCreatedRoomEvent;
        [SerializeField] private UltEvent<short, string> _onCreateRoomFailedEvent;
        [SerializeField] private UltEvent _onJoinedRoomEvent;
        [SerializeField] private UltEvent<short, string> _onJoinRoomFailedEvent;
        [SerializeField] private UltEvent<short, string> _onJoinRandomFailedEvent;
        [SerializeField] private UltEvent _onLeftRoomEvent;
        [SerializeField] private UltEvent<List<FriendInfo>> _onFriendListUpdateEvent;

        void OnEnable () {
            if (_data) {
                _data.RegisterEvents ();
                _data.OnCreatedRoomEvent += _onCreatedRoomEvent.Invoke;
                _data.OnCreateRoomFailedEvent += _onCreateRoomFailedEvent.Invoke;
                _data.OnJoinedRoomEvent += _onJoinedRoomEvent.Invoke;
                _data.OnJoinRoomFailedEvent += _onJoinRoomFailedEvent.Invoke;
                _data.OnJoinRandomFailedEvent += _onJoinRandomFailedEvent.Invoke;
                _data.OnLeftRoomEvent += _onLeftRoomEvent.Invoke;
                _data.OnFriendListUpdateEvent += _onFriendListUpdateEvent.Invoke;
            }
        }

        void OnDisable () {
            if (_data) {
                _data.UnregisterEvents ();
                _data.OnCreatedRoomEvent -= _onCreatedRoomEvent.Invoke;
                _data.OnCreateRoomFailedEvent -= _onCreateRoomFailedEvent.Invoke;
                _data.OnJoinedRoomEvent -= _onJoinedRoomEvent.Invoke;
                _data.OnJoinRoomFailedEvent -= _onJoinRoomFailedEvent.Invoke;
                _data.OnJoinRandomFailedEvent -= _onJoinRandomFailedEvent.Invoke;
                _data.OnLeftRoomEvent -= _onLeftRoomEvent.Invoke;
                _data.OnFriendListUpdateEvent -= _onFriendListUpdateEvent.Invoke;
            }
        }

        [Button]
        public bool JoinRoom () {
            if (_data)
                return _data.JoinRoom ();

            return false;
        }

        [Button]
        public void RejoinRoom () {
            // Need playerTtl to work
            // First Reconnect() and them -> or use ReconnectAndRejoin()
            if (_data)
                _data.JoinRoom ();
        }

        [Button]
        public void LeaveRoom () {
            if (_data)
                _data.JoinRoom ();
        }

        [Button]
        public bool ReconnectAndRejoin () {
            if (_data)
                return _data.JoinRoom ();

            return false;
        }
    }
}
#endif
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
    [CreateAssetMenu (menuName = "d4160/Networking/Photon Lobby")]
    public class PhotonLobbyBehaviour : MonoBehaviourUnityData<PhotonLobbySO> {

        [SerializeField] private UltEvent _onJoinedLobbyEvent;
        [SerializeField] private UltEvent _onLeftLobbyEvent;
        [SerializeField] private UltEvent<List<RoomInfo>> _onRoomListUpdateEvent;
        [SerializeField] private UltEvent<List<TypedLobbyInfo>> _onLobbyStatisticsUpdateEvent;

        void OnEnable () {
            if (_data) {
                _data.RegisterEvents ();
                _data.OnJoinedLobbyEvent += _onJoinedLobbyEvent.Invoke;
                _data.OnLeftLobbyEvent += _onLeftLobbyEvent.Invoke;
                _data.OnRoomListUpdateEvent += _onRoomListUpdateEvent.Invoke;
                _data.OnLobbyStatisticsUpdateEvent += _onLobbyStatisticsUpdateEvent.Invoke;
            }
        }

        void OnDisable () {
            if (_data) {
                _data.UnregisterEvents ();
                _data.OnJoinedLobbyEvent -= _onJoinedLobbyEvent.Invoke;
                _data.OnLeftLobbyEvent -= _onLeftLobbyEvent.Invoke;
                _data.OnRoomListUpdateEvent -= _onRoomListUpdateEvent.Invoke;
                _data.OnLobbyStatisticsUpdateEvent -= _onLobbyStatisticsUpdateEvent.Invoke;
            }
        }

        [Button]
        public void JoinLobby () {
            if (_data) {
                _data.JoinLobby ();
            }
        }

        [Button]
        public void LeaveLobby () {
            if (_data) {
                _data.LeaveLobby ();
            }
        }
    }
}
#endif
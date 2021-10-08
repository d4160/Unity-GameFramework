#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections;
using System.Collections.Generic;
using d4160.Core;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Logger = d4160.Logging.M31Logger;

namespace d4160.Networking.Photon {
    [CreateAssetMenu (menuName = "d4160/Networking/Photon Lobby")]
    public class PhotonLobbySO : ScriptableObject {
        [SerializeField] private JoinLobbyType _joinLobbytype = JoinLobbyType.JoinDefault;
        [SerializeField] private string _lobbyName;

        public JoinLobbyType JoinLobbyType { get => _joinLobbytype; set => _joinLobbytype = value; }
        public string LobbyName { get => _lobbyName; set => _lobbyName = value; }
        public bool EnableLobbyStatistics { get => _lobbyService.EnableLobbyStatistics; set => _lobbyService.EnableLobbyStatistics = value; }

        public TypedLobby CurrentLobby => _lobbyService.CurrentLobby;
        public bool InLobby => _lobbyService.InLobby;
        public List<RoomInfo> RoomList => _lobbyService.RoomList;
        public List<TypedLobbyInfo> LobbyStatistics => _lobbyService.LobbyStatistics;

        public event Action OnJoinedLobbyEvent;
        public event Action OnLeftLobbyEvent;
        public event Action<List<RoomInfo>> OnRoomListUpdateEvent;
        public event Action<List<TypedLobbyInfo>> OnLobbyStatisticsUpdateEvent;

        private void CallOnJoinedLobbyEvent () => OnJoinedLobbyEvent?.Invoke ();
        private void CallOnLeftLobbyEvent () => OnLeftLobbyEvent?.Invoke ();
        private void CallOnRoomListUpdateEvent (List<RoomInfo> rooms) => OnRoomListUpdateEvent?.Invoke (rooms);
        private void CallOnLobbyStatisticsUpdateEvent (List<TypedLobbyInfo> lobbies) => OnLobbyStatisticsUpdateEvent?.Invoke (lobbies);

        private readonly PhotonLobbyService _lobbyService = PhotonLobbyService.Instance;

        public void RegisterEvents () {
            PhotonLobbyService.OnJoinedLobbyEvent += CallOnJoinedLobbyEvent;
            PhotonLobbyService.OnLeftLobbyEvent += CallOnLeftLobbyEvent;
            PhotonLobbyService.OnRoomListUpdateEvent += CallOnRoomListUpdateEvent;
            PhotonLobbyService.OnLobbyStatisticsUpdateEvent += CallOnLobbyStatisticsUpdateEvent;
        }

        public void UnregisterEvents () {
            PhotonLobbyService.OnJoinedLobbyEvent -= CallOnJoinedLobbyEvent;
            PhotonLobbyService.OnLeftLobbyEvent -= CallOnLeftLobbyEvent;
            PhotonLobbyService.OnRoomListUpdateEvent -= CallOnRoomListUpdateEvent;
            PhotonLobbyService.OnLobbyStatisticsUpdateEvent -= CallOnLobbyStatisticsUpdateEvent;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinLobby () {
            _lobbyService.JoinLobbyType = _joinLobbytype;
            _lobbyService.LobbyName = _lobbyName;
            _lobbyService.JoinLobby();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void LeaveLobby (Action onLeftLobby = null) {
            _lobbyService.LeaveLobby (onLeftLobby);
        }

        public bool GetCustomRoomList (TypedLobby lobby, string sqlFilter) {
            // Only for Sql Lobbies, return OnRoomListUpdate
            return _lobbyService.GetCustomRoomList (lobby, sqlFilter);
        }
    }
}
#endif
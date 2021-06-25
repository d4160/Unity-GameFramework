#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections;
using System.Collections.Generic;
using d4160.Collections;
using d4160.Core;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Logger = d4160.Logging.M31Logger;

namespace d4160.Networking.Photon {
    [CreateAssetMenu (menuName = "d4160/Networking/Photon Matchmaking")]
    public class PhotonMatchmakingSO : ScriptableObject {

        [SerializeField] private JoinRoomOptions _joinRoomOption = JoinRoomOptions.JoinRandom;
        [SerializeField] private string _roomName;
        [SerializeField] private bool _becomeInactiveWhenLeaveRoom = true; // Allow return ignoring player TTL
        [SerializeField] private RoomOptionsStruct _roomOptions;
        [SerializeField] private string[] _expectedUsers;
        [Range (0, 255)]
        [SerializeField] private byte _expectedMaxPlayers;
        [SerializeField] private MatchmakingMode _matchmakingMode;
        [SerializeField] private string _sqlLobbyFilter;

        public TypedLobby TypedLobby { get => _matchService.TypedLobby; set => _matchService.TypedLobby = value; }

        public int CountOfPlayersInRooms => _matchService.CountOfPlayersInRooms; // In rooms
        public int CountOfPlayersOnMaster => _matchService.CountOfPlayersOnMaster; // Wait for room
        public int CountOfRooms => _matchService.CountOfRooms;
        public List<FriendInfo> FriendList => _matchService.FriendList;

        public event Action OnCreatedRoomEvent;
        public event Action<short, string> OnCreateRoomFailedEvent;
        public event Action OnJoinedRoomEvent;
        public event Action<short, string> OnJoinRoomFailedEvent;
        public event Action<short, string> OnJoinRandomFailedEvent;
        public event Action OnLeftRoomEvent;
        public event Action<List<FriendInfo>> OnFriendListUpdateEvent;

        public string RoomName { get => _roomName; set => _roomName = value; }
        public string CurrentRoomName => _matchService.CurrentRoomName;

        private void CallOnFriendListUpdateEvent (List<FriendInfo> friendList) => OnFriendListUpdateEvent?.Invoke (friendList);
        private void CallOnCreatedRoomEvent () => OnCreatedRoomEvent?.Invoke ();
        private void CallOnCreateRoomFailedEvent (short returnCode, string message) => OnCreateRoomFailedEvent?.Invoke (returnCode, message);
        private void CallOnJoinedRoomEvent () => OnJoinedRoomEvent?.Invoke ();
        private void CallOnJoinRoomFailedEvent (short returnCode, string message) => OnJoinRoomFailedEvent?.Invoke (returnCode, message);
        private void CallOnJoinRandomFailedEvent (short returnCode, string message) => OnJoinRandomFailedEvent?.Invoke (returnCode, message);
        private void CallOnLeftRoomEvent () => OnLeftRoomEvent?.Invoke ();

        private readonly PhotonMatchmakingService _matchService = PhotonMatchmakingService.Instance;

        public void RegisterEvents () {
            PhotonMatchmakingService.OnCreatedRoomEvent += CallOnCreatedRoomEvent;
            PhotonMatchmakingService.OnCreateRoomFailedEvent += CallOnCreateRoomFailedEvent;
            PhotonMatchmakingService.OnJoinedRoomEvent += CallOnJoinedRoomEvent;
            PhotonMatchmakingService.OnJoinRoomFailedEvent += CallOnJoinRoomFailedEvent;
            PhotonMatchmakingService.OnJoinRandomFailedEvent += CallOnJoinRandomFailedEvent;
            PhotonMatchmakingService.OnLeftRoomEvent += CallOnLeftRoomEvent;
            PhotonMatchmakingService.OnFriendListUpdateEvent += CallOnFriendListUpdateEvent;
        }

        public void UnregisterEvents () {
            PhotonMatchmakingService.OnCreatedRoomEvent -= CallOnCreatedRoomEvent;
            PhotonMatchmakingService.OnCreateRoomFailedEvent -= CallOnCreateRoomFailedEvent;
            PhotonMatchmakingService.OnJoinedRoomEvent -= CallOnJoinedRoomEvent;
            PhotonMatchmakingService.OnJoinRoomFailedEvent -= CallOnJoinRoomFailedEvent;
            PhotonMatchmakingService.OnJoinRandomFailedEvent -= CallOnJoinRandomFailedEvent;
            PhotonMatchmakingService.OnLeftRoomEvent -= CallOnLeftRoomEvent;
            PhotonMatchmakingService.OnFriendListUpdateEvent -= CallOnFriendListUpdateEvent;
        }   

        [Button]
        public bool JoinRoom () {
            _matchService.JoinRoomOptions = _joinRoomOption;
            _matchService.RoomName = _roomName;
            _matchService.ExpectedMaxPlayers = _expectedMaxPlayers;
            _matchService.BecomeInactiveWhenLeaveRoom = _becomeInactiveWhenLeaveRoom;
            _matchService.RoomOptions = null;
            _matchService.MatchmakingMode = _matchmakingMode;
            _matchService.ExpectedUsers = _expectedUsers;

            return _matchService.JoinRoom ();
        }

        [Button]
        public void RejoinRoom () {
            // Need playerTtl to work
            // First Reconnect() and them -> or use ReconnectAndRejoin()
            _matchService.RejoinRoom ();
        }

        [Button]
        public void LeaveRoom () {
            _matchService.LeaveRoom ();
        }

        [Button]
        public bool ReconnectAndRejoin () {
            return _matchService.ReconnectAndRejoin ();
        }

        public bool FindFriends (string[] friendsToFind) {
            return _matchService.FindFriends (friendsToFind);
        }

        public string GetCustoRoomPropKey(int index) {
            if (_roomOptions.customRoomProperties.IsValidIndex(index))
                return _roomOptions.customRoomProperties[index].key;
            return string.Empty;
        }
    }
}
#endif
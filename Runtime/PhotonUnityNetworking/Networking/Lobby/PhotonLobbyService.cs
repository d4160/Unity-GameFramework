#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections;
using System.Collections.Generic;
using d4160.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;

namespace d4160.Networking.Photon {
    public class PhotonLobbyService : ILobbyCallbacks {
        public static PhotonLobbyService Instance => _instance ?? (_instance = new PhotonLobbyService ());
        private static PhotonLobbyService _instance;

        private JoinLobbyType _joinLobbyType = JoinLobbyType.JoinDefault;
        private string _lobbyName;
        private List<RoomInfo> _roomList = null;
        private List<TypedLobbyInfo> _lobbyStatistics = null;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public JoinLobbyType JoinLobbyType { get => _joinLobbyType; set => _joinLobbyType = value; }
        public string LobbyName { get => _lobbyName; set => _lobbyName = value; }
        public bool EnableLobbyStatistics { get => PhotonNetwork.NetworkingClient.EnableLobbyStatistics; set => PhotonNetwork.NetworkingClient.EnableLobbyStatistics = value; }

        public TypedLobby CurrentLobby => PhotonNetwork.CurrentLobby;
        public bool InLobby => PhotonNetwork.InLobby;
        public List<RoomInfo> RoomList => _roomList;
        public List<TypedLobbyInfo> LobbyStatistics => _lobbyStatistics;

        public static event Action OnJoinedLobbyEvent;
        public static event Action OnLeftLobbyEvent;
        public static event Action<List<RoomInfo>> OnRoomListUpdateEvent;
        public static event Action<List<TypedLobbyInfo>> OnLobbyStatisticsUpdateEvent;

        private static event Action OnLeftLobbyEventTemp;

        private PhotonLobbyService () {
            _instance = this;
        }

        public void RegisterEvents () {
            PhotonNetwork.AddCallbackTarget (this);
        }

        public void UnregisterEvents () {
            PhotonNetwork.RemoveCallbackTarget (this);
        }

        public void JoinLobby () {
            CheckAndExecute (() => {
                switch (_joinLobbyType) {
                    case JoinLobbyType.JoinDefault:
                        PhotonNetwork.JoinLobby (TypedLobby.Default);
                        break;
                    case JoinLobbyType.Join:
                    default:
                        PhotonNetwork.JoinLobby (new TypedLobby (_lobbyName, LobbyType.Default));
                        break;
                }
            });
        }

        public void LeaveLobby (Action onLeftLobby = null) {
            CheckAndExecute (() => {
                if (onLeftLobby != null) OnLeftLobbyEventTemp += onLeftLobby.Invoke;
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }
                else {
                    InvokeOnLeftLobbyEventTemp();
                }
            });
        }

        public bool GetCustomRoomList (TypedLobby lobby, string sqlFilter) {
            return CheckAndExecute (() => {
                // Only for Sql Lobbies, return OnRoomListUpdate
                return PhotonNetwork.GetCustomRoomList (lobby, sqlFilter);
            });
        }

        private void CheckAndExecute (Action executeAction) {
            if (Application.isPlaying) {
                executeAction?.Invoke ();
            } else {
                M31Logger.LogWarning ("PHOTON: This function only can be used in playing mode", LogLevel);
            }
        }

        private bool CheckAndExecute (Func<bool> executeAction) {
            if (Application.isPlaying) {
                return executeAction.Invoke ();
            } else {
                M31Logger.LogWarning ("PHOTON: This function only can be used in playing mode", LogLevel);
                return false;
            }
        }

        public void OnJoinedLobby () {
            M31Logger.LogInfo ("PHOTON: OnJoinedLobby", LogLevel);
            OnJoinedLobbyEvent?.Invoke ();
        }

        public void OnLeftLobby () {
            InvokeOnLeftLobbyEventTemp();
            M31Logger.LogInfo ("PHOTON: OnLeftLobby", LogLevel);
            OnLeftLobbyEvent?.Invoke ();
        }

        public void OnRoomListUpdate (List<RoomInfo> roomList) {
            M31Logger.LogInfo ($"PHOTON: OnRoomListUpdate, RoomList count: {roomList.Count}", LogLevel);
            _roomList = roomList;
            OnRoomListUpdateEvent?.Invoke (roomList);
        }

        public void OnLobbyStatisticsUpdate (List<TypedLobbyInfo> lobbyStatistics) {
            M31Logger.LogInfo ("PHOTON: OnLobbyStatisticsUpdate", LogLevel);
            _lobbyStatistics = lobbyStatistics;
            OnLobbyStatisticsUpdateEvent?.Invoke (lobbyStatistics);
        }

        private void InvokeOnLeftLobbyEventTemp() {
            OnLeftLobbyEventTemp?.Invoke();
            OnLeftLobbyEventTemp = null;
        }
    }

    public enum JoinLobbyType {
        JoinDefault,
        Join
    }
}
#endif
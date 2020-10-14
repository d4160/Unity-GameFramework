#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace d4160.GameFramework.Networking
{
    public enum JoinRoomOptions
    {
        JoinRandom,
        Create,
        JoinOrCreate,
        Join
    }

    public enum JoinLobbyOptions
    {
        JoinDefault,
        Join
    }

    public class Photon2NetworkManager : BasePhoton2NetworkManager<Photon2NetworkManager>
    {
        [SerializeField] protected bool _automaticallySyncScene = false;

        [Foldout("Room")]
        [SerializeField] protected JoinRoomOptions _joinRoomOption = JoinRoomOptions.JoinRandom;
        [ShowIf("IsRoomNameNeeded")]
        [Foldout("Room")]
        [SerializeField] protected string _roomName;
        [Foldout("Room")]
        [SerializeField] protected byte _maxPlayersPerRoom = 4;
        [Foldout("Room")]
        [SerializeField] protected bool _becomeInactiveWhenLeaveRoom = true;

        [Foldout("Lobby")]
        [SerializeField] protected JoinLobbyOptions _joinLobbyOption = JoinLobbyOptions.JoinDefault;
        [ShowIf("IsLobbyNameNeeded")]
        [Foldout("Lobby")]
        [SerializeField] protected string _lobbyName;

        public bool IsConnected => PhotonNetwork.IsConnected;
        public bool IsConnectedAndReady => PhotonNetwork.IsConnectedAndReady;
        public bool IsMasterClient => PhotonNetwork.IsMasterClient;
        public bool IsMessageQueueRunning
        {
            get => PhotonNetwork.IsMessageQueueRunning;
            set => PhotonNetwork.IsMessageQueueRunning = value;
        }

#if UNITY_EDITOR
        private bool IsRoomNameNeeded => _joinRoomOption != JoinRoomOptions.JoinRandom;
        private bool IsLobbyNameNeeded => _joinLobbyOption != JoinLobbyOptions.JoinDefault;
#endif

        protected override void Awake()
        {
            base.Awake();

            PhotonNetwork.AutomaticallySyncScene = _automaticallySyncScene;
        }
        
        [Button]
        public void SetAutomaticallySyncScene()
        {
            PhotonNetwork.AutomaticallySyncScene = _automaticallySyncScene;
        }

        [Button]
        public virtual void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Is already connected");
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        [Button]
        public virtual void Reconnect()
        {
            PhotonNetwork.Reconnect();
        }

        [Button]
        public virtual void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        [Button]
        public virtual int GetPing()
        {
            var ping = PhotonNetwork.GetPing();
            Debug.Log($"Ping: {ping}");
            
            return ping;
        }

        public override void OnConnected()
        {
            Debug.Log($"OnConnected, connected: {PhotonNetwork.IsConnected} and ready: {PhotonNetwork.IsConnectedAndReady}");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log($"OnConnectedToMaster, connected: {PhotonNetwork.IsConnected} and ready: {PhotonNetwork.IsConnectedAndReady}");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"OnDisconnected, cause: {cause}");
        }

        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log($"OnRegionListReceived, regions: {regionHandler.GetResults()}");
        }

        public override void OnJoinedLobby()
        {
            Debug.Log($"OnJoinedLobby");
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.Log($"OnLobbyStatisticsUpdate, Capacity: {lobbyStatistics.Capacity}, Count: {lobbyStatistics.Count}");
        }

        public override void OnLeftLobby()
        {
            Debug.Log($"OnLeftLobby");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            Debug.Log($"OnCreatedRoom");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            Debug.Log($"OnCreateRoomFailed: returnCode: {returnCode}, message: {message}");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log($"OnJoinedRoom");
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            Debug.Log($"OnLeftRoom");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            Debug.Log($"OnJoinRoomFailed: returnCode: {returnCode}, message: {message}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            
            Debug.Log($"OnJoinRandomFailed: returnCode: {returnCode}, message: {message}");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);

            Debug.Log($"OnRoomListUpdate: count: {roomList.Count}, capacity: {roomList.Capacity}");
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            Debug.Log($"OnRoomPropertiesUpdate: count: {propertiesThatChanged.Count}");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            Debug.Log($"OnPlayerEnteredRoom: {newPlayer.NickName}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            Debug.Log($"OnPlayerLeftRoom: {otherPlayer.NickName}");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            Debug.Log($"OnPlayerPropertiesUpdate: TargetPlayer: {targetPlayer.NickName}, changedProps count: {changedProps.Count}");
        }

        [Button]
        public void JoinLobby()
        {
            switch (_joinLobbyOption)
            {
                case JoinLobbyOptions.JoinDefault:
                    PhotonNetwork.JoinLobby(TypedLobby.Default);
                    break;
                case JoinLobbyOptions.Join:
                default:
                    PhotonNetwork.JoinLobby(new TypedLobby(_lobbyName, LobbyType.Default));
                    break;
            }
        }

        [Button]
        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }

        [Button]
        public virtual void JoinRoom()
        {
            switch (_joinRoomOption)
            {
                case JoinRoomOptions.JoinRandom:
                    PhotonNetwork.JoinRandomRoom();
                    break;
                case JoinRoomOptions.Create:
                    PhotonNetwork.CreateRoom(_roomName, new Photon.Realtime.RoomOptions() { MaxPlayers = _maxPlayersPerRoom }, TypedLobby.Default);
                    break;
                case JoinRoomOptions.JoinOrCreate:
                    PhotonNetwork.JoinOrCreateRoom(_roomName, new Photon.Realtime.RoomOptions() { MaxPlayers = _maxPlayersPerRoom }, TypedLobby.Default);
                    break;
                case JoinRoomOptions.Join:
                default:
                    PhotonNetwork.JoinRoom(_roomName);
                    break;
            }
        }

        [Button]
        public virtual void RejoinRoom()
        {
            PhotonNetwork.RejoinRoom(_roomName);
        }

        [Button]
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom(_becomeInactiveWhenLeaveRoom);
        }

        [Button]
        public virtual void ReconnectAndRejoin()
        {
            PhotonNetwork.ReconnectAndRejoin();
        }
    }
}
#endif
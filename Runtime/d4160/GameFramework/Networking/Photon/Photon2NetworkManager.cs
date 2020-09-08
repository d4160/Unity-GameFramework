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
    public enum NetworkAction
    {
        None,
        JoinLobby,
        JoinRoom
    }

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
        [SerializeField] protected NetworkAction _onConnectedAction = NetworkAction.None;
        [ShowIf("IsJoinRoomActionSelected")]
        [SerializeField] protected JoinRoomOptions JoinRoomOption = JoinRoomOptions.JoinRandom;
        [ShowIf("IsJoinLobbyActionSelected")]
        [SerializeField] protected JoinLobbyOptions _joinLobbyOption = JoinLobbyOptions.JoinDefault;
        [ShowIf("IsNeededJoinName")]
        [SerializeField] protected string _roomOrLobbyName;
        [SerializeField] protected byte _maxPlayersPerRoom = 4;

        public bool IsConnected => PhotonNetwork.IsConnected;
        public bool IsConnectedAndReady => PhotonNetwork.IsConnectedAndReady;

#if UNITY_EDITOR
        private bool IsJoinRoomActionSelected => _onConnectedAction == NetworkAction.JoinRoom;
        private bool IsJoinLobbyActionSelected => _onConnectedAction == NetworkAction.JoinLobby;
        private bool IsNeededJoinName => (IsJoinRoomActionSelected && JoinRoomOption != JoinRoomOptions.JoinRandom) ||
        (IsJoinLobbyActionSelected && _joinLobbyOption != JoinLobbyOptions.JoinDefault);
#endif

        protected override void Awake()
        {
            base.Awake();

            PhotonNetwork.AutomaticallySyncScene = _automaticallySyncScene;

            //Debug.Log(photonView.IsMine);
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
        public virtual void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnConnected()
        {
            Debug.Log($"OnConnected, connected: {PhotonNetwork.IsConnected}");
            Debug.Log($"OnConnected, connected and ready: {PhotonNetwork.IsConnectedAndReady}");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log($"OnConnectedToMaster, connected: {PhotonNetwork.IsConnected}");
            Debug.Log($"OnConnectedToMaster, connected and ready: {PhotonNetwork.IsConnectedAndReady}");

            if (PhotonNetwork.IsConnectedAndReady)
            {
                switch (_onConnectedAction)
                {
                    case NetworkAction.JoinRoom:
                        JoinRoom();
                        break;
                    case NetworkAction.JoinLobby:
                        JoinLobby();
                        break;
                    case NetworkAction.None:
                    default:
                        break;
                }
            }
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
            Debug.Log($"OnJoinedLobby, connected: {PhotonNetwork.IsConnected}");
            Debug.Log($"OnJoinedLobby, connected and ready: {PhotonNetwork.IsConnectedAndReady}");
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            
        }

        public override void OnLeftLobby()
        {
            
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }

        [Button]
        public virtual void JoinRoom()
        {
            switch (JoinRoomOption)
            {
                case JoinRoomOptions.JoinRandom:
                    PhotonNetwork.JoinRandomRoom();
                    break;
                case JoinRoomOptions.Create:
                    PhotonNetwork.CreateRoom(_roomOrLobbyName, new Photon.Realtime.RoomOptions() { MaxPlayers = _maxPlayersPerRoom }, TypedLobby.Default);
                    break;
                case JoinRoomOptions.JoinOrCreate:
                    PhotonNetwork.JoinOrCreateRoom(_roomOrLobbyName, new Photon.Realtime.RoomOptions() { MaxPlayers = _maxPlayersPerRoom }, TypedLobby.Default);
                    break;
                case JoinRoomOptions.Join:
                default:
                    PhotonNetwork.JoinRoom(_roomOrLobbyName);
                    break;
            }
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
                    PhotonNetwork.JoinLobby(new TypedLobby(_roomOrLobbyName, LobbyType.Default));
                    break;
            }
        }
    }
}
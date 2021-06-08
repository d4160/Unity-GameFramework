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
    public class PhotonMatchmakingService : IMatchmakingCallbacks
    {
        public static PhotonMatchmakingService Instance => _instance ?? (_instance = new PhotonMatchmakingService());
        private static PhotonMatchmakingService _instance;

        private JoinRoomOptions _joinRoomOption = JoinRoomOptions.JoinRandom;
        private string _roomName;
        private bool _becomeInactiveWhenLeaveRoom = true; // Allow return ignoring player TTL

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public RoomOptionsStruct? RoomOptions { get; set; } = null;
        public JoinRoomOptions JoinRoomOptions { get => _joinRoomOption; set => _joinRoomOption = value; }
        public string RoomName { get => _roomName; set => _roomName = value; }
        public bool BecomeInactiveWhenLeaveRoom { get => _becomeInactiveWhenLeaveRoom; set => _becomeInactiveWhenLeaveRoom = value; }
        public TypedLobby TypedLobby { get; set; } = TypedLobby.Default;
        public string[] ExpectedUsers { get; set; }
        public byte ExpectedMaxPlayers { get; set; } = 0;
        public MatchmakingMode MatchmakingMode { get; set; } = MatchmakingMode.FillRoom;
        public string SqlLobbyFilter { get; set; } 

        public int CountOfPlayersInRooms => PhotonNetwork.CountOfPlayersInRooms; // In rooms
        public int CountOfPlayersOnMaster => PhotonNetwork.CountOfPlayersOnMaster; // Wait for room
        public int CountOfRooms => PhotonNetwork.CountOfRooms;
        public List<FriendInfo> FriendList { get; private set; }

        public static event Action<List<FriendInfo>> OnFriendListUpdateEvent;
        public static event Action OnCreatedRoomEvent;
        public static event Action<short, string> OnCreateRoomFailedEvent;
        public static event Action OnJoinedRoomEvent;
        public static event Action<short, string> OnJoinRoomFailedEvent;
        public static event Action<short, string> OnJoinRandomFailedEvent;
        public static event Action OnLeftRoomEvent;

        private PhotonMatchmakingService()
        {
            _instance = this;
        }

        public void RegisterEvents()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void UnregisterEvents()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public bool JoinRoom()
        {
            switch (_joinRoomOption)
            {
                case JoinRoomOptions.JoinRandom:
                    return PhotonNetwork.JoinRandomRoom(RoomOptions.HasValue ? RoomOptions.Value.MakePhotonHashtable() : null, ExpectedMaxPlayers, MatchmakingMode, TypedLobby, SqlLobbyFilter, ExpectedUsers);
                case JoinRoomOptions.Create:
                    return PhotonNetwork.CreateRoom(_roomName, RoomOptions.HasValue ? RoomOptions.Value.GetRoomOptions() : null, TypedLobby, ExpectedUsers);
                case JoinRoomOptions.JoinOrCreate:
                    return PhotonNetwork.JoinOrCreateRoom(_roomName, RoomOptions.HasValue ? RoomOptions.Value.GetRoomOptions() : null, TypedLobby, ExpectedUsers);
                case JoinRoomOptions.Join:
                default:
                    return PhotonNetwork.JoinRoom(_roomName, ExpectedUsers);
            }
        }

        public void RejoinRoom()
        {
            // Need playerTtl to work
            // First Reconnect() and them -> or use ReconnectAndRejoin()
            PhotonNetwork.RejoinRoom(_roomName);
        }

        public void LeaveRoom()
        {
            // becomeInactiveWhenLeaveRoom used for possibility to Rejoin
            PhotonNetwork.LeaveRoom(_becomeInactiveWhenLeaveRoom);
        }

        public bool ReconnectAndRejoin()
        {
            return PhotonNetwork.ReconnectAndRejoin();
        }

        public bool FindFriends(string[] friendsToFind){
            return PhotonNetwork.FindFriends(friendsToFind);
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            FriendList =  friendList;
            OnFriendListUpdateEvent?.Invoke(friendList);
        }

        public void OnCreatedRoom()
        {
            OnCreatedRoomEvent?.Invoke();
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            OnCreateRoomFailedEvent?.Invoke(returnCode, message);
        }

        public void OnJoinedRoom()
        {
            OnJoinedRoomEvent?.Invoke();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            OnJoinRoomFailedEvent?.Invoke(returnCode, message);
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            OnJoinRandomFailedEvent?.Invoke(returnCode, message);
        }

        public void OnLeftRoom()
        {
            OnLeftRoomEvent?.Invoke();
        }
    }

    public enum JoinRoomOptions
    {
        JoinRandom,
        Create,
        JoinOrCreate,
        Join
    }

    [Serializable]
    public struct RoomOptionsStruct
    {
        public bool isVisible; // true, if false no random only name
        public bool isOpen; // true
        [Range(0, 255)]
        public byte maxPlayers; // 0 no limit
        public int playerTtl; // time to live millis
        public int emptyRoomTtl; // millis
        public bool cleanupCacheOnLeave; // true default, user events & properties when leave
        public HashtableStruct[] customRoomProperties; // string key, short as possible
        public string[] customRoomPropertiesForLobby; // inside the hashtable the properties to share in lobby
        public string[] plugins; // = new string[0];
        public bool suppressRoomEvents; // events for joining and leaving players
        public bool suppressPlayerInfo; // events and property broadcast
        public bool publishUserId; // useful for FindFriends
        public bool deleteNullProperties;
        public bool broadcastPropsChangeToAll; //= true, avoid de-sync

        public ExitGames.Client.Photon.Hashtable CustomRoomProperties { get; private set; }

        public ExitGames.Client.Photon.Hashtable MakePhotonHashtable(){
            CustomRoomProperties = HashtableStruct.GetPhotonHashtable(customRoomProperties);
            return CustomRoomProperties;
        }

        public RoomOptions GetRoomOptions(){
            MakePhotonHashtable();
            return new RoomOptions() { 
                IsVisible = isVisible,
                IsOpen = isOpen,
                MaxPlayers = maxPlayers,
                PlayerTtl = playerTtl,
                EmptyRoomTtl = emptyRoomTtl,
                CleanupCacheOnLeave = cleanupCacheOnLeave,
                CustomRoomProperties = CustomRoomProperties,
                CustomRoomPropertiesForLobby = customRoomPropertiesForLobby,
                Plugins = plugins,
                SuppressRoomEvents = suppressRoomEvents,
                SuppressPlayerInfo = suppressPlayerInfo,
                PublishUserId = publishUserId,
                DeleteNullProperties = deleteNullProperties,
                BroadcastPropsChangeToAll = broadcastPropsChangeToAll
            };
        }
    }

    [Serializable]
    public struct HashtableStruct {
        public string key;
        [Tooltip("The type to try to parse from value")]
        public CommonType valueType;
        public string value;

        private int IntValue => int.Parse(value);
        private float FloatValue => float.Parse(value);
        private bool BoolValue => bool.Parse(value);
        private char CharValue => char.Parse(value);

        public object GetValue(){
            switch(valueType){
                case CommonType.Boolean: return BoolValue;
                case CommonType.Integer: return IntValue;
                case CommonType.Float: return FloatValue;
                case CommonType.Char: return CharValue;
                case CommonType.String: default: return value;
            }
        }

        public static ExitGames.Client.Photon.Hashtable GetPhotonHashtable(HashtableStruct[] hastableArray) {

            if (hastableArray.Length > 0)
            {
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                for (var i = 0; i < hastableArray.Length; i++)
                {
                    hashtable.Add(hastableArray[i].key, hastableArray[i].GetValue());
                }

                return hashtable;
            }

            return null;
        }
    }
}
#endif
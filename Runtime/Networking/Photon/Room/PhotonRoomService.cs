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
    public class PhotonRoomService : IInRoomCallbacks
    {
        public static PhotonRoomService Instance => _instance ?? (_instance = new PhotonRoomService());
        private static PhotonRoomService _instance;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public HashtableStruct[] CustomRoomProperties { get; set; }
        public HashtableStruct[] ExpectedProperties { get; set; }

        public bool InRoom => PhotonNetwork.InRoom;
        public Room CurrentRoom => PhotonNetwork.CurrentRoom;
        public Dictionary<int, Player> Players => CurrentRoom.Players; // id|actorNumber
        public int PlayerCount => CurrentRoom.PlayerCount;
        public Player[] PlayerList => PhotonNetwork.PlayerList; // In room also
        public Player[] PlayerListOthers => PhotonNetwork.PlayerListOthers; 
        public byte MaxPlayers { get => CurrentRoom.MaxPlayers; set => CurrentRoom.MaxPlayers = value; }
        public int PlayerTtl { get => CurrentRoom.PlayerTtl; set => CurrentRoom.PlayerTtl = value; }
        public int EmptyRoomTtl { get => CurrentRoom.EmptyRoomTtl; set => CurrentRoom.EmptyRoomTtl = value; }

        public static event Action<Player> OnPlayerEnteredRoomEvent;
        public static event Action<Player> OnPlayerLeftRoomEvent;
        public static event Action<ExitGames.Client.Photon.Hashtable> OnRoomPropertiesUpdateEvent;
        public static event Action<Player, ExitGames.Client.Photon.Hashtable> OnPlayerPropertiesUpdateEvent;
        public static event Action<Player> OnMasterClientSwitchedEvent;

        private PhotonRoomService()
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

        public GameObject InstantiateRoomObject(string prefab, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.InstantiateRoomObject(prefab, position, rotation, group, data);
        }

        public bool SetCustomProperties()
        {
            return CurrentRoom.SetCustomProperties(HashtableStruct.GetPhotonHashtable(CustomRoomProperties), HashtableStruct.GetPhotonHashtable(ExpectedProperties));
        }

        public bool SetMasterClient(Player masterClientPlayer){
            return CurrentRoom.SetMasterClient(masterClientPlayer);
        }

        public bool SetPropertiesListedInLobby(string[] lobbyProps) {
            return CurrentRoom.SetPropertiesListedInLobby(lobbyProps);
        }

        public virtual Player GetPlayer(int id, bool findMaster = false){ // id 0 return master
            return CurrentRoom.GetPlayer(id, findMaster);
        }

        public virtual bool AddPlayer(Player player){ 
            return CurrentRoom.AddPlayer(player);
        }

        public virtual bool ClearExpectedUsers(){ 
            return CurrentRoom.ClearExpectedUsers();
        }

        public virtual bool SetExpectedUsers(string[] newExpectedUsers){ 
            return CurrentRoom.SetExpectedUsers(newExpectedUsers);
        }

        public void AllocateRoomViewID(PhotonView view)
        {
            PhotonNetwork.AllocateRoomViewID(view);
        }

        public bool SetPlayerCustomProperties(ExitGames.Client.Photon.Hashtable customProperties) {
            return PhotonNetwork.SetPlayerCustomProperties(customProperties);
        }

        public void RemovePlayerCustomProperties(string[] customPropertiesToDelete) {
            PhotonNetwork.RemovePlayerCustomProperties(customPropertiesToDelete);
        }

        public void DestroyPlayerObjects(Player targetPlayer) {
            PhotonNetwork.DestroyPlayerObjects(targetPlayer);
        }

        public void DestroyAll() {
            PhotonNetwork.DestroyAll();
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            OnPlayerLeftRoomEvent?.Invoke(otherPlayer);
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            OnRoomPropertiesUpdateEvent?.Invoke(propertiesThatChanged);
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            OnPlayerPropertiesUpdateEvent?.Invoke(targetPlayer, changedProps);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            OnMasterClientSwitchedEvent?.Invoke(newMasterClient);
        }
    }
}
#endif
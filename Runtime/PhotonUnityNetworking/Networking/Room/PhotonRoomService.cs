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
    public class PhotonRoomService : IInRoomCallbacks {
        public static PhotonRoomService Instance => _instance ?? (_instance = new PhotonRoomService ());
        private static PhotonRoomService _instance;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public HashtableStruct[] CustomRoomProperties { get; set; }
        public HashtableStruct[] ExpectedProperties { get; set; }

        private Player[] _playerList; // when leftroom, this become null by default
        private Player[] _playerListOthers; // when leftroom, this become null by default

        public bool InRoom => PhotonNetwork.InRoom;
        public Room CurrentRoom => PhotonNetwork.CurrentRoom;
        public Dictionary<int, Player> Players => CurrentRoom.Players; // id|actorNumber
        public int PlayerCount => CurrentRoom.PlayerCount;
        public Player[] PlayerList => _playerList == null ? PhotonNetwork.PlayerList : _playerList; // In room also
        public Player[] PlayerListOthers => _playerListOthers == null ? PhotonNetwork.PlayerListOthers : _playerListOthers;
        public byte MaxPlayers { get => CurrentRoom.MaxPlayers; set => CurrentRoom.MaxPlayers = value; }
        public int PlayerTtl { get => CurrentRoom.PlayerTtl; set => CurrentRoom.PlayerTtl = value; }
        public int EmptyRoomTtl { get => CurrentRoom.EmptyRoomTtl; set => CurrentRoom.EmptyRoomTtl = value; }

        public static event Action<Player> OnPlayerEnteredRoomEvent;
        public static event Action<Player> OnPlayerLeftRoomEvent;
        public static event Action<ExitGames.Client.Photon.Hashtable> OnRoomPropertiesUpdateEvent;
        public static event Action<Player, ExitGames.Client.Photon.Hashtable> OnPlayerPropertiesUpdateEvent;
        public static event Action<Player> OnMasterClientSwitchedEvent;

        private PhotonRoomService () {
            _instance = this;
        }

        public void RegisterEvents () {
            PhotonNetwork.AddCallbackTarget (this);
        }

        public void UnregisterEvents () {
            PhotonNetwork.RemoveCallbackTarget (this);
        }

        public GameObject InstantiateRoomObject (string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null) {
            return CheckAndExecute (() => {
                return PhotonNetwork.InstantiateRoomObject (prefabName, position, rotation, group, data);
            });
        }

        public bool SetCurrentRoomCustomProperties () {
            return CheckAndExecute (() => {
                // Expected for example is expected to have winner=0, so winner now is 3 player
                return CurrentRoom.SetCustomProperties (HashtableStruct.GetPhotonHashtable (CustomRoomProperties), HashtableStruct.GetPhotonHashtable (ExpectedProperties));
            });
        }

        public bool SetMasterClient (Player masterClientPlayer) {
            return CheckAndExecute (() => {
                return CurrentRoom.SetMasterClient (masterClientPlayer);
            });
        }

        public bool SetPropertiesListedInLobby (string[] lobbyProps) {
            return CheckAndExecute (() => {
                return CurrentRoom.SetPropertiesListedInLobby (lobbyProps);
            });
        }

        public Player GetPlayer (int id, bool findMaster = false) { // id 0 return master
            return CheckAndExecute (() => {
                return CurrentRoom.GetPlayer (id, findMaster);
            });
        }

        public bool AddPlayer (Player player) {
            return CheckAndExecute (() => {
                return CurrentRoom.AddPlayer (player);
            });
        }

        public bool ClearExpectedUsers () {
            return CheckAndExecute (() => {
                return CurrentRoom.ClearExpectedUsers ();
            });
        }

        public bool SetExpectedUsers (string[] newExpectedUsers) {
            return CheckAndExecute (() => {
                return CurrentRoom.SetExpectedUsers (newExpectedUsers);
            });
        }

        public void AllocateRoomViewID (PhotonView view) {
            CheckAndExecute (() => {
                PhotonNetwork.AllocateRoomViewID (view);
            });
        }

        public bool SetPlayerCustomProperties (ExitGames.Client.Photon.Hashtable customProperties) {
            return CheckAndExecute (() => {
                return PhotonNetwork.SetPlayerCustomProperties (customProperties);
            });
        }

        public void RemovePlayerCustomProperties (string[] customPropertiesToDelete) {
            CheckAndExecute (() => {
                PhotonNetwork.RemovePlayerCustomProperties (customPropertiesToDelete);
            });
        }

        public void DestroyPlayerObjects (Player targetPlayer) {
            CheckAndExecute (() => {
                PhotonNetwork.DestroyPlayerObjects (targetPlayer);
            });
        }

        public void DestroyAll () {
            CheckAndExecute (() => {
                PhotonNetwork.DestroyAll ();
            });
        }

        private void CheckAndExecute (Action executeAction) {
            if (Application.isPlaying) {
                executeAction?.Invoke ();
            } else {
                M31Logger.LogWarning ("PHOTON: This function only can be used in playing mode", LogLevel);
            }
        }

        private T CheckAndExecute<T> (Func<T> executeAction) {
            if (Application.isPlaying) {
                return executeAction.Invoke ();
            } else {
                M31Logger.LogWarning ("PHOTON: This function only can be used in playing mode", LogLevel);
                return default;
            }
        }

        public void OnPlayerEnteredRoom (Player newPlayer) {
            _playerList = PhotonNetwork.PlayerList;
            _playerListOthers = PhotonNetwork.PlayerListOthers;
            OnPlayerEnteredRoomEvent?.Invoke (newPlayer);
            M31Logger.LogInfo ("PHOTON: OnPlayerEnteredRoom", LogLevel);
        }

        public void OnPlayerLeftRoom (Player otherPlayer) {
            _playerList = PhotonNetwork.PlayerList;
            _playerListOthers = PhotonNetwork.PlayerListOthers;
            OnPlayerLeftRoomEvent?.Invoke (otherPlayer);
            M31Logger.LogInfo ("PHOTON: OnPlayerLeftRoom", LogLevel);
        }

        public void OnRoomPropertiesUpdate (ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
            OnRoomPropertiesUpdateEvent?.Invoke (propertiesThatChanged);
            M31Logger.LogInfo ("PHOTON: OnRoomPropertiesUpdate", LogLevel);
        }

        public void OnPlayerPropertiesUpdate (Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
            OnPlayerPropertiesUpdateEvent?.Invoke (targetPlayer, changedProps);
            M31Logger.LogInfo ("PHOTON: OnPlayerPropertiesUpdate", LogLevel);
        }

        public void OnMasterClientSwitched (Player newMasterClient) {
            OnMasterClientSwitchedEvent?.Invoke (newMasterClient);
            M31Logger.LogInfo ("PHOTON: OnMasterClientSwitched", LogLevel);
        }
    }
}
#endif
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
    public class PhotonRoomBehaviour : MonoBehaviourUnityData<PhotonRoomSO> {

        [SerializeField] private UltEvent<Player> _onPlayerEnteredRoomEvent;
        [SerializeField] private UltEvent<Player> _onPlayerLeftRoomEvent;
        [SerializeField] private UltEvent<ExitGames.Client.Photon.Hashtable> _onRoomPropertiesUpdateEvent;
        [SerializeField] private UltEvent<Player, ExitGames.Client.Photon.Hashtable> _onPlayerPropertiesUpdateEvent;
        [SerializeField] private UltEvent<Player> _onMasterClientSwitchedEvent;

        private readonly PhotonRoomService _roomService = PhotonRoomService.Instance;

        void OnEnable () {
            if(_data) {
                _data.RegisterEvents();
                PhotonRoomService.OnPlayerEnteredRoomEvent += _onPlayerEnteredRoomEvent.Invoke;
                PhotonRoomService.OnPlayerLeftRoomEvent += _onPlayerLeftRoomEvent.Invoke;
                PhotonRoomService.OnRoomPropertiesUpdateEvent += _onRoomPropertiesUpdateEvent.Invoke;
                PhotonRoomService.OnPlayerPropertiesUpdateEvent += _onPlayerPropertiesUpdateEvent.Invoke;
                PhotonRoomService.OnMasterClientSwitchedEvent += _onMasterClientSwitchedEvent.Invoke;
            }
        }

        void OnDisable () {
            if(_data) {
                _data.UnregisterEvents();
                PhotonRoomService.OnPlayerEnteredRoomEvent -= _onPlayerEnteredRoomEvent.Invoke;
                PhotonRoomService.OnPlayerLeftRoomEvent -= _onPlayerLeftRoomEvent.Invoke;
                PhotonRoomService.OnRoomPropertiesUpdateEvent -= _onRoomPropertiesUpdateEvent.Invoke;
                PhotonRoomService.OnPlayerPropertiesUpdateEvent -= _onPlayerPropertiesUpdateEvent.Invoke;
                PhotonRoomService.OnMasterClientSwitchedEvent -= _onMasterClientSwitchedEvent.Invoke;
            }
        }

        public GameObject InstantiateRoomObject (string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null) {
            if(_data)
                return _data.InstantiateRoomObject(prefabName, position, rotation, group, data);
            return null;
        }

        [Button("SetRoomCustomProperties")]
        public bool SetCustomProperties () {
            if(_data)
                return _data.SetCustomProperties();
            return false;
        }

        public bool SetMasterClient (Player masterClientPlayer) {
            if(_data)
                return _data.SetMasterClient(masterClientPlayer);
            return false;
        }

        public bool SetPropertiesListedInLobby (string[] lobbyProps) {
            if(_data)
                return _data.SetPropertiesListedInLobby(lobbyProps);
            return false;
        }

        public Player GetPlayer (int id, bool findMaster = false) { // id 0 return master
            if(_data)
                return _data.GetPlayer(id, findMaster);
            return null;
        }

        public bool AddPlayer (Player player) {
            if(_data)
                return _data.AddPlayer(player);
            return false;
        }

        public bool ClearExpectedUsers () {
            if(_data)
                return _data.ClearExpectedUsers();
            return false;
        }

        public bool SetExpectedUsers (string[] newExpectedUsers) {
            if(_data)
                return _data.SetExpectedUsers(newExpectedUsers);
            return false;
        }

        public void AllocateRoomViewID (PhotonView view) {
            if(_data)
                _data.AllocateRoomViewID(view);
        }

        public bool SetPlayerCustomProperties (ExitGames.Client.Photon.Hashtable customProperties) {
            if(_data)
                return _data.SetPlayerCustomProperties(customProperties);
            return false;
        }

        public void RemovePlayerCustomProperties (string[] customPropertiesToDelete) {
            if(_data)
                _data.RemovePlayerCustomProperties(customPropertiesToDelete);
        }

        public void DestroyPlayerObjects (Player targetPlayer) {
            if(_data)
                _data.DestroyPlayerObjects(targetPlayer);
        }

        public void DestroyAll () {
            if(_data)
                _data.DestroyAll();
        }
    }
}
#endif
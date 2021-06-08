#if PHOTON_UNITY_NETWORKING
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace d4160.SceneManagement
{
    public partial class SceneManagerSO : ScriptableObject, IInRoomCallbacks, IMatchmakingCallbacks
    {
        [Header("PHOTON")]
        [SerializeField] private bool _automaticallySyncScene = true; 

        private bool _loadingLevelAndPausedNetwork = false;

        const string CurrentSceneProperty = "curScn";
        const string CurrentScenePropertyLoadAsync = "curScnLa";
        
        public bool AutomaticallySyncScene
        {
            get
            {
                return _automaticallySyncScene;
            }
            set
            {
                _automaticallySyncScene = value;
                if (_automaticallySyncScene && PhotonNetwork.CurrentRoom != null)
                {
                    LoadLevelIfSynced();
                }
            }
        }

        private void OnCollectionLoadedCallbackPhoton(int index, string label) {
            // Debug.Log($"Index: {index}. Label: {label}");
            if (_loadingLevelAndPausedNetwork)
            {
                _loadingLevelAndPausedNetwork = false;
                PhotonNetwork.IsMessageQueueRunning = true;
            }
            else
            {
                SetLevelInPropsIfSynced(index);
            }
        }

        private void OnOperation(OperationResponse opResponse)
        {
            switch (opResponse.OperationCode)
            {
                case OperationCode.JoinGame:
                    if (PhotonNetwork.Server == ServerConnection.GameServer)
                    {
                        LoadLevelIfSynced();
                    }
                    break;
            }
        }


        public void LoadLevelIfSynced()
        {
            if (!AutomaticallySyncScene || PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            // check if "current level" is set in props
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentSceneProperty))
            {
                return;
            }

            // if loaded level is not the one defined by master in props, load that level
            object sceneId = PhotonNetwork.CurrentRoom.CustomProperties[CurrentSceneProperty];
            if (sceneId is int)
            {
                if (_lastLoadedIndex != (int)sceneId)
                {
                    LoadSceneCollectionAsync((int)sceneId);
                }
            }
            else if (sceneId is string)
            {
                if (_lastLoadedLabel != (string)sceneId)
                {
                    LoadSceneCollectionAsync((string)sceneId);
                }
            }
        }

        public void SetLevelInPropsIfSynced(object levelId)
        {
            if (!AutomaticallySyncScene || !PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }
            if (levelId == null)
            {
                Debug.LogError("Parameter levelId can't be null!");
                return;
            }

            // check if "current level" is already set in the room properties (then we don't set it again)
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentSceneProperty))
            {
                object levelIdInProps = PhotonNetwork.CurrentRoom.CustomProperties[CurrentSceneProperty];
                //Debug.Log("levelId (to set): "+ levelId + " levelIdInProps: " + levelIdInProps + " SceneManagerHelper.ActiveSceneName: "+ SceneManagerHelper.ActiveSceneName);

                if (levelId.Equals(levelIdInProps))
                {
                    //Debug.LogWarning("The levelId equals levelIdInProps. Don't set property again.");
                    return;
                }
                else
                {
                    // if the new levelId does not equal the level in properties, there is a chance that build index and scene name refer to the same scene.
                    // as Unity does not provide all scenes with build index, we only check for the currently loaded scene (with a high chance this is the correct one).
                    int scnIndex = _lastLoadedIndex;
                    string scnName = _lastLoadedLabel;

                    if ((levelId.Equals(scnIndex) && levelIdInProps.Equals(scnName)) || (levelId.Equals(scnName) && levelIdInProps.Equals(scnIndex)))
                    {
                        //Debug.LogWarning("The levelId and levelIdInProps refer to the same scene. Don't set property for it.");
                        return;
                    }
                }
            }

            
            // if the new levelId does not match the current room-property, we can cancel existing loading (as we start a new one)
            // if (_AsyncLevelLoadingOperation != null)
            // {
            //     if (!_AsyncLevelLoadingOperation.isDone)
            //     {
            //         Debug.LogWarning("PUN cancels an ongoing async level load, as another scene should be loaded. Next scene to load: " + levelId);
            //     }

            //     _AsyncLevelLoadingOperation.allowSceneActivation = false;
            //     _AsyncLevelLoadingOperation = null;
            // }


            // current level is not yet in props, or different, so this client has to set it
            ExitGames.Client.Photon.Hashtable setScene = new ExitGames.Client.Photon.Hashtable();
            if (levelId is int) setScene[CurrentSceneProperty] = (int)levelId;
            else if (levelId is string) setScene[CurrentSceneProperty] = (string)levelId;
            else Debug.LogError("Parameter levelId must be int or string!");

            PhotonNetwork.CurrentRoom.SetCustomProperties(setScene);
            PhotonNetwork.SendAllOutgoingCommands(); // send immediately! because: in most cases the client will begin to load and pause sending anything for a while
        }


        public void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            LoadLevelIfSynced();
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public void OnCreatedRoom()
        {
            SetLevelInPropsIfSynced(_lastLoadedIndex);
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinedRoom()
        {
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public void OnLeftRoom()
        {
        }
    }
}
#endif
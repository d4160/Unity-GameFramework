#if PHOTON_UNITY_NETWORKING
using System.Collections;
using System.Collections.Generic;
using d4160.Core.MonoBehaviours;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace d4160.GameFramework.Networking
{
    public class BasePhoton2NetworkManager<T> : Singleton<T>, IConnectionCallbacks , IMatchmakingCallbacks , IInRoomCallbacks, ILobbyCallbacks, IWebRpcCallback, IErrorInfoCallback where T : MonoBehaviour
    {
        /// <summary>Cache field for the PhotonView on this GameObject.</summary>
        private PhotonView pvCache;

        /// <summary>A cached reference to a PhotonView on this GameObject.</summary>
        /// <remarks>
        /// If you intend to work with a PhotonView in a script, it's usually easier to write this.photonView.
        ///
        /// If you intend to remove the PhotonView component from the GameObject but keep this Photon.MonoBehaviour,
        /// avoid this reference or modify this code to use PhotonView.Get(obj) instead.
        /// </remarks>
        public PhotonView photonView
        {
            get
            {
#if UNITY_EDITOR
                // In the editor we want to avoid caching this at design time, so changes in PV structure appear immediately.
                if (!Application.isPlaying || this.pvCache == null)
                {
                    this.pvCache = PhotonView.Get(this);
                }
#else
                if (this.pvCache == null)
                {
                    this.pvCache = PhotonView.Get(this);
                }
#endif
                return this.pvCache;
            }
        }

        //#if UNITY_EDITOR
        //protected virtual void Reset()
        //{
        //    this.pvCache = this.transform.GetParentComponent<PhotonView>();

        //    if (this.pvCache == null)
        //    {
        //        Debug.LogWarning(this.GetType().Name + " requires a PhotonView. No PhotonView was found, so one is being added to GameObject '" + this.transform.root.name + "'");
        //        this.pvCache = this.transform.root.gameObject.AddComponent<PhotonView>();
        //    }
        //}
        //#endif

        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        /// <summary>
        /// Called to signal that the raw connection got established but before the client can call operation on the server.
        /// </summary>
        /// <remarks>
        /// After the (low level transport) connection is established, the client will automatically send
        /// the Authentication operation, which needs to get a response before the client can call other operations.
        ///
        /// Your logic should wait for either: OnRegionListReceived or OnConnectedToMaster.
        ///
        /// This callback is useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnDisconnected().
        ///
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        public virtual void OnConnected()
        {
        }

        /// <summary>
        /// Called when the local user/client left a room, so the game's logic can clean up it's internal state.
        /// </summary>
        /// <remarks>
        /// When leaving a room, the LoadBalancingClient will disconnect the Game Server and connect to the Master Server.
        /// This wraps up multiple internal actions.
        ///
        /// Wait for the callback OnConnectedToMaster, before you use lobbies and join or create rooms.
        /// </remarks>
        public virtual void OnLeftRoom()
        {
        }

        /// <summary>
        /// Called after switching to a new MasterClient when the current one leaves.
        /// </summary>
        /// <remarks>
        /// This is not called when this client enters a room.
        /// The former MasterClient is still in the player list when this method get called.
        /// </remarks>
        public virtual void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        /// <summary>
        /// Called when the server couldn't create a room (OpCreateRoom failed).
        /// </summary>
        /// <remarks>
        /// The most common cause to fail creating a room, is when a title relies on fixed room-names and the room already exists.
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        public virtual void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        /// <summary>
        /// Called when a previous OpJoinRoom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        public virtual void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see OpCreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", implement OnMasterClientSwitched()
        /// and make each new MasterClient check the room's state.
        /// </remarks>
        public virtual void OnCreatedRoom()
        {
        }

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
        /// </summary>
        /// <remarks>
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify in the public cloud).
        /// The room list gets available via OnRoomListUpdate.
        /// </remarks>
        public virtual void OnJoinedLobby()
        {
        }

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [OpCreateRoom](@ref OpCreateRoom) and [OpJoinRandomRoom](@ref OpJoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        public virtual void OnLeftLobby()
        {
        }

        /// <summary>
        /// Called after disconnecting from the Photon server. It could be a failure or intentional
        /// </summary>
        /// <remarks>
        /// The reason for this disconnect is provided as DisconnectCause.
        /// </remarks>
        public virtual void OnDisconnected(DisconnectCause cause)
        {
        }

        /// <summary>
        /// Called when the Name Server provided a list of regions for your title.
        /// </summary>
        /// <remarks>Check the RegionHandler class description, to make use of the provided values.</remarks>
        /// <param name="regionHandler">The currently used RegionHandler.</param>
        public virtual void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (InLobby) on the Master Server.
        /// </summary>
        /// <remarks>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        public virtual void OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }

        /// <summary>
        /// Called when the LoadBalancingClient entered a room, no matter if this client created it or simply joined.
        /// </summary>
        /// <remarks>
        /// When this is called, you can access the existing players in Room.Players, their custom properties and Room.CustomProperties.
        ///
        /// In this callback, you could create player objects. For example in Unity, instantiate a prefab for the player.
        ///
        /// If you want a match to be started "actively", enable the user to signal "ready" (using OpRaiseEvent or a Custom Property).
        /// </remarks>
        public virtual void OnJoinedRoom()
        {
        }

        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public virtual void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <remarks>
        /// If another player leaves the room or if the server detects a lost connection, this callback will
        /// be used to notify your game logic.
        ///
        /// Depending on the room's setup, players may become inactive, which means they may return and retake
        /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
        ///
        /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
        /// the callback is called.
        /// </remarks>
        public virtual void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        /// <summary>
        /// Called when a previous OpJoinRandom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        ///
        /// When using multiple lobbies (via OpJoinLobby or a TypedLobby parameter), another lobby might have more/fitting rooms.<br/>
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        public virtual void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        /// <summary>
        /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
        /// </summary>
        /// <remarks>
        /// The list of available rooms won't become available unless you join a lobby via LoadBalancingClient.OpJoinLobby.
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// </remarks>
        public virtual void OnConnectedToMaster()
        {
        }

        /// <summary>
        /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
        /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="propertiesThatChanged"></param>
        public virtual void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
        }

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Changing properties must be done by Player.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        ///
        /// <param name="targetPlayer">Contains Player that changed.</param>
        /// <param name="changedProps">Contains the properties that changed.</param>
        public virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        /// <summary>
        /// Called when the server sent the response to a FindFriends request.
        /// </summary>
        /// <remarks>
        /// After calling OpFindFriends, the Master Server will cache the friend list and send updates to the friend
        /// list. The friends includes the name, userId, online state and the room (if any) for each requested user/friend.
        ///
        /// Use the friendList to update your UI and store it, if the UI should highlight changes.
        /// </remarks>
        public virtual void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        /// <summary>
        /// Called when your Custom Authentication service responds with additional data.
        /// </summary>
        /// <remarks>
        /// Custom Authentication services can include some custom data in their response.
        /// When present, that data is made available in this callback as Dictionary.
        /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
        /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
        ///
        /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/realtime/current/reference/custom-authentication"/>
        public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://dashboard.photonengine.com)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development.</param>
        public virtual void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        //TODO: Check if this needs to be implemented
        // in: IOptionalInfoCallbacks
        public virtual void OnWebRpcResponse(OperationResponse response)
        {
        }

        //TODO: Check if this needs to be implemented
        // in: IOptionalInfoCallbacks
        public virtual void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }

        /// <summary>
        /// Called when the client receives an event from the server indicating that an error happened there.
        /// </summary>
        /// <remarks>
        /// In most cases this could be either:
        /// 1. an error from webhooks plugin (if HasErrorInfo is enabled), read more here:
        /// https://doc.photonengine.com/en-us/realtime/current/gameplay/web-extensions/webhooks#options
        /// 2. an error sent from a custom server plugin via PluginHost.BroadcastErrorInfoEvent, see example here:
        /// https://doc.photonengine.com/en-us/server/current/plugins/manual#handling_http_response
        /// 3. an error sent from the server, for example, when the limit of cached events has been exceeded in the room
        /// (all clients will be disconnected and the room will be closed in this case)
        /// read more here: https://doc.photonengine.com/en-us/realtime/current/gameplay/cached-events#special_considerations
        /// </remarks>
        /// <param name="errorInfo">object containing information about the error</param>
        public virtual void OnErrorInfo(ErrorInfo errorInfo)
        {
        }
    }
}
#endif
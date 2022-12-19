using d4160.Events;
using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/Events/ConnectionFailed")]
public class ConnectionFailedEventSO : EventSOBase<NetworkRunner, NetAddress, NetConnectFailedReason>
{
}

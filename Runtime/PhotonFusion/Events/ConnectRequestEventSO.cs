using d4160.Events;
using Fusion.Sockets;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/Events/ConnectRequest")]
public class ConnectRequestEventSO : EventSOBase<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]>
{
}

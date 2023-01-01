using d4160.Events;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/Events/ReliableDataReceived")]
public class ReliableDataReceivedEventSO : EventSOBase<NetworkRunner, PlayerRef, ArraySegment<byte>>
{
}

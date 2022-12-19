using d4160.Events;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/Events/Shutdown")]
public class ShutdownEventSO : EventSOBase<NetworkRunner, ShutdownReason>
{
}

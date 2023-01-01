using d4160.Events;
using Fusion;
using System;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/ReliableDataReceived")]
    public class ReliableDataReceivedEventSO : EventSOBase<NetworkRunner, PlayerRef, ArraySegment<byte>>
    {
    }
}

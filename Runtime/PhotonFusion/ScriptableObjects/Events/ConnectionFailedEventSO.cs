using d4160.Events;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/ConnectionFailed")]
    public class ConnectionFailedEventSO : EventSOBase<NetworkRunner, NetAddress, NetConnectFailedReason>
    {
    }
}
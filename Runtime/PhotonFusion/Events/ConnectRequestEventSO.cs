using d4160.Events;
using Fusion.Sockets;
using Fusion;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/ConnectRequest")]
    public class ConnectRequestEventSO : EventSOBase<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]>
    {
    }
}
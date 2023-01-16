using d4160.Events;
using Fusion;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/NetworkRunnerPlayer")]
    public class NetworkRunnerPlayerEventSO : EventSOBase<NetworkRunner, PlayerRef>
    {
    }
}
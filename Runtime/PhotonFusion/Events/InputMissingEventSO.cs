using d4160.Events;
using Fusion;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/InputMissing")]
    public class InputMissingEventSO : EventSOBase<NetworkRunner, PlayerRef, NetworkInput>
    {
    }
}
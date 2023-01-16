using d4160.Events;
using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/CustomAuthenticationResponse")]
    public class CustomAuthenticationResponseEventSO : EventSOBase<NetworkRunner, Dictionary<string, object>>
    {
    }
}
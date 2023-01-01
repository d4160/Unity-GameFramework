using d4160.Events;
using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Events/SessionListUpdated")]
    public class SessionListUpdatedEventSO : EventSOBase<NetworkRunner, List<SessionInfo>>
    {
    }
}
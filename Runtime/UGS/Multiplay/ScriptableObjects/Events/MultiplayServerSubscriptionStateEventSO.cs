#if DEDICATED_SERVER
using UnityEngine;
using d4160.Events;
using Unity.Services.Multiplay;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Events/MultiplayServerSubscriptionState")]
    public class MultiplayServerSubscriptionStateEventSO : EventSOBase<MultiplayServerSubscriptionState>
    {

    }
}
#endif
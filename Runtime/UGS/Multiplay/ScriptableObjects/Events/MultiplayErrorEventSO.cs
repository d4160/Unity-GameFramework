#if DEDICATED_SERVER
using d4160.Events;
using Unity.Services.Multiplay;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Events/MultiplayError")]
    public class MultiplayErrorEventSO : EventSOBase<MultiplayError>
    {

    }
}
#endif
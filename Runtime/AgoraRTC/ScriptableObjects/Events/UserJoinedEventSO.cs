using Agora.Rtc;
using UnityEngine;
using d4160.Events;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Events/UserJoined")]
    public class UserJoinedEventSO : EventSOBase<RtcConnection, uint, int>
    {

    }
}
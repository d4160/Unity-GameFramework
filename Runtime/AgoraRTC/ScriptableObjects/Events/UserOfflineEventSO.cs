using Agora.Rtc;
using d4160.Events;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Events/UserOffline")]
    public class UserOfflineEventSO : EventSOBase<RtcConnection, uint, USER_OFFLINE_REASON_TYPE>
    {

    }
}
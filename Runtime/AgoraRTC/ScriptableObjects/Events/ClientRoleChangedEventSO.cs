using Agora.Rtc;
using UnityEngine;
using d4160.Events;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Events/ClientRoleChanged")]
    public class ClientRoleChangedEventSO : EventSOBase<RtcConnection, CLIENT_ROLE_TYPE, CLIENT_ROLE_TYPE, ClientRoleOptions>
    {

    }
}
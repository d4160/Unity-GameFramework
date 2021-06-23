#if AGORA
using agora_gaming_rtc;
using UnityEngine;
using d4160.MonoBehaviourData;
using UltEvents;

namespace d4160.Chat.Agora
{
    public class AgoraUserBehaviour : MonoBehaviourData<AgoraUserSO>
    {
        [SerializeField] private UltEvent<uint, int> OnUserJoined;
        [SerializeField] private UltEvent<uint, USER_OFFLINE_REASON> OnUserOffline;

        public void RegisterEvents () {
            AgoraUserService.OnUserJoinedEvent += OnUserJoined.Invoke;
            AgoraUserService.OnUserOfflineEvent += OnUserOffline.Invoke;
        }

        public void UnregisterEvents(){
            AgoraUserService.OnUserJoinedEvent -= OnUserJoined.Invoke;
            AgoraUserService.OnUserOfflineEvent -= OnUserOffline.Invoke;
        }   
    }
}
#endif
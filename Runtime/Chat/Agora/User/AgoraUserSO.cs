#if AGORA
using System;
using agora_gaming_rtc;
using UnityEngine;
using System.Collections.Generic;
using d4160.Instancers;

namespace d4160.Chat.Agora
{
    [CreateAssetMenu(menuName = "d4160/Chat/Agora User")]
    public class AgoraUserSO : ScriptableObject
    {
        public event Action<uint, int> OnUserJoinedEvent;
        public event Action<uint, USER_OFFLINE_REASON> OnUserOfflineEvent;

        private readonly AgoraUserService _userService = AgoraUserService.Instance; 

        public Dictionary<uint, VideoSurface> UserVideoDict => _userService.UserVideoDict;

        public IProvider<VideoSurface> VideoSurfaceProvider { get => _userService.VideoSurfaceProvider; set => _userService.VideoSurfaceProvider = value; }
        public float OtherVideoSurfaceScaleMultiplier { get => _userService.OtherVideoSurfaceScaleMultiplier; set => _userService.OtherVideoSurfaceScaleMultiplier = value; }

        private void CallOnUserJoined(uint uid, int elapsed) => OnUserJoinedEvent?.Invoke(uid, elapsed);

        private void CallOnUserOfflineEvent(uint uid, USER_OFFLINE_REASON reason) => OnUserOfflineEvent?.Invoke(uid, reason);

        public void RegisterEvents () {
            AgoraUserService.OnUserJoinedEvent += CallOnUserJoined;
            AgoraUserService.OnUserOfflineEvent += CallOnUserOfflineEvent;
        }

        public void UnregisterEvents(){
            AgoraUserService.OnUserJoinedEvent -= CallOnUserJoined;
            AgoraUserService.OnUserOfflineEvent -= CallOnUserOfflineEvent;
        }   
    }
}
#endif
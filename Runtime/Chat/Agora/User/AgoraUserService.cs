#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using M31Logger = d4160.Logging.M31Logger;
using System.Collections.Generic;
using d4160.Instancers;

namespace d4160.Chat.Agora
{
    public class AgoraUserService
    {
        public static event Action<uint, int> OnUserJoinedEvent;
        public static event Action<uint, USER_OFFLINE_REASON> OnUserOfflineEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraAuthService _authService = AgoraAuthService.Instance; 
        public static AgoraUserService Instance => _instance ?? (_instance = new AgoraUserService());
        private static AgoraUserService _instance;

        private Dictionary<uint, VideoSurface> _userVideoDict = new Dictionary<uint, VideoSurface>();
        public Dictionary<uint, VideoSurface> UserVideoDict => _userVideoDict;

        public IProvider<VideoSurface> VideoSurfaceProvider { get; set; }
        public float OtherVideoSurfaceScaleMultiplier { get; set; }

        private void CallOnUserJoined(uint uid, int elapsed)
        {
            M31Logger.LogInfo("onUserJoined: uid = " + uid + " elapsed = " + elapsed, LogLevel);
            
            if (CheckErrors()) return;

            VideoSurface videoSurface = VideoSurfaceProvider.Instantiate();
            if (!ReferenceEquals(videoSurface, null))
            {
                // configure videoSurface
                videoSurface.transform.localScale = Vector3.one * OtherVideoSurfaceScaleMultiplier;
                videoSurface.SetForUser(uid);
                videoSurface.SetEnable(true);
                videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                videoSurface.SetGameFps(30);
                videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
                UserVideoDict[uid] = videoSurface;
                //Vector2 pos = AgoraUIUtils.GetRandomPosition(100);
                //videoSurface.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            }

            OnUserJoinedEvent?.Invoke(uid, elapsed);
        }

        private void CallOnUserOfflineEvent(uint uid, USER_OFFLINE_REASON reason)
        {
            M31Logger.LogInfo("onUserOffline: uid = " + uid + " reason = " + reason, LogLevel);
            
            if (UserVideoDict.ContainsKey(uid))
            {
                var surface = UserVideoDict[uid];
                surface.SetEnable(false);
                UserVideoDict.Remove(uid);
                if (VideoSurfaceProvider != null)
                    VideoSurfaceProvider.Destroy(surface);
                else
                    GameObject.Destroy(surface.gameObject);
            }

            OnUserOfflineEvent?.Invoke(uid, reason);
        }

        private AgoraUserService()
        {
            _instance = this;
        }

        public void RegisterEvents () {
            _authService.RtcEngine.OnUserJoined += CallOnUserJoined;
            _authService.RtcEngine.OnUserOffline += CallOnUserOfflineEvent;
        }

        public void UnregisterEvents(){
            _authService.RtcEngine.OnUserJoined -= CallOnUserJoined;
            _authService.RtcEngine.OnUserOffline -= CallOnUserOfflineEvent;
        }   

        private bool CheckErrors() {
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_authService.RtcEngine == null) {
                M31Logger.LogWarning("AGORA: RtcEngine is null", LogLevel);
                return true;
            }

            if (VideoSurfaceProvider == null) {
                M31Logger.LogWarning("AGORA: VideoSurfaceProvider is null", LogLevel);
                return true;
            }

            return false;
        }
    }
}
#endif
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
        public AgoraVideoSurfaceType AgoraVideoSurfaceType { get; set; } = AgoraVideoSurfaceType.Renderer;
        public uint VideoFps { get; set; } = 30;
        public bool EnableFlipHorizontal { get; set; } = true;
        public bool EnableFlipVertical { get; set; } = false;

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance; 
        public static AgoraUserService Instance => _instance ?? (_instance = new AgoraUserService());
        private static AgoraUserService _instance;

        private Dictionary<uint, VideoSurface> _userVideoDict = new Dictionary<uint, VideoSurface>();
        public Dictionary<uint, VideoSurface> UserVideoDictionary => _userVideoDict;

        public ComponentProviderSOBase VideoSurfaceProvider { get; set; }
        public float OtherVideoSurfaceScaleMultiplier { get; set; } // 0.25f

        private void CallOnUserJoined(uint uid, int elapsed)
        {
            M31Logger.LogInfo("onUserJoined: uid = " + uid + " elapsed = " + elapsed, LogLevel);
            
            if (CheckErrors()) return;

            VideoSurface videoSurface = VideoSurfaceProvider.InstantiateAs<VideoSurface>();
            if (!ReferenceEquals(videoSurface, null))
            {
                // configure videoSurface
                videoSurface.transform.localScale = Vector3.one * OtherVideoSurfaceScaleMultiplier;
                videoSurface.SetForUser(uid);
                videoSurface.SetEnable(true);
                videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType);
                videoSurface.SetGameFps(VideoFps);
                videoSurface.EnableFilpTextureApply(EnableFlipHorizontal, EnableFlipVertical);
                UserVideoDictionary[uid] = videoSurface;
                //Vector2 pos = AgoraUIUtils.GetRandomPosition(100);
                //videoSurface.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            }
            else {
                M31Logger.LogInfo("onUserJoined: uid = " + uid + " elapsed = " + elapsed, LogLevel);
            }

            OnUserJoinedEvent?.Invoke(uid, elapsed);
        }

        private void CallOnUserOfflineEvent(uint uid, USER_OFFLINE_REASON reason)
        {
            M31Logger.LogInfo("onUserOffline: uid = " + uid + " reason = " + reason, LogLevel);
            
            if (UserVideoDictionary.ContainsKey(uid))
            {
                var surface = UserVideoDictionary[uid];
                surface.SetEnable(false);
                UserVideoDictionary.Remove(uid);
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
            _connection.RtcEngine.OnUserJoined += CallOnUserJoined;
            _connection.RtcEngine.OnUserOffline += CallOnUserOfflineEvent;
        }

        public void UnregisterEvents(){
            _connection.RtcEngine.OnUserJoined -= CallOnUserJoined;
            _connection.RtcEngine.OnUserOffline -= CallOnUserOfflineEvent;
        }   

        private bool CheckErrors() {
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_connection.RtcEngine == null) {
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
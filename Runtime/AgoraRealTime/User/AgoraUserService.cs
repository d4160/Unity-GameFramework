#if AGORA
using System;
using d4160.Core;
using Agora.Rtc;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;
using System.Collections.Generic;
using d4160.Instancers;

namespace d4160.Agora_
{
    public class AgoraUserService
    {
        public static event Action<RtcConnection, uint, int> OnUserJoinedEvent;
        public static event Action<RtcConnection, uint, USER_OFFLINE_REASON_TYPE> OnUserOfflineEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public bool AutoVideoSurface { get; set; } = true;
        public VideoSurfaceType AgoraVideoSurfaceType { get; set; } = VideoSurfaceType.Renderer;
        public uint VideoFps { get; set; } = 30;
        public bool EnableFlipHorizontal { get; set; } = true;
        public bool EnableFlipVertical { get; set; } = false;
        public uint LocalUserUID { get; internal set; }

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance;
        private readonly AgoraChannelService _channel = AgoraChannelService.Instance;

        public static AgoraUserService Instance => _instance ?? (_instance = new AgoraUserService());
        private static AgoraUserService _instance;

        private Dictionary<uint, VideoSurface> _userVideoDict = new Dictionary<uint, VideoSurface>();
        public Dictionary<uint, VideoSurface> UserVideoDictionary => _userVideoDict;

        public ComponentProviderSOBase VideoSurfaceProvider { get; set; }
        public float OtherVideoSurfaceScaleMultiplier { get; set; } // 0.25f

        public void CallOnUserJoined(RtcConnection conn, uint uid, int elapsed)
        {
            if (CheckErrors()) return;

            if (!_userVideoDict.ContainsKey(uid))
                _userVideoDict.Add(uid, null);

            M31Logger.LogInfo($"Agora: OnUserJoined channel: {conn.channelId} uid: ${uid} elapsed: {elapsed}");

            if (AutoVideoSurface)
            {
                VideoSurface videoSurface = VideoSurfaceProvider.InstantiateAs<VideoSurface>();
                if (!ReferenceEquals(videoSurface, null))
                {
                    if (uid == 0)
                    {
                        videoSurface.SetForUser(uid);
                    }
                    else
                    {
                        videoSurface.SetForUser(uid, _channel.ChannelInfo.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
                    }

                    videoSurface.OnTextureSizeModify += (int width, int height) =>
                    {
                        float scale = (float)height / (float)width;
                        videoSurface.transform.localScale = new Vector3(-5, 5 * scale, 1);
                        Debug.Log("OnTextureSizeModify: " + width + "  " + height);
                    };

                    videoSurface.SetEnable(true);
                    // configure videoSurface
                    //videoSurface.transform.localScale = Vector3.one * OtherVideoSurfaceScaleMultiplier;
                    //videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType);
                    //videoSurface.SetGameFps(VideoFps);
                    //videoSurface.EnableFilpTextureApply(EnableFlipHorizontal, EnableFlipVertical);
                    //Vector2 pos = AgoraUIUtils.GetRandomPosition(100);
                    //videoSurface.transform.localPosition = new Vector3(pos.x, pos.y, 0);

                    _userVideoDict[uid] = videoSurface;
                }
                else
                {
                    M31Logger.LogWarning("onUserJoined missing VideoSurface: uid = " + uid + " elapsed = " + elapsed, LogLevel);
                }
            }

            OnUserJoinedEvent?.Invoke(conn, uid, elapsed);
        }

        public void CallOnUserOfflineEvent(RtcConnection conn, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            M31Logger.LogInfo($"Agora: OnUserOffline: uid = {uid} reason = {reason}", LogLevel);
            
            if (_userVideoDict.ContainsKey(uid))
            {
                var videoSurface = _userVideoDict[uid];
                if (videoSurface)
                {
                    videoSurface.SetEnable(false);

                    if (VideoSurfaceProvider != null)
                        VideoSurfaceProvider.Destroy(videoSurface);
                    else
                        GameObject.Destroy(videoSurface.gameObject);
                }
                _userVideoDict.Remove(uid);
            }

            OnUserOfflineEvent?.Invoke(conn, uid, reason);
        }

        private AgoraUserService()
        {
            _instance = this;
        }

        public void RegisterEvents () 
        {
            if (_connection.RtcEngine != null)
            {
                _connection.AgoraEventHandler.UserService = this;
            }
        }

        public void UnregisterEvents()
        {
            if (_connection.RtcEngine != null)
            {
                _connection.AgoraEventHandler.UserService = null;
            }
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
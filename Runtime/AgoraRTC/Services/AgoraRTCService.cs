using Agora.Rtc;
using d4160.Events;
using d4160.Logging;
using System;
using UnityEngine;

namespace d4160.AgoraRtc
{
    public class AgoraRtcService
    {
        public AgoraRtcSettingsSO Settings { get; set; }
        public LoggerSO Logger { get; set; }

        public uint LocalUid { get; internal set; }

        public IntStringEventSO OnError { get; set; }
        public JoinChannelEventSO OnJoinChannelSuccess { get; set; }
        public JoinChannelEventSO OnRejoinChannelSuccess { get; set; }
        public LeaveChannelEventSO OnLeaveChannelSuccess { get; set; }
        public ClientRoleChangedEventSO OnClientRoleChanged { get; set; }
        public UserJoinedEventSO OnUserJoined { get; set; }
        public UserOfflineEventSO OnUserOffline { get; set; }

        internal IRtcEngine _rtcEngine = null;
        public IRtcEngine RtcEngine => _rtcEngine;

        public static AgoraRtcService Instance => _instance ??= new AgoraRtcService();
        private static AgoraRtcService _instance;

        public void InitRtcEngine()
        {
            if (!CheckAppId())
            {
                LogInfo($"Please fill in your appId in (AgoraRtcSettingsSO asset)");
                return;
            }

            _rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            UserEventHandler handler = new(this);
            RtcEngineContext context = new(Settings.AppID, Settings.Context,
                                        Settings.ChannelProfileType,
                                        Settings.AudioScenarioType);
            _rtcEngine.Initialize(context);
            _rtcEngine.InitEventHandler(handler);

            LogInfo($"[InitRtcEngine] Success");
        }

        private bool CheckAppId() => Settings.AppID.Length > 10;

        public void DisposeRtcEngine()
        {
            if (RtcEngine == null) return;
            RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
        }

        public void JoinChannel(string token, string channelName, JoinChannelModules modules = JoinChannelModules.EnableAudio | JoinChannelModules.EnableVideo, CLIENT_ROLE_TYPE clientRoleType = CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER, CHANNEL_PROFILE_TYPE channelProfileType = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, VideoEncoderConfiguration videoEncoderConfig = null, ChannelMediaOptions options = null)
        {
            if ((modules & JoinChannelModules.EnableAudio) != 0)
            {
                RtcEngine.EnableAudio();

                if ((modules & JoinChannelModules.EnableLocalAudio) == 0)
                {
                    RtcEngine.EnableLocalAudio(false);
                }
            }
            else
            {
                RtcEngine.DisableAudio();
            }

            if ((modules & JoinChannelModules.EnableVideo) != 0)
            {
                RtcEngine.SetVideoEncoderConfiguration(videoEncoderConfig);
                RtcEngine.EnableVideo();

                if ((modules & JoinChannelModules.EnableLocalVideo) == 0)
                {
                    RtcEngine.EnableLocalVideo(false);
                }
            }
            else
            {
                RtcEngine.DisableVideo();
            }

            RtcEngine.SetChannelProfile(channelProfileType);
            RtcEngine.SetClientRole(clientRoleType);

            if (options == null)
            {
                RtcEngine.JoinChannel(token, channelName);
            }
            else
            {
                RtcEngine.JoinChannel(token, channelName, 0, options);
            }
        }

        public void LeaveChannel()
        {
            //RtcEngine.InitEventHandler(null);
            RtcEngine?.LeaveChannel();
        }

        public void EnableVideo()
        {
            RtcEngine.EnableVideo();
        }

        public void EnableLocalVideo(bool enabled)
        {
            RtcEngine.EnableLocalVideo(enabled);
        }

        public void MuteLocalVideoStream(bool mute)
        {
            RtcEngine.MuteLocalVideoStream(mute);
        }

        public void EnableAudio()
        {
            RtcEngine.EnableAudio();
        }

        public void EnableLocalAudio(bool enabled)
        {
            RtcEngine.EnableLocalAudio(enabled);
        }

        public void MuteLocalAudioStream(bool mute)
        {
            RtcEngine.MuteLocalAudioStream(mute);
        }

        public void DisableVideo()
        {
            RtcEngine.DisableVideo();
        }

        public void DisableAudio()
        {
            RtcEngine.DisableAudio();
        }

        public void StartPreview()
        {
            RtcEngine.StartPreview();
        }

        public void StopPreview()
        {
            RtcEngine.StopPreview();
        }

        public void UpdateChannelMediaOptions(ChannelMediaOptions options)
        {
            RtcEngine.UpdateChannelMediaOptions(options);
        }

        public void SetVideoEncoderConfiguration(VideoEncoderConfiguration config)
        {
            RtcEngine.SetVideoEncoderConfiguration(config);
        }

        public IVideoDeviceManager GetVideoDeviceManager()
        {
            return RtcEngine.GetVideoDeviceManager();
        }

        public IAudioDeviceManager GetAudioDeviceManager()
        {
            return RtcEngine.GetAudioDeviceManager();
        }

        public ScreenCaptureSourceInfo[] GetScreenCaptureSources(SIZE thumbSize, SIZE iconSize, bool includeScreen)
        {
            return RtcEngine.GetScreenCaptureSources(thumbSize, iconSize, includeScreen);
        }

        public void LogInfo(string message)
        {
            if (Logger) Logger.LogInfo(message);
        }
    }

    [Flags]
    public enum JoinChannelModules
    {
        None = 0,
        EnableAudio = 0x1,
        EnableVideo = 0x2,
        EnableLocalAudio = 0x4,
        EnableLocalVideo = 0x8
    }

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraRtcService _service;

        internal UserEventHandler(AgoraRtcService service)
        {
            _service = service;
        }

        public override void OnError(int err, string msg)
        {
            _service.LogInfo($"OnError(int, string); ErrorCode:{err}; Message:{msg};");
            if (_service.OnError) _service.OnError.Invoke(err, msg);
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            _service.LogInfo($"SDK Version: {_service.RtcEngine.GetVersion(ref build)}");
            _service.LogInfo($"OnJoinChannelSuccess(RtcConnection, int); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; Elapsed:{elapsed};");

            _service.LocalUid = connection.localUid;

            //Debug.Log($"[OnJoinChannelSuccess] IsCallbackNull?: {_service.OnJoinChannelSuccess == null}");
            if (_service.OnJoinChannelSuccess) _service.OnJoinChannelSuccess.Invoke(connection, elapsed);
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            _service.LogInfo($"OnRejoinChannelSuccess(RtcConnection, int); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; Elapsed:{elapsed};");
            if (_service.OnRejoinChannelSuccess) _service.OnRejoinChannelSuccess.Invoke(connection, elapsed);
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            _service.LogInfo($"OnLeaveChannel(RtcConnection, RtcStats); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; Duration:{stats.duration}; UserCount:{stats.userCount};");

            _service.LocalUid = 0;

            if (_service.OnLeaveChannelSuccess) _service.OnLeaveChannelSuccess.Invoke(connection, stats);
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            _service.LogInfo($"[OnClientRoleChanged] ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; OldRole:{oldRole}; NewRole:{newRole}; AudienceLatencyLevel:{newRoleOptions.audienceLatencyLevel};");
            if (_service.OnClientRoleChanged) _service.OnClientRoleChanged.Invoke(connection, oldRole, newRole, newRoleOptions);
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            _service.LogInfo($"OnUserJoined(RtcConnection, uint, int); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; Uid:{uid}; Elapsed:{elapsed};");
            if (_service.OnUserJoined) _service.OnUserJoined.Invoke(connection, uid, elapsed);
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            _service.LogInfo($"OnUserOffline(RtcConnection, uint, USER_OFFLINE_REASON_TYPE); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; Uid:{uid}; USER_OFFLINE_REASON_TYPE:{reason};");
            if (_service.OnUserOffline) _service.OnUserOffline.Invoke(connection, uid, reason);
        }
    }
}
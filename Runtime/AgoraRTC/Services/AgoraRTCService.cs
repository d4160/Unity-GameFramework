using Agora.Rtc;
using d4160.Events;
using d4160.Logging;
using System;

namespace d4160.AgoraRtc
{
    public class AgoraRtcService
    {
        public AgoraRtcSettingsSO Settings { get; set; }
        public LoggerSO Logger { get; set; }

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
        }

        private bool CheckAppId() => Settings.AppID.Length > 10;

        public void DisposeRtcEngine()
        {
            if (RtcEngine == null) return;
            RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
        }

        public void JoinChannel(string token, string channelName, JoinChannelModules modules = JoinChannelModules.Both, CLIENT_ROLE_TYPE clientRoleType = CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER, CHANNEL_PROFILE_TYPE channelProfileType = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, VideoEncoderConfiguration videoEncoderConfig = null)
        {
            if ((modules & JoinChannelModules.EnableAudio) != 0)
            {
                RtcEngine.EnableAudio();
            }

            if ((modules & JoinChannelModules.EnableVideo) != 0)
            {
                RtcEngine.SetVideoEncoderConfiguration(videoEncoderConfig);
                RtcEngine.EnableVideo();
            }

            RtcEngine.SetChannelProfile(channelProfileType);
            RtcEngine.SetClientRole(clientRoleType);
            RtcEngine.JoinChannel(token, channelName);
        }

        public void LeaveChannel()
        {
            //RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
        }

        public void EnableVideo()
        {
            RtcEngine.EnableVideo();
        }

        public void EnableAudio()
        {
            RtcEngine.EnableAudio();
        }

        public void DisableVideo()
        {
            RtcEngine.DisableVideo();
        }

        public void DisableAudio()
        {
            RtcEngine.DisableAudio();
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
        Both = 0x3
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
            if(_service.OnError) _service.OnError.Invoke(err, msg);
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            _service.LogInfo($"SDK Version: {_service.RtcEngine.GetVersion(ref build)}");
            _service.LogInfo($"OnJoinChannelSuccess(RtcConnection, int); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; Elapsed:{elapsed};");

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
            if (_service.OnLeaveChannelSuccess) _service.OnLeaveChannelSuccess.Invoke(connection, stats);
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
        {
            _service.LogInfo($"OnClientRoleChanged(RtcConnection, CLIENT_ROLE_TYPE, CLIENT_ROLE_TYPE); ChannelId:{connection.channelId}; LocalUid:{connection.localUid}; OldRole:{oldRole}; NewRole:{newRole};");
            if (_service.OnClientRoleChanged) _service.OnClientRoleChanged.Invoke(connection, oldRole, newRole);
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
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

        /// <summary>
        /// BugFix#70: True when the local user is actively speaking (detected by Agora VAD).
        /// Updated by OnAudioVolumeIndication callback every 200ms.
        /// </summary>
        public bool IsLocalSpeaking { get; internal set; }

        public void InitRtcEngine()
        {
            if (!CheckAppId())
            {
                LogInfo($"Please fill in your appId in (AgoraRtcSettingsSO asset)");
                return;
            }

            _rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            UserEventHandler handler = new(this);

            _rtcEngine.Initialize(Settings.GetRtcEngineContext());
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
                int ret = RtcEngine.EnableAudio();
                Debug.Log($"[BugFix#67b] EnableAudio() returned {ret}");

                if ((modules & JoinChannelModules.EnableLocalAudio) == 0)
                {
                    int ret2 = RtcEngine.EnableLocalAudio(false);
                    Debug.Log($"[BugFix#67b] EnableLocalAudio(false) returned {ret2}");
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

            // BugFix#67b: Always use ChannelMediaOptions overload with explicit autoSubscribeAudio/Video.
            // The legacy JoinChannel(token, channelName, "", 0) API does not guarantee autoSubscribeAudio=true
            // in Agora 4.x, causing remote audio to not be received even though video works
            // (video uses explicit VideoSurface.SetForUser subscription, audio relies on autoSubscribe).
            if (options == null)
            {
                options = new ChannelMediaOptions();
            }
            // Ensure audio/video subscription is always enabled
            if (!options.autoSubscribeAudio.HasValue())
                options.autoSubscribeAudio.SetValue(true);
            if (!options.autoSubscribeVideo.HasValue())
                options.autoSubscribeVideo.SetValue(true);
            // Set publish flags based on module configuration
            if (!options.publishMicrophoneTrack.HasValue())
                options.publishMicrophoneTrack.SetValue((modules & JoinChannelModules.EnableLocalAudio) != 0);
            if (!options.publishCameraTrack.HasValue())
                options.publishCameraTrack.SetValue((modules & JoinChannelModules.EnableLocalVideo) != 0);

            Debug.Log($"[BugFix#67b] JoinChannel: autoSubAudio={options.autoSubscribeAudio.GetValue()}, autoSubVideo={options.autoSubscribeVideo.GetValue()}, pubMic={options.publishMicrophoneTrack.GetValue()}, pubCam={options.publishCameraTrack.GetValue()}");
            RtcEngine.JoinChannel(token, channelName, 0, options);

            // BugFix#70: Enable audio volume indication for speaking detection.
            // 200ms interval, smooth factor 3, VAD (Voice Activity Detection) enabled.
            RtcEngine.EnableAudioVolumeIndication(200, 3, true);
        }

        public void LeaveChannel()
        {
            //RtcEngine.InitEventHandler(null);
            IsLocalSpeaking = false;
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

        // BugFix#70: Detect local user speaking state via Agora VAD
        public override void OnAudioVolumeIndication(RtcConnection connection, AudioVolumeInfo[] speakers, uint speakerNumber, int totalVolume)
        {
            bool localSpeaking = false;
            for (int i = 0; i < speakerNumber; i++)
            {
                if (speakers[i].uid == 0) // uid 0 = local user
                {
                    localSpeaking = speakers[i].vad == 1;
                    break;
                }
            }
            _service.IsLocalSpeaking = localSpeaking;
        }

        // BugFix#67b: Diagnostic callbacks for audio state tracking
        public override void OnRemoteAudioStateChanged(RtcConnection connection, uint remoteUid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
        {
            Debug.Log($"[BugFix#67b] OnRemoteAudioStateChanged: Uid={remoteUid}, State={state}, Reason={reason}, Elapsed={elapsed}");
        }

        public override void OnLocalAudioStateChanged(RtcConnection connection, LOCAL_AUDIO_STREAM_STATE state, LOCAL_AUDIO_STREAM_REASON reason)
        {
            Debug.Log($"[BugFix#67b] OnLocalAudioStateChanged: State={state}, Reason={reason}");
        }

        public override void OnAudioPublishStateChanged(string channel, STREAM_PUBLISH_STATE oldState, STREAM_PUBLISH_STATE newState, int elapseSinceLastState)
        {
            Debug.Log($"[BugFix#67b] OnAudioPublishStateChanged: Channel={channel}, OldState={oldState}, NewState={newState}, Elapsed={elapseSinceLastState}");
        }

        public override void OnAudioSubscribeStateChanged(string channel, uint uid, STREAM_SUBSCRIBE_STATE oldState, STREAM_SUBSCRIBE_STATE newState, int elapseSinceLastState)
        {
            Debug.Log($"[BugFix#67b] OnAudioSubscribeStateChanged: Channel={channel}, Uid={uid}, OldState={oldState}, NewState={newState}, Elapsed={elapseSinceLastState}");
        }
    }
}
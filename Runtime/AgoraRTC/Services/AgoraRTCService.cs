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

        /// <summary>
        /// When true, Agora's audio device (mic/speaker) is kept disabled at all times.
        /// Set this BEFORE calling InitRtcEngine() so that the native AudioRecord is never
        /// claimed, leaving the hardware free for Dissonance VoIP.
        /// </summary>
        public bool DisableAudioDevice { get; set; }

        public void InitRtcEngine()
        {
            Debug.Log($"[AgoraRtcService] InitRtcEngine: Settings={Settings != null}, AppID={(Settings != null ? Settings.AppID : "null")}, DisableAudioDevice={DisableAudioDevice}");

            if (!CheckAppId())
            {
                Debug.LogError($"[AgoraRtcService] InitRtcEngine FAILED — CheckAppId returned false. Settings is {(Settings == null ? "NULL" : "assigned")}. Agora video/screen share will not work. Assign AgoraRtcSettingsSO in the AgoraRtcServiceSO asset.");
                return;
            }

            _rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            UserEventHandler handler = new(this);

            _rtcEngine.Initialize(Settings.GetRtcEngineContext());
            _rtcEngine.InitEventHandler(handler);

            // BugFix#80: When Dissonance handles voice, prevent Agora from ever claiming
            // the native audio device. On Android, Agora's Initialize() alone doesn't grab
            // the mic, but EnableAudio()/JoinChannel with audio DOES — and even after
            // DisableAudio() the native AudioRecord handle isn't fully released, causing
            // Dissonance's Microphone.Start() to fail with FMOD error 80.
            // By disabling audio immediately, we ensure no subsequent call can activate it.
            if (DisableAudioDevice)
            {
                _rtcEngine.DisableAudio();
                _rtcEngine.EnableLocalAudio(false);
                _rtcEngine.MuteLocalAudioStream(true);
                _rtcEngine.MuteAllRemoteAudioStreams(true);
                Debug.Log("[BugFix#80] Agora audio device disabled at init — Dissonance will handle voice.");
            }

            LogInfo($"[InitRtcEngine] Success");
        }

        private bool CheckAppId() => Settings != null && Settings.AppID.Length > 10;

        public void DisposeRtcEngine()
        {
            if (RtcEngine == null) return;
            RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
        }

        public void JoinChannel(string token, string channelName, JoinChannelModules modules = JoinChannelModules.EnableAudio | JoinChannelModules.EnableVideo, CLIENT_ROLE_TYPE clientRoleType = CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER, CHANNEL_PROFILE_TYPE channelProfileType = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, VideoEncoderConfiguration videoEncoderConfig = null, ChannelMediaOptions options = null)
        {
            if (_rtcEngine == null)
            {
                Debug.LogError("[AgoraRtcService] JoinChannel failed — RtcEngine is null. Was InitRtcEngine() called with a valid Settings asset?");
                return;
            }

            // BugFix#80: When audio device is disabled (Dissonance handles voice),
            // never call EnableAudio() — it would reclaim the native AudioRecord on Android.
            if (DisableAudioDevice)
            {
                Debug.Log("[BugFix#80] JoinChannel: audio device disabled, skipping all audio init.");
            }
            else if ((modules & JoinChannelModules.EnableAudio) != 0)
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
            // BugFix#80: Force audio off in channel options when audio device is disabled
            if (DisableAudioDevice)
            {
                options.autoSubscribeAudio.SetValue(false);
                options.publishMicrophoneTrack.SetValue(false);
            }
            else
            {
                // Ensure audio subscription is always enabled
                if (!options.autoSubscribeAudio.HasValue())
                    options.autoSubscribeAudio.SetValue(true);
                if (!options.publishMicrophoneTrack.HasValue())
                    options.publishMicrophoneTrack.SetValue((modules & JoinChannelModules.EnableLocalAudio) != 0);
            }
            // Video subscription is always needed (Agora still handles video)
            if (!options.autoSubscribeVideo.HasValue())
                options.autoSubscribeVideo.SetValue(true);
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
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] EnableVideo skipped — RtcEngine is null."); return; }
            RtcEngine.EnableVideo();
        }

        public void EnableLocalVideo(bool enabled)
        {
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] EnableLocalVideo skipped — RtcEngine is null."); return; }
            RtcEngine.EnableLocalVideo(enabled);
        }

        public void MuteLocalVideoStream(bool mute)
        {
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] MuteLocalVideoStream skipped — RtcEngine is null."); return; }
            RtcEngine.MuteLocalVideoStream(mute);
        }

        public void EnableAudio()
        {
            if (DisableAudioDevice) { Debug.Log("[BugFix#80] EnableAudio blocked — audio device disabled."); return; }
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] EnableAudio skipped — RtcEngine is null."); return; }
            RtcEngine.EnableAudio();
        }

        public void EnableLocalAudio(bool enabled)
        {
            if (DisableAudioDevice) { Debug.Log("[BugFix#80] EnableLocalAudio blocked — audio device disabled."); return; }
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] EnableLocalAudio skipped — RtcEngine is null."); return; }
            RtcEngine.EnableLocalAudio(enabled);
        }

        public void MuteLocalAudioStream(bool mute)
        {
            if (DisableAudioDevice && !mute) { Debug.Log("[BugFix#80] MuteLocalAudioStream(false) blocked — audio device disabled."); return; }
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] MuteLocalAudioStream skipped — RtcEngine is null."); return; }
            RtcEngine.MuteLocalAudioStream(mute);
        }

        public void DisableVideo()
        {
            if (_rtcEngine == null) return;
            RtcEngine.DisableVideo();
        }

        public void DisableAudio()
        {
            if (_rtcEngine == null) return;
            RtcEngine.DisableAudio();
            if (DisableAudioDevice)
            {
                RtcEngine.EnableLocalAudio(false);
                RtcEngine.MuteLocalAudioStream(true);
                RtcEngine.MuteAllRemoteAudioStreams(true);
            }
        }

        public void StartPreview()
        {
            if (_rtcEngine == null) return;
            RtcEngine.StartPreview();
        }

        public void StopPreview()
        {
            if (_rtcEngine == null) return;
            RtcEngine.StopPreview();
        }

        public void UpdateChannelMediaOptions(ChannelMediaOptions options)
        {
            if (_rtcEngine == null) { Debug.LogWarning("[AgoraRtcService] UpdateChannelMediaOptions skipped — RtcEngine is null."); return; }
            RtcEngine.UpdateChannelMediaOptions(options);
        }

        public void SetVideoEncoderConfiguration(VideoEncoderConfiguration config)
        {
            if (_rtcEngine == null) return;
            RtcEngine.SetVideoEncoderConfiguration(config);
        }

        public IVideoDeviceManager GetVideoDeviceManager()
        {
            if (_rtcEngine == null) return null;
            return RtcEngine.GetVideoDeviceManager();
        }

        public IAudioDeviceManager GetAudioDeviceManager()
        {
            if (_rtcEngine == null) return null;
            return RtcEngine.GetAudioDeviceManager();
        }

        public ScreenCaptureSourceInfo[] GetScreenCaptureSources(SIZE thumbSize, SIZE iconSize, bool includeScreen)
        {
            if (_rtcEngine == null) return null;
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
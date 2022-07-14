#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;

namespace d4160.Agora
{
    public enum AgoraChannelType {
        None = 0,
        Audio = 1 << 0,
        Video = 1 << 1
    }

    public class AgoraChannelService
    {
        public static event Action<string, uint, int> OnJoinChannelSuccessEvent;
        public static event Action<string, uint, int> OnReJoinChannelSuccessEvent;
        public static event Action<RtcStats> OnLeaveChannelEvent;
        public static event Action<CHANNEL_MEDIA_RELAY_EVENT> OnChannelMediaRelayEventEvent;
        public static event Action<CHANNEL_MEDIA_RELAY_STATE, CHANNEL_MEDIA_RELAY_ERROR> OnChannelMediaRelayStateChangedEvent;

        private static event Action<RtcStats> OnLeaveChannelEventTemp;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance;
        public static AgoraChannelService Instance => _instance ?? (_instance = new AgoraChannelService());
        private static AgoraChannelService _instance;
        private static AgoraUserService _userService = AgoraUserService.Instance;

        public string CurrentChannel { get; private set; }
        public bool SetExternalVideoSource { get; set; }
        public bool InChannel { get;  private set; }

        // TODO ChannelProfile

        private void CallOnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            InChannel = true;
            CurrentChannel = channelName;
            _userService.LocalUserUID = uid;
            
            M31Logger.LogInfo("AGORA: JoinChannelSuccessHandler: uid = " + uid, LogLevel);
            OnJoinChannelSuccessEvent?.Invoke(channelName, uid, elapsed);
        }

        private void CallOnReJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            M31Logger.LogInfo("AGORA: ReJoinChannelSuccessHandler: uid = " + uid, LogLevel);
            OnReJoinChannelSuccessEvent?.Invoke(channelName, uid, elapsed);
        }

        private void CallOnLeaveChannel(RtcStats stats)
        {
            InChannel = false;
            InvokeOnLeaveChannelEventTemp(stats);

            M31Logger.LogInfo("AGORA: LeaveChannel: stats = " + stats, LogLevel);
            OnLeaveChannelEvent?.Invoke(stats);
        }

        private void CallOnChannelMediaRelayEvent(CHANNEL_MEDIA_RELAY_EVENT events)
        {
            M31Logger.LogInfo("AGORA: ChannelMediaRelay: events = " + events, LogLevel);
            OnChannelMediaRelayEventEvent?.Invoke(events);
        }

        private void CallOnChannelMediaRelayStateChanged(CHANNEL_MEDIA_RELAY_STATE state, CHANNEL_MEDIA_RELAY_ERROR code)
        {
            M31Logger.LogInfo("AGORA: ChannelMediaRelayStateChanged: state = " + state, LogLevel);
            OnChannelMediaRelayStateChangedEvent?.Invoke(state, code);
        }

        private AgoraChannelService()
        {
            _instance = this;
        }

        public void RegisterEvents () {

            var rtcEngine = _connection.RtcEngine;

            if (rtcEngine != null)
            {
                rtcEngine.OnJoinChannelSuccess += CallOnJoinChannelSuccess;
                rtcEngine.OnReJoinChannelSuccess += CallOnReJoinChannelSuccess;
                rtcEngine.OnLeaveChannel += CallOnLeaveChannel;
                rtcEngine.OnChannelMediaRelayEvent += CallOnChannelMediaRelayEvent;
                rtcEngine.OnChannelMediaRelayStateChanged += CallOnChannelMediaRelayStateChanged;
            }
            else {
                M31Logger.LogError("AGORA: RegisterEvents failed. Initialize the RtcEngine first. You can do this calling 'LoadEngine' of AgoraConnectionService or using AgoraConnectionBehaviour or AgoraConnectionSO.", LogLevel);
            }
        }

        public void UnregisterEvents(){
            var rtcEngine = _connection.RtcEngine;

            if (rtcEngine != null)
            {
                rtcEngine.OnJoinChannelSuccess -= CallOnJoinChannelSuccess;
                rtcEngine.OnReJoinChannelSuccess -= CallOnReJoinChannelSuccess;
                rtcEngine.OnLeaveChannel -= CallOnLeaveChannel;
                rtcEngine.OnChannelMediaRelayEvent -= CallOnChannelMediaRelayEvent;
                rtcEngine.OnChannelMediaRelayStateChanged -= CallOnChannelMediaRelayStateChanged;
            }
            else {
                M31Logger.LogError("AGORA: UnregisterEvents failed. Initialize the RtcEngine first.You can do this calling 'LoadEngine' of AgoraConnectionService or using AgoraConnectionBehaviour or AgoraConnectionSO.", LogLevel);
            }
        }   

        public void Setup(AgoraChannelType channelType = AgoraChannelType.Video | AgoraChannelType.Audio){
            if (CheckErrors()) return;

            if(((int)channelType & (int)AgoraChannelType.Video) != 0) {
                // enable video
                _connection.RtcEngine.EnableVideo();
                // allow camera output callback
                _connection.RtcEngine.EnableVideoObserver();
            }
            else {
                _connection.RtcEngine.DisableVideo();
            }

            if(((int)channelType & (int)AgoraChannelType.Audio) != 0) {
                _connection.RtcEngine.EnableAudio();
            }
            else {
                _connection.RtcEngine.DisableAudio();
            }
        }

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableAudio(bool enableAudio)
        {
            if (CheckErrors()) return;

            if (enableAudio)
            {
                _connection.RtcEngine.EnableAudio();
            }
            else
            {
                _connection.RtcEngine.DisableAudio();
            }
        }

        /// <summary>
        ///   Join a RTC channel
        /// </summary>
        /// <param name="channelName"></param>
        public void JoinChannel(string channelName, string info = null, uint uid = 0)
        {
            M31Logger.LogInfo("AGORA: calling join (channel = " + channelName + ")", LogLevel);

            if (CheckErrors()) return;

            CurrentChannel = channelName;

            int externalVideo = 0;
            if (SetExternalVideoSource)
                externalVideo = _connection.RtcEngine.SetExternalVideoSource(true, false);

            // join channel
            _connection.RtcEngine.JoinChannel(channelName, info, uid);

            M31Logger.LogInfo($"AGORA: Joining to '{channelName}' channel. ExternalVideo response: {externalVideo}", LogLevel);
        }

        /// <summary>
        ///   Leave a RTC channel
        /// </summary>
        public void LeaveChannel(Action<RtcStats> onLeaveChannel = null)
        {
            M31Logger.LogInfo("calling leave", LogLevel);

            if (CheckErrors()) return;

            if (InChannel)
            {
                // leave channel
                _connection.RtcEngine.LeaveChannel();
                // deregister video frame observers in native-c code
                _connection.RtcEngine.DisableVideoObserver();
            }
            else {
                InvokeOnLeaveChannelEventTemp(default);
            }
        }

        private bool CheckErrors(){
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_connection.RtcEngine == null) {
                M31Logger.LogWarning("AGORA: RtcEngine is null", LogLevel);
                return true;
            }

            return false;
        }

        private void InvokeOnLeaveChannelEventTemp(RtcStats stats) {
            OnLeaveChannelEventTemp?.Invoke(stats);
            OnLeaveChannelEventTemp = null;
        }
    }

    [Serializable]
    public struct ChannelStruct {
        public string channelName;
        public string info; // ""
        public uint uid; // 0
    }
}
#endif
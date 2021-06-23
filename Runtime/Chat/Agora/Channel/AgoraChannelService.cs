#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using M31Logger = d4160.Logging.M31Logger;

namespace d4160.Chat.Agora
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

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraAuthService _authService = AgoraAuthService.Instance; 
        public static AgoraChannelService Instance => _instance ?? (_instance = new AgoraChannelService());
        private static AgoraChannelService _instance;

        public string CurrentChannel { get; private set; }

        private void CallOnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            M31Logger.LogInfo("JoinChannelSuccessHandler: uid = " + uid, LogLevel);
            OnJoinChannelSuccessEvent?.Invoke(channelName, uid, elapsed);
        }

        private void CallOnReJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            M31Logger.LogInfo("ReJoinChannelSuccessHandler: uid = " + uid, LogLevel);
            OnReJoinChannelSuccessEvent?.Invoke(channelName, uid, elapsed);
        }

        private void CallOnLeaveChannel(RtcStats stats)
        {
            M31Logger.LogInfo("LeaveChannel: stats = " + stats, LogLevel);
            OnLeaveChannelEvent?.Invoke(stats);
        }

        private void CallOnChannelMediaRelayEvent(CHANNEL_MEDIA_RELAY_EVENT events)
        {
            M31Logger.LogInfo("ChannelMediaRelay: events = " + events, LogLevel);
            OnChannelMediaRelayEventEvent?.Invoke(events);
        }

        private void CallOnChannelMediaRelayStateChanged(CHANNEL_MEDIA_RELAY_STATE state, CHANNEL_MEDIA_RELAY_ERROR code)
        {
            M31Logger.LogInfo("ChannelMediaRelayStateChanged: state = " + state, LogLevel);
            OnChannelMediaRelayStateChangedEvent?.Invoke(state, code);
        }

        private AgoraChannelService()
        {
            _instance = this;
        }

        public void RegisterEvents () {
            _authService.RtcEngine.OnJoinChannelSuccess += CallOnJoinChannelSuccess;
            _authService.RtcEngine.OnReJoinChannelSuccess += CallOnReJoinChannelSuccess;
            _authService.RtcEngine.OnLeaveChannel += CallOnLeaveChannel;
            _authService.RtcEngine.OnChannelMediaRelayEvent += CallOnChannelMediaRelayEvent;
            _authService.RtcEngine.OnChannelMediaRelayStateChanged += CallOnChannelMediaRelayStateChanged;
        }

        public void UnregisterEvents(){
            _authService.RtcEngine.OnJoinChannelSuccess -= CallOnJoinChannelSuccess;
            _authService.RtcEngine.OnReJoinChannelSuccess -= CallOnReJoinChannelSuccess;
            _authService.RtcEngine.OnLeaveChannel -= CallOnLeaveChannel;
            _authService.RtcEngine.OnChannelMediaRelayEvent -= CallOnChannelMediaRelayEvent;
            _authService.RtcEngine.OnChannelMediaRelayStateChanged -= CallOnChannelMediaRelayStateChanged;
        }   

        public void Setup(AgoraChannelType channelType = AgoraChannelType.Video | AgoraChannelType.Audio){
            if (CheckErrors()) return;

            if(((int)channelType & (int)AgoraChannelType.Video) != 0) {
                // enable video
                _authService.RtcEngine.EnableVideo();
                // allow camera output callback
                _authService.RtcEngine.EnableVideoObserver();
            }
            else {
                _authService.RtcEngine.DisableVideo();
            }

            if(((int)channelType & (int)AgoraChannelType.Audio) != 0) {
                _authService.RtcEngine.EnableAudio();
            }
            else {
                _authService.RtcEngine.DisableAudio();
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
                _authService.RtcEngine.EnableAudio();
            }
            else
            {
                _authService.RtcEngine.DisableAudio();
            }
        }

        /// <summary>
        ///   Join a RTC channel
        /// </summary>
        /// <param name="channelName"></param>
        public void JoinChannel(string channelName, string info = null, uint uid = 0)
        {
            M31Logger.LogInfo("calling join (channel = " + channelName + ")", LogLevel);

            if (CheckErrors()) return;

            CurrentChannel = channelName;

            //_authService.RtcEngine.OnUserJoined = OnUserJoined;
            //_authService.RtcEngine.OnUserOffline = OnUserOffline;
            //_authService.RtcEngine.OnVideoSizeChanged = OnVideoSizeChanged;
            // Calling virtual setup function

            // join channel
            _authService.RtcEngine.JoinChannel(channelName, info, uid);

            M31Logger.LogInfo("initializeEngine done", LogLevel);
        }

        /// <summary>
        ///   Leave a RTC channel
        /// </summary>
        public void LeaveChannel()
        {
            M31Logger.LogInfo("calling leave", LogLevel);

            if (CheckErrors()) return;

            // leave channel
            _authService.RtcEngine.LeaveChannel();
            // deregister video frame observers in native-c code
            _authService.RtcEngine.DisableVideoObserver();
        }

        private bool CheckErrors(){
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_authService.RtcEngine == null) {
                M31Logger.LogWarning("AGORA: RtcEngine is null", LogLevel);
                return true;
            }

            return false;
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
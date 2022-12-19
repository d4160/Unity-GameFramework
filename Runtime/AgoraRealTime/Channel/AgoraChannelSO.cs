#if AGORA
using System;
using d4160.Core;
using Agora.Rtc;
using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Agora_
{
    [CreateAssetMenu(menuName = "d4160/Chat/Agora Channel")]
    public class AgoraChannelSO : ScriptableObject
    {
        [SerializeField] private AgoraChannelType _setupChannelType;
        [SerializeField] private bool _enableAudio;
        [SerializeField] private ChannelStruct _joinChannelParams;
        [SerializeField] private bool _setExternalVideoSource;

        public event Action<RtcConnection, int> OnJoinChannelSuccessEvent;
        public event Action<RtcConnection, int> OnReJoinChannelSuccessEvent;
        public event Action<RtcConnection, RtcStats> OnLeaveChannelEvent;
        //public event Action<CHANNEL_MEDIA_RELAY_EVENT> OnChannelMediaRelayEventEvent;
        //public event Action<CHANNEL_MEDIA_RELAY_STATE, CHANNEL_MEDIA_RELAY_ERROR> OnChannelMediaRelayStateChangedEvent;

        private readonly AgoraChannelService _channelService = AgoraChannelService.Instance;

        public RtcConnection ChannelInfo => _channelService.ChannelInfo;

        private void CallOnJoinChannelSuccess(RtcConnection conn, int elapsed) => OnJoinChannelSuccessEvent?.Invoke(conn, elapsed);
        private void CallOnReJoinChannelSuccess(RtcConnection conn, int elapsed) => OnReJoinChannelSuccessEvent?.Invoke(conn, elapsed);
        private void CallOnLeaveChannel(RtcConnection conn, RtcStats stats) => OnLeaveChannelEvent?.Invoke(conn, stats);
        //private void CallOnChannelMediaRelayEvent(CHANNEL_MEDIA_RELAY_EVENT events) => OnChannelMediaRelayEventEvent?.Invoke(events);
        //private void CallOnChannelMediaRelayStateChanged(CHANNEL_MEDIA_RELAY_STATE state, CHANNEL_MEDIA_RELAY_ERROR code) => OnChannelMediaRelayStateChangedEvent?.Invoke(state, code);

        public string ChannelName { get => _joinChannelParams.channelName; set => _joinChannelParams.channelName = value; }

        public void RegisterEvents () {
            _channelService.RegisterEvents();

            AgoraChannelService.OnJoinChannelSuccessEvent += CallOnJoinChannelSuccess;
            AgoraChannelService.OnReJoinChannelSuccessEvent += CallOnReJoinChannelSuccess;
            AgoraChannelService.OnLeaveChannelEvent += CallOnLeaveChannel;
            //AgoraChannelService.OnChannelMediaRelayEventEvent += CallOnChannelMediaRelayEvent;
            //AgoraChannelService.OnChannelMediaRelayStateChangedEvent += CallOnChannelMediaRelayStateChanged;
        }

        public void UnregisterEvents(){
            _channelService.UnregisterEvents();

            AgoraChannelService.OnJoinChannelSuccessEvent -= CallOnJoinChannelSuccess;
            AgoraChannelService.OnReJoinChannelSuccessEvent -= CallOnReJoinChannelSuccess;
            AgoraChannelService.OnLeaveChannelEvent -= CallOnLeaveChannel;
            //AgoraChannelService.OnChannelMediaRelayEventEvent -= CallOnChannelMediaRelayEvent;
            //AgoraChannelService.OnChannelMediaRelayStateChangedEvent -= CallOnChannelMediaRelayStateChanged;
        }   

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Setup() {
            Setup(_setupChannelType);
        }

        public void Setup(AgoraChannelType channelType = AgoraChannelType.Video | AgoraChannelType.Audio){
            _channelService.Setup(channelType);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetEnableAudio() {
            SetEnableAudio(_enableAudio);
        }

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableAudio(bool enableAudio)
        {
            _channelService.SetEnableAudio(enableAudio);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinChannel() {
            JoinChannel(_joinChannelParams.token, _joinChannelParams.channelName, _joinChannelParams.info, _joinChannelParams.uid);
        }

        /// <summary>
        ///   Join a RTC channel
        /// </summary>
        /// <param name="channel"></param>
        public void JoinChannel(string token, string channel, string info = null, uint uid = 0)
        {
            _channelService.SetExternalVideoSource = _setExternalVideoSource;
            _channelService.JoinChannel(token, channel, info, uid);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        /// <summary>
        ///   Leave a RTC channel
        /// </summary>
        public void LeaveChannel(Action<RtcStats> onLeaveChannel = null)
        {
            _channelService.LeaveChannel(onLeaveChannel);
        }
    }
}
#endif
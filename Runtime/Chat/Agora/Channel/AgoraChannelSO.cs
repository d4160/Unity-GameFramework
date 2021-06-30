#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using NaughtyAttributes;

namespace d4160.Chat.Agora
{
    [CreateAssetMenu(menuName = "d4160/Chat/Agora Channel")]
    public class AgoraChannelSO : ScriptableObject
    {
        [SerializeField] private AgoraChannelType _setupChannelType;
        [SerializeField] private bool _enableAudio;
        [SerializeField] private ChannelStruct _joinChannelParams;
        [SerializeField] private bool _setExternalVideoSource;

        public event Action<string, uint, int> OnJoinChannelSuccessEvent;
        public event Action<string, uint, int> OnReJoinChannelSuccessEvent;
        public event Action<RtcStats> OnLeaveChannelEvent;
        public event Action<CHANNEL_MEDIA_RELAY_EVENT> OnChannelMediaRelayEventEvent;
        public event Action<CHANNEL_MEDIA_RELAY_STATE, CHANNEL_MEDIA_RELAY_ERROR> OnChannelMediaRelayStateChangedEvent;

        private readonly AgoraChannelService _channelService = AgoraChannelService.Instance;

        public string CurrentChannel => _channelService.CurrentChannel;

        private void CallOnJoinChannelSuccess(string channelName, uint uid, int elapsed) => OnJoinChannelSuccessEvent?.Invoke(channelName, uid, elapsed);
        private void CallOnReJoinChannelSuccess(string channelName, uint uid, int elapsed) => OnReJoinChannelSuccessEvent?.Invoke(channelName, uid, elapsed);
        private void CallOnLeaveChannel(RtcStats stats) => OnLeaveChannelEvent?.Invoke(stats);
        private void CallOnChannelMediaRelayEvent(CHANNEL_MEDIA_RELAY_EVENT events) => OnChannelMediaRelayEventEvent?.Invoke(events);
        private void CallOnChannelMediaRelayStateChanged(CHANNEL_MEDIA_RELAY_STATE state, CHANNEL_MEDIA_RELAY_ERROR code) => OnChannelMediaRelayStateChangedEvent?.Invoke(state, code);

        public string ChannelName { get => _joinChannelParams.channelName; set => _joinChannelParams.channelName = value; }

        public void RegisterEvents () {
            _channelService.RegisterEvents();

            AgoraChannelService.OnJoinChannelSuccessEvent += CallOnJoinChannelSuccess;
            AgoraChannelService.OnReJoinChannelSuccessEvent += CallOnReJoinChannelSuccess;
            AgoraChannelService.OnLeaveChannelEvent += CallOnLeaveChannel;
            AgoraChannelService.OnChannelMediaRelayEventEvent += CallOnChannelMediaRelayEvent;
            AgoraChannelService.OnChannelMediaRelayStateChangedEvent += CallOnChannelMediaRelayStateChanged;
        }

        public void UnregisterEvents(){
            _channelService.UnregisterEvents();

            AgoraChannelService.OnJoinChannelSuccessEvent -= CallOnJoinChannelSuccess;
            AgoraChannelService.OnReJoinChannelSuccessEvent -= CallOnReJoinChannelSuccess;
            AgoraChannelService.OnLeaveChannelEvent -= CallOnLeaveChannel;
            AgoraChannelService.OnChannelMediaRelayEventEvent -= CallOnChannelMediaRelayEvent;
            AgoraChannelService.OnChannelMediaRelayStateChangedEvent -= CallOnChannelMediaRelayStateChanged;
        }   

        [Button]
        public void Setup() {
            Setup(_setupChannelType);
        }

        public void Setup(AgoraChannelType channelType = AgoraChannelType.Video | AgoraChannelType.Audio){
            _channelService.Setup(channelType);
        }

        [Button]
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

        [Button]
        public void JoinChannel() {
            JoinChannel(_joinChannelParams.channelName, _joinChannelParams.info, _joinChannelParams.uid);
        }

        /// <summary>
        ///   Join a RTC channel
        /// </summary>
        /// <param name="channel"></param>
        public void JoinChannel(string channel, string info = null, uint uid = 0)
        {
            _channelService.SetExternalVideoSource = _setExternalVideoSource;
            _channelService.JoinChannel(channel, info, uid);
        }

        [Button]
        /// <summary>
        ///   Leave a RTC channel
        /// </summary>
        public void LeaveChannel()
        {
            _channelService.LeaveChannel();
        }
    }
}
#endif
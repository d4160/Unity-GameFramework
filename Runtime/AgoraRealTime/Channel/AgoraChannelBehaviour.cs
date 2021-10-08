#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using d4160.MonoBehaviourData;
using UltEvents;

namespace d4160.Chat.Agora
{
    public class AgoraChannelBehaviour : MonoBehaviourUnityData<AgoraChannelSO>
    {
        [SerializeField] private UltEvent<string, uint, int> _onJoinChannelSuccess;
        [SerializeField] private UltEvent<string, uint, int> _onReJoinChannelSuccess;
        [SerializeField] private UltEvent<RtcStats> _onLeaveChannel;
        [SerializeField] private UltEvent<CHANNEL_MEDIA_RELAY_EVENT> _onChannelMediaRelayEvent;
        [SerializeField] private UltEvent<CHANNEL_MEDIA_RELAY_STATE, CHANNEL_MEDIA_RELAY_ERROR> _onChannelMediaRelayStateChanged;

        public string ChannelName { get => _data?.ChannelName; set { if (_data) _data.ChannelName = value; } }
        
        void OnEnable () {
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnJoinChannelSuccessEvent += _onJoinChannelSuccess.Invoke;
                _data.OnReJoinChannelSuccessEvent += _onReJoinChannelSuccess.Invoke;
                _data.OnLeaveChannelEvent += _onLeaveChannel.Invoke;
                _data.OnChannelMediaRelayEventEvent += _onChannelMediaRelayEvent.Invoke;
                _data.OnChannelMediaRelayStateChangedEvent += _onChannelMediaRelayStateChanged.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnJoinChannelSuccessEvent -= _onJoinChannelSuccess.Invoke;
                _data.OnReJoinChannelSuccessEvent -= _onReJoinChannelSuccess.Invoke;
                _data.OnLeaveChannelEvent -= _onLeaveChannel.Invoke;
                _data.OnChannelMediaRelayEventEvent -= _onChannelMediaRelayEvent.Invoke;
                _data.OnChannelMediaRelayStateChangedEvent -= _onChannelMediaRelayStateChanged.Invoke;
            }
        }   

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Setup() {
            if(_data)
                _data.Setup();
        }

        public void Setup(AgoraChannelType channelType = AgoraChannelType.Video | AgoraChannelType.Audio){
            if(_data)
                _data.Setup(channelType);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetEnableAudio() {
            if(_data)
                _data.SetEnableAudio();
        }

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableAudio(bool enableAudio)
        {
            if(_data)
                _data.SetEnableAudio(enableAudio);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinChannel() {
            if(_data)
                _data.JoinChannel();
        }

        /// <summary>
        ///   Join a RTC channel
        /// </summary>
        /// <param name="channel"></param>
        public void JoinChannel(string channel, string info = null, uint uid = 0)
        {
            if(_data)
                _data.JoinChannel(channel, info, uid);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        /// <summary>
        ///   Leave a RTC channel
        /// </summary>
        public void LeaveChannel()
        {
            if(_data)
                _data.LeaveChannel();
        }
    }
}
#endif
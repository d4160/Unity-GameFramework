#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using NaughtyAttributes;
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
                _data.OnJoinChannelSuccessEvent += _onJoinChannelSuccess.Invoke;
                _data.OnReJoinChannelSuccessEvent += _onReJoinChannelSuccess.Invoke;
                _data.OnLeaveChannelEvent += _onLeaveChannel.Invoke;
                _data.OnChannelMediaRelayEventEvent += _onChannelMediaRelayEvent.Invoke;
                _data.OnChannelMediaRelayStateChangedEvent += _onChannelMediaRelayStateChanged.Invoke;
            }
        }

        void UnregisterEvents(){
            if (_data)
            {
                _data.OnJoinChannelSuccessEvent -= _onJoinChannelSuccess.Invoke;
                _data.OnReJoinChannelSuccessEvent -= _onReJoinChannelSuccess.Invoke;
                _data.OnLeaveChannelEvent -= _onLeaveChannel.Invoke;
                _data.OnChannelMediaRelayEventEvent -= _onChannelMediaRelayEvent.Invoke;
                _data.OnChannelMediaRelayStateChangedEvent -= _onChannelMediaRelayStateChanged.Invoke;
            }
        }   

        [Button]
        public void Setup() {
            if(_data)
                _data.Setup();
        }

        public void Setup(AgoraChannelType channelType = AgoraChannelType.Video | AgoraChannelType.Audio){
            if(_data)
                _data.Setup(channelType);
        }

        [Button]
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

        [Button]
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

        [Button]
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
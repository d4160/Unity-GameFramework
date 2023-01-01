#if VIVOX
using UnityEngine;
using VivoxUnity;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using d4160.MonoBehaviours;
using UltEvents;

namespace d4160.Chat.Vivox
{
    public class VivoxChannelBehaviour : MonoBehaviourUnityData<VivoxChannelSO>
    {
        [SerializeField] private UltEvent<ChannelId> _onJoinedToChannel;
        [SerializeField] private UltEvent<IChannelTextMessage> _onMessageLogRecieved;
        [SerializeField] private UltEvent<IParticipant> _onParticipantAdded;
        [SerializeField] private UltEvent<IParticipant> _onParticipantRemoved;
        [SerializeField] private UltEvent<IParticipant, bool> _onSpeechDetected; 
        [SerializeField] private UltEvent<IParticipant, double> _onAudioEnergyChanged;

        public string ChannelName { get => _data?.ChannelName; set { if (_data) _data.ChannelName = value; } }
        public bool Muted { get => _data ? _data.Muted : true; set { if (_data) _data.Muted = value; } }

        void OnEnable() {
            if (_data)
            {
                _data.OnJoinedToChannelEvent += _onJoinedToChannel.Invoke;
                _data.OnMessageLogRecievedEvent += _onMessageLogRecieved.Invoke;
                _data.OnParticipantAddedEvent += _onParticipantAdded.Invoke;
                _data.OnParticipantRemovedEvent += _onParticipantRemoved.Invoke;
                _data.OnSpeechDetectedEvent += _onSpeechDetected.Invoke;
                _data.OnAudioEnergyChangedEvent += _onAudioEnergyChanged.Invoke;
            }
        }

        void OnDisable() {
            if (_data)
            {
                _data.OnJoinedToChannelEvent -= _onJoinedToChannel.Invoke;
                _data.OnMessageLogRecievedEvent -= _onMessageLogRecieved.Invoke;
                _data.OnParticipantAddedEvent -= _onParticipantAdded.Invoke;
                _data.OnParticipantRemovedEvent -= _onParticipantRemoved.Invoke;
                _data.OnSpeechDetectedEvent -= _onSpeechDetected.Invoke;
                _data.OnAudioEnergyChangedEvent -= _onAudioEnergyChanged.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinChannel(){
            if (_data)
                _data.JoinChannel();
        }
                
        public void JoinChannel(string channelName, ChannelType channelType, ChatCapability chatCapability,
        bool switchTransmission = true, Channel3DProperties properties = null)
        {
            if (_data)
                _data.JoinChannel(channelName, channelType, chatCapability, switchTransmission, properties);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SendTextMessage(){
            if (_data)
                _data.SendTextMessage();
        }

        public void SendTextMessage(string messageToSend, ChannelId channel, string applicationStanzaNamespace = null, string applicationStanzaBody = null)
        {
            if (_data)
                _data.SendTextMessage(messageToSend, channel, applicationStanzaNamespace, applicationStanzaBody);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisconnectAllChannels()
        {
            if (_data)
                _data.DisconnectAllChannels();
        }
    }
}
#endif
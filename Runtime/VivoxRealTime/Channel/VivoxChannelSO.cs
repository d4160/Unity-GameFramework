#if VIVOX
using System;
using System.ComponentModel;
using d4160.Core;
using UnityEngine;
using VivoxUnity;
using d4160.Auth.Vivox;
using Logger = d4160.Logging.LoggerM31;
using System.Linq;
using System.Collections.Generic;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Chat.Vivox
{
    [CreateAssetMenu(menuName = "d4160/Chat/Vivox Channel")]
    public class VivoxChannelSO : ScriptableObject
    {
        [SerializeField] private VivoxJoinChannelParamsStruct _joinChannelParams;
        [SerializeField] private SendTextParamsStruct _sendTextParams;

        public event Action<ChannelId> OnJoinedToChannelEvent;
        public event Action<IChannelTextMessage> OnMessageLogRecievedEvent;
        public event Action<IParticipant> OnParticipantAddedEvent;
        public event Action<IParticipant> OnParticipantRemovedEvent;
        public event Action<IParticipant, bool> OnSpeechDetectedEvent; 
        public event Action<IParticipant, double> OnAudioEnergyChangedEvent;

#if UNITY_EDITOR
        private string[] ChannelNames { 
            get {
                string[] names = new string[ChannelIds.Count];
                for (var i = 0; i < names.Length; i++)
                {
                    names[i] = ChannelIds[i].Name;
                }

                return names;
            }
        }
#endif
        public VivoxUnity.IReadOnlyDictionary<ChannelId, IChannelSession> ActiveChannels => _channelService.ActiveChannels;
        // To mute
        public IAudioDevices AudioInputDevices => _channelService.AudioInputDevices;
        public IAudioDevices AudioOutputDevices => _channelService.AudioOutputDevices;
        public bool Muted { get => _channelService.Muted; set => _channelService.Muted = value; }
        public int VolumeAdjustment { get => _channelService.VolumeAdjustment; set => _channelService.VolumeAdjustment = value; }
        public IChannelSession TransmittingSession
        {
            get => _channelService.TransmittingSession;
            set => _channelService.TransmittingSession = value;
        }
        public List<ChannelId> ChannelIds => _channelService.ChannelIds;

        private readonly VivoxChannelService _channelService = VivoxChannelService.Instance;

        private void CallOnJoinedToChannelEvent(ChannelId channel) => OnJoinedToChannelEvent?.Invoke(channel);
        private void CallOnMessageLogRecievedEvent(IChannelTextMessage textMessage) => OnMessageLogRecievedEvent?.Invoke(textMessage);
        private void CallOnParticipantAddedEvent(IParticipant participant) => OnParticipantAddedEvent?.Invoke(participant);
        private void CallOnParticipantRemovedEvent(IParticipant participant) => OnParticipantRemovedEvent?.Invoke(participant);
        private void CallOnSpeechDetectedEvent(IParticipant participant, bool speechValue) => OnSpeechDetectedEvent?.Invoke(participant, speechValue); 
        private void CallOnAudioEnergyChangedEvent(IParticipant participant, double energyValue) => OnAudioEnergyChangedEvent?.Invoke(participant, energyValue);

        public string ChannelName { get => _joinChannelParams.channelName; set => _joinChannelParams.channelName = value; }

        public void RegisterEvents() {
            VivoxChannelService.OnJoinedToChannelEvent += CallOnJoinedToChannelEvent;
            VivoxChannelService.OnMessageLogRecievedEvent += CallOnMessageLogRecievedEvent;
            VivoxChannelService.OnParticipantAddedEvent += CallOnParticipantAddedEvent;
            VivoxChannelService.OnParticipantRemovedEvent += CallOnParticipantRemovedEvent;
            VivoxChannelService.OnSpeechDetectedEvent += CallOnSpeechDetectedEvent;
            VivoxChannelService.OnAudioEnergyChangedEvent += CallOnAudioEnergyChangedEvent;
        }

        public void UnegisterEvents() {
            VivoxChannelService.OnJoinedToChannelEvent -= CallOnJoinedToChannelEvent;
            VivoxChannelService.OnMessageLogRecievedEvent -= CallOnMessageLogRecievedEvent;
            VivoxChannelService.OnParticipantAddedEvent -= CallOnParticipantAddedEvent;
            VivoxChannelService.OnParticipantRemovedEvent -= CallOnParticipantRemovedEvent;
            VivoxChannelService.OnSpeechDetectedEvent -= CallOnSpeechDetectedEvent;
            VivoxChannelService.OnAudioEnergyChangedEvent -= CallOnAudioEnergyChangedEvent;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinChannel(){
            JoinChannel(_joinChannelParams.channelName, _joinChannelParams.channelType, _joinChannelParams.chatCapability, _joinChannelParams.switchTransmission, _joinChannelParams.GetChannel3DProperties());
        }
                
        public void JoinChannel(string channelName, ChannelType channelType, ChatCapability chatCapability,
        bool switchTransmission = true, Channel3DProperties properties = null)
        {
            _channelService.JoinChannel(channelName, channelType, chatCapability, switchTransmission, properties);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SendTextMessage(){
            SendTextMessage(_sendTextParams.messageToSend, _channelService.GetChannelId(_sendTextParams.channel), _sendTextParams.applicationStanzaNamespace, _sendTextParams.applicationStanzaBody);
        }

        public void SendTextMessage(string messageToSend, ChannelId channel, string applicationStanzaNamespace = null, string applicationStanzaBody = null)
        {
            _channelService.SendTextMessage(messageToSend, channel, applicationStanzaNamespace, applicationStanzaBody);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisconnectChannel()
        {
            _channelService.DisconnectChannel(_joinChannelParams.channelName);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisconnectAllChannels()
        {
            _channelService.DisconnectAllChannels();
        }
    }
}
#endif
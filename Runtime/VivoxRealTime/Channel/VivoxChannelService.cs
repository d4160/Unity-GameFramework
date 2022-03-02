#if VIVOX
using System;
using System.ComponentModel;
using d4160.Core;
using UnityEngine;
using VivoxUnity;
using d4160.Auth.Vivox;
using M31Logger = d4160.Logging.LoggerM31;
using System.Linq;
using System.Collections.Generic;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using d4160.Collections;

namespace d4160.Chat.Vivox
{
    public class VivoxChannelService
    {
        public static event Action<ChannelId> OnJoinedToChannelEvent;
        public static event Action<IChannelTextMessage> OnMessageLogRecievedEvent;
        public static event Action<IParticipant> OnParticipantAddedEvent;
        public static event Action<IParticipant> OnParticipantRemovedEvent;
        public static event Action<IParticipant, bool> OnSpeechDetectedEvent; 
        public static event Action<IParticipant, double> OnAudioEnergyChangedEvent; 

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public VivoxUnity.IReadOnlyDictionary<ChannelId, IChannelSession> ActiveChannels => _authService.LoginSession?.ChannelSessions;
        // To mute
        public IAudioDevices AudioInputDevices => VivoxAuthService.Client.AudioInputDevices;
        public IAudioDevices AudioOutputDevices => VivoxAuthService.Client.AudioOutputDevices;
        public bool Muted { get => AudioInputDevices.Muted; set => AudioInputDevices.Muted = value; }
        public int VolumeAdjustment { get => AudioInputDevices.VolumeAdjustment; set => AudioInputDevices.VolumeAdjustment = value; }

        public IChannelSession TransmittingSession
        {
            get
            {
                if (VivoxAuthService.Client == null)
                    throw new NullReferenceException("client");

                return _authService.LoginSession.ChannelSessions.FirstOrDefault(x => x.IsTransmitting);
            }
            set
            {
                if (value != null)
                {
                    _authService.LoginSession.SetTransmissionMode(TransmissionMode.Single, value.Channel);
                }
            }
        }

        private List<ChannelId> _channelIds = new List<ChannelId>();
        public List<ChannelId> ChannelIds => _channelIds;

        private readonly VivoxAuthService _authService = VivoxAuthService.Instance;
        public static VivoxChannelService Instance => _instance ?? (_instance = new VivoxChannelService());
        private static VivoxChannelService _instance;

        public ChannelId GetChannelId(int index) {
            if (_channelIds.IsValidIndex(index)) {
                return _channelIds[index];
            }

            return null;
        }
                
        private VivoxChannelService()
        {
            _instance = this;
        }

        public void JoinChannel(string channelName, ChannelType channelType, ChatCapability chatCapability,
        bool switchTransmission = true, Channel3DProperties properties = null)
        {
            if (_authService.LoginState == LoginState.LoggedIn)
            {
                Debug.Log("JOINING...");
                string fixedChannelName = channelName.Replace(" ", "");
                ChannelId channelId = new ChannelId(_authService.AuthSettings.TokenIssuer, fixedChannelName, _authService.AuthSettings.Domain, channelType, properties);
                IChannelSession channelSession = _authService.LoginSession.GetChannelSession(channelId);
                channelSession.PropertyChanged += OnChannelPropertyChanged;
                channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
                channelSession.MessageLog.AfterItemAdded += OnMessageLogRecieved;
                channelSession.BeginConnect(chatCapability != ChatCapability.TextOnly, chatCapability != ChatCapability.AudioOnly, switchTransmission, channelSession.GetConnectToken(_authService.AuthSettings.TokenKey, _authService.AuthSettings.TokenExpiration), ar =>
                {
                    try
                    {
                        channelSession.EndConnect(ar);

                        _channelIds.Add(channelId);

                        M31Logger.LogInfo($"VIVOX: OnJoinedToChannel channelId: ${channelId.Name}", LogLevel);
                        OnJoinedToChannelEvent?.Invoke(channelId);
                    }
                    catch (Exception e)
                    {
                        // Handle error 
                        VivoxLogError($"Could not connect to voice channel: {e.Message}");
                        return;
                    }
                });
            }
            else
            {
                VivoxLogError("Cannot join a channel when not logged in.");
            }
        }

        public void SendTextMessage(string messageToSend, ChannelId channel, string applicationStanzaNamespace = null, string applicationStanzaBody = null)
        {
            if (ChannelId.IsNullOrEmpty(channel))
            {
                throw new ArgumentException("Must provide a valid ChannelId");
            }
            if (string.IsNullOrEmpty(messageToSend))
            {
                throw new ArgumentException("Must provide a message to send");
            }
            var channelSession = _authService.LoginSession.GetChannelSession(channel);
            channelSession.BeginSendText(null, messageToSend, applicationStanzaNamespace, applicationStanzaBody, ar =>
            {
                try
                {
                    channelSession.EndSendText(ar);
                }
                catch (Exception e)
                {
                    VivoxLog($"SendTextMessage failed with exception {e.Message}");
                }
            });
        }

        public void DisconnectChannel(string channelName)
        {
            if (ActiveChannels?.Count > 0)
            {
                foreach (var channelSession in ActiveChannels)
                {
                    // Debug.Log($"{channelSession?.Channel.Name} - {channelName}");
                    if (channelSession?.Channel.Name == channelName)
                    {
                        channelSession?.Disconnect();
                        break;
                    }
                }
            }
        }

        public void DisconnectAllChannels()
        {
            if (ActiveChannels?.Count > 0)
            {
                foreach (var channelSession in ActiveChannels)
                {
                    channelSession?.Disconnect();
                }
            }
        }

        private static void ValidateArgs(object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null)
                    throw new ArgumentNullException(obj.GetType().ToString(), "Specify a non-null/non-empty argument.");
            }
        }

        private void OnMessageLogRecieved(object sender, QueueItemAddedEventArgs<IChannelTextMessage> textMessage)
        {
            ValidateArgs(new object[] { sender, textMessage });

            IChannelTextMessage channelTextMessage = textMessage.Value;
            VivoxLog(channelTextMessage.Message);
            OnMessageLogRecievedEvent?.Invoke(channelTextMessage); // channelTextMessage.Sender.DisplayName,
        }

        private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] { sender, keyEventArg });

            // INFO: sender is the dictionary that changed and trigger the event.  Need to cast it back to access it.
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;
            // Look up the participant via the key.
            var participant = source[keyEventArg.Key]; // participant.channel.key // participant.account.name
            // Trigger callback
            OnParticipantAddedEvent?.Invoke(participant);
        }

        private void OnParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] { sender, keyEventArg });

            // INFO: sender is the dictionary that changed and trigger the event.  Need to cast it back to access it.
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;
            // Look up the participant via the key.
            var participant = source[keyEventArg.Key];
            var channelSession = participant.ParentChannelSession;

            if (participant.IsSelf)
            {
                VivoxLog($"Unsubscribing from: {channelSession.Key.Name}");
                // Now that we are disconnected, unsubscribe.
                channelSession.PropertyChanged -= OnChannelPropertyChanged;
                channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;
                channelSession.MessageLog.AfterItemAdded -= OnMessageLogRecieved;

                // Remove session.
                var user = _authService.LoginSession;
                user.DeleteChannelSession(channelSession.Channel);

                _channelIds.Remove(channelSession.Key);
            }

            // Trigger callback
            OnParticipantRemovedEvent?.Invoke(participant);
        }

        private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
        {
            ValidateArgs(new object[] { sender, valueEventArg });

            //string username = valueEventArg.Value.Account.DisplayName;
            //ChannelId channel = valueEventArg.Value.ParentChannelSession.Key;
            string property = valueEventArg.PropertyName;

            switch (property)
            {
                case "SpeechDetected":
                    {
                        // VivoxLog($"OnSpeechDetectedEvent: {username} in {channel.Name}.");
                        OnSpeechDetectedEvent?.Invoke(valueEventArg.Value, valueEventArg.Value.SpeechDetected);
                        break;
                    }
                case "AudioEnergy":
                    {
                        OnAudioEnergyChangedEvent?.Invoke(valueEventArg.Value, valueEventArg.Value.AudioEnergy);
                        break;
                    }
                default:
                    break;
            }
        }

        private void OnChannelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ValidateArgs(new object[] { sender, propertyChangedEventArgs });

            //if (_client == null)
            //    throw new InvalidClient("Invalid client.");
            var channelSession = (IChannelSession)sender;

            // IF the channel has removed audio, make sure all the VAD indicators aren't showing speaking.
            if (propertyChangedEventArgs.PropertyName == "AudioState" && channelSession.AudioState == ConnectionState.Disconnected)
            {
                VivoxLog($"Audio disconnected from: {channelSession.Key.Name}");

                foreach (var participant in channelSession.Participants)
                {
                    OnSpeechDetectedEvent?.Invoke(participant, false);
                }
            }

            // IF the channel has fully disconnected, unsubscribe and remove.
            if ((propertyChangedEventArgs.PropertyName == "AudioState" || propertyChangedEventArgs.PropertyName == "TextState") &&
                channelSession.AudioState == ConnectionState.Disconnected &&
                channelSession.TextState == ConnectionState.Disconnected)
            {
                VivoxLog($"Unsubscribing from: {channelSession.Key.Name}");
                // Now that we are disconnected, unsubscribe.
                channelSession.PropertyChanged -= OnChannelPropertyChanged;
                channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;
                channelSession.MessageLog.AfterItemAdded -= OnMessageLogRecieved;

                // Remove session.
                var user = _authService.LoginSession;
                user.DeleteChannelSession(channelSession.Channel);

                _channelIds.Remove(channelSession.Key);
            }
        }

        private void VivoxLog(string msg)
        {
            M31Logger.LogInfo("<color=green>VivoxVoice: </color>: " + msg, LogLevel);
        }

        private void VivoxLogError(string msg)
        {
            M31Logger.LogError("<color=green>VivoxVoice: </color>: " + msg, LogLevel);
        }
    }

    public enum ChatCapability
    {
        TextOnly,
        AudioOnly,
        TextAndAudio
    };

    [Serializable]
    public struct VivoxJoinChannelParamsStruct {
        public string channelName;
        public ChannelType channelType;
        public ChatCapability chatCapability;
        [Tooltip("Default: true")]
        public bool switchTransmission; // true
        [Tooltip("Default: null")]
        public Channel3DPropertiesStruct properties; // null

        public Channel3DProperties GetChannel3DProperties() {
            if (properties.audibleDistance == 0 && properties.conversationalDistance == 0 && properties.audioFadeIntensityByDistanceaudio == 0.0f && properties.audioFadeModel == AudioFadeModel.None) return null;
            return new Channel3DProperties(properties.audibleDistance, properties.conversationalDistance, properties.audioFadeIntensityByDistanceaudio, properties.audioFadeModel);
        }
    }

    [Serializable]
    public struct Channel3DPropertiesStruct {
        [Tooltip("Default: 32")]
        public int audibleDistance; 
        [Tooltip("Default: 1")]
        public int conversationalDistance; 
        [Tooltip("Default: 1.0f")]
        public float audioFadeIntensityByDistanceaudio;
        [Tooltip("Default: AudioFadeModel.InverseByDistance")]
        public AudioFadeModel audioFadeModel;
    }

    [Serializable]
    public struct SendTextParamsStruct {
        public string messageToSend;
        [DropdownIndex("ChannelNames", SearchOnUnityObject = true)]
        public int channel;
        [Tooltip("Default: null")]
        public string applicationStanzaNamespace; // null
        [Tooltip("Default: null")]
        public string applicationStanzaBody; // null
    }
}
#endif
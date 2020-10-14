#if VIVOX
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using d4160.Core.MonoBehaviours;
using d4160.GameFramework.Authentication;
using NaughtyAttributes;
using UnityEngine;
using VivoxUnity;

namespace d4160.GameFramework.Networking
{
    public class VivoxNetworkManager : Singleton<VivoxNetworkManager>
    {
#region Enums

        /// <summary>
        /// Defines properties that can change.  Used by the functions that subscribe to the OnAfterTYPEValueUpdated functions.
        /// </summary>
        public enum ChangedProperty
        {
            None,
            Speaking,
            Typing,
            Muted
        }

        public enum ChatCapability
        {
            TextOnly,
            AudioOnly,
            TextAndAudio
        };

#endregion

#region Delegates/Events

        public delegate void ParticipantValueChangedHandler(string username, ChannelId channel, bool value);

        public event ParticipantValueChangedHandler OnSpeechDetectedEvent;

        public delegate void ParticipantValueUpdatedHandler(string username, ChannelId channel, double value);

        public event ParticipantValueUpdatedHandler OnAudioEnergyChangedEvent;


        public delegate void ParticipantStatusChangedHandler(string username, ChannelId channel,
            IParticipant participant);

        public event ParticipantStatusChangedHandler OnParticipantAddedEvent;
        public event ParticipantStatusChangedHandler OnParticipantRemovedEvent;

        public delegate void ChannelTextMessageChangedHandler(string sender, IChannelTextMessage channelTextMessage);

        public event ChannelTextMessageChangedHandler OnTextMessageLogReceivedEvent;

#endregion

#region Member Variables
        [Expandable]
        [SerializeField] private VivoxAuthSettings _authSettings;
        [SerializeField] private string _displayName;

        private readonly VivoxAuthService _authService = VivoxAuthService.Instance;
        private Client _client = new Client();

        public VivoxUnity.IReadOnlyDictionary<ChannelId, IChannelSession> ActiveChannels =>
            _authService.LoginSession?.ChannelSessions;

        public IAudioDevices AudioInputDevices => _client.AudioInputDevices;
        public IAudioDevices AudioOutputDevices => _client.AudioOutputDevices;

#endregion

#region Properties

        public string UniqueId
        {
            get => _authService.Id;
            set => _authService.Id = value;
        }

        public string DisplayName
        {
            get => _authService.DisplayName;
            set => _authService.DisplayName = value;
        }

        /// <summary>
        /// Retrieves the first instance of a session that is transmitting. 
        /// </summary>
        public IChannelSession TransmittingSession
        {
            get
            {
                if (_client == null)
                    throw new NullReferenceException("client");
                return _client.GetLoginSession(_authService.AccountId).ChannelSessions.FirstOrDefault(x => x.IsTransmitting);
            }
            set
            {
                if (value != null)
                {
                    _client.GetLoginSession(_authService.AccountId).SetTransmissionMode(TransmissionMode.Single, value.Channel);
                }
            }
        }

#endregion

        protected virtual void Start()
        {
            if (_authSettings)
            {
                if (_authSettings.IsInvalid)
                {
                    Debug.LogError(
                        "The default VivoxVoiceServer values (Server, Domain, TokenIssuer, and TokenKey) must be replaced with application specific issuer and key values from your developer account.");
                }
            }
            else
            {
                Debug.LogError(
                    "Assing one Vivox Auth Settings asset in the manager.", gameObject);
            }

            _client.Uninitialize();

            _client.Initialize();
        }

        protected virtual void OnApplicationQuit()
        {
            // Needed to add this to prevent some unsuccessful uninit, we can revisit to do better -carlo
            Client.Cleanup();
            if (_client != null)
            {
                VivoxLog("Uninitializing client.");
                _client.Uninitialize();
                _client = null;
            }
        }

        [Button]
        public void SetDisplayName()
        {
            DisplayName = _displayName;

            Debug.Log($"Vivox DisplayName changed to: {DisplayName}. Ready to Login.");
        }

        [Button]
        public virtual void Connect()
        {
            _authService.Client = _client;
            _authService.AuthSettings = _authSettings;

            _authService.Authenticate();
        }

        [Button]
        public virtual void Disconnect()
        {
            _authService.Unauthenticate();
        }

        public void JoinChannel(string channelName, ChannelType channelType, ChatCapability chatCapability,
            bool switchTransmission = true, Channel3DProperties properties = null)
        {
            if (_authService.LoginState == LoginState.LoggedIn)
            {

                ChannelId channelId = new ChannelId(_authSettings.TokenIssuer, channelName, _authSettings.Domain, channelType, properties);
                IChannelSession channelSession = _authService.LoginSession.GetChannelSession(channelId);
                channelSession.PropertyChanged += OnChannelPropertyChanged;
                channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
                channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
                channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
                channelSession.MessageLog.AfterItemAdded += OnMessageLogRecieved;
                channelSession.BeginConnect(chatCapability != ChatCapability.TextOnly,
                    chatCapability != ChatCapability.AudioOnly, switchTransmission,
                    channelSession.GetConnectToken(_authSettings.TokenKey, _authSettings.TokenExpiration), ar =>
                    {
                        try
                        {
                            channelSession.EndConnect(ar);
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

        public void SendTextMessage(string messageToSend, ChannelId channel, string applicationStanzaNamespace = null,
            string applicationStanzaBody = null)
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
            if (!(ActiveChannels?.Count > 0)) return;
            var channelSession = ActiveChannels.FirstOrDefault((cs) => cs.Channel.Name == channelName);
            channelSession?.Disconnect();
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

#region Vivox Callbacks

        private void OnMessageLogRecieved(object sender, QueueItemAddedEventArgs<IChannelTextMessage> textMessage)
        {
            ValidateArgs(new object[] {sender, textMessage});

            IChannelTextMessage channelTextMessage = textMessage.Value;
            VivoxLog(channelTextMessage.Message);
            OnTextMessageLogReceivedEvent?.Invoke(channelTextMessage.Sender.DisplayName, channelTextMessage);
        }

        private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] {sender, keyEventArg});

            // INFO: sender is the dictionary that changed and trigger the event.  Need to cast it back to access it.
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>) sender;
            // Look up the participant via the key.
            var participant = source[keyEventArg.Key];
            var username = participant.Account.Name;
            var channel = participant.ParentChannelSession.Key;
            var channelSession = participant.ParentChannelSession;

            // Trigger callback
            OnParticipantAddedEvent?.Invoke(username, channel, participant);
        }

        private void OnParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
        {
            ValidateArgs(new object[] {sender, keyEventArg});

            // INFO: sender is the dictionary that changed and trigger the event.  Need to cast it back to access it.
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>) sender;
            // Look up the participant via the key.
            var participant = source[keyEventArg.Key];
            var username = participant.Account.Name;
            var channel = participant.ParentChannelSession.Key;
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
                var user = _client.GetLoginSession(_authService.AccountId);
                user.DeleteChannelSession(channelSession.Channel);
            }

            // Trigger callback
            OnParticipantRemovedEvent?.Invoke(username, channel, participant);
        }

        private static void ValidateArgs(object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null)
                    throw new ArgumentNullException(obj.GetType().ToString(), "Specify a non-null/non-empty argument.");
            }
        }

        private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
        {
            ValidateArgs(new object[] {sender, valueEventArg});

            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>) sender;
            // Look up the participant via the key.
            var participant = source[valueEventArg.Key];

            string username = valueEventArg.Value.Account.Name;
            ChannelId channel = valueEventArg.Value.ParentChannelSession.Key;
            string property = valueEventArg.PropertyName;

            switch (property)
            {
                case "SpeechDetected":
                {
                    VivoxLog($"OnSpeechDetectedEvent: {username} in {channel}.");
                    OnSpeechDetectedEvent?.Invoke(username, channel, valueEventArg.Value.SpeechDetected);
                    break;
                }
                case "AudioEnergy":
                {
                    OnAudioEnergyChangedEvent?.Invoke(username, channel, valueEventArg.Value.AudioEnergy);
                    break;
                }
                default:
                    break;
            }
        }

        private void OnChannelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ValidateArgs(new object[] {sender, propertyChangedEventArgs});

            //if (_client == null)
            //    throw new InvalidClient("Invalid client.");
            var channelSession = (IChannelSession) sender;

            // IF the channel has removed audio, make sure all the VAD indicators aren't showing speaking.
            if (propertyChangedEventArgs.PropertyName == "AudioState" &&
                channelSession.AudioState == ConnectionState.Disconnected)
            {
                VivoxLog($"Audio disconnected from: {channelSession.Key.Name}");

                foreach (var participant in channelSession.Participants)
                {
                    OnSpeechDetectedEvent?.Invoke(participant.Account.Name, channelSession.Channel, false);
                }
            }

            // IF the channel has fully disconnected, unsubscribe and remove.
            if ((propertyChangedEventArgs.PropertyName == "AudioState" ||
                 propertyChangedEventArgs.PropertyName == "TextState") &&
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
                var user = _client.GetLoginSession(_authService.AccountId);
                user.DeleteChannelSession(channelSession.Channel);

            }
        }

#endregion

        private void VivoxLog(string msg)
        {
            Debug.Log("<color=green>VivoxVoice: </color>: " + msg);
        }

        private void VivoxLogError(string msg)
        {
            Debug.LogError("<color=green>VivoxVoice: </color>: " + msg);
        }
    }
}
#endif
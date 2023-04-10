using Agora.Rtc;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Channel")]
    public class AgoraRtcChannelSO : ScriptableObject
    {
        [SerializeField] private string _token;
        [SerializeField] private string _channelName;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [EnumFlags]
#endif
        [SerializeField] private JoinChannelModules _modulesToActiveWhenJoin;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_IsVideoModule"), Expandable]
#endif
        [SerializeField] private VideoEncoderConfigurationSO _videoEncoderConfig;
        [SerializeField] private CLIENT_ROLE_TYPE _clientRoleType = CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER;
        [SerializeField] private CHANNEL_PROFILE_TYPE _channelProfileType = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ChannelMediaOptionsSO _channelMediaOptions;
        [SerializeField] private bool _setMediaOptionsWhenJoin = false;

        public string ChannelName { get => _channelName; set => _channelName = value; }

#if UNITY_EDITOR
        private bool _IsVideoModule => (_modulesToActiveWhenJoin & JoinChannelModules.EnableVideo) != 0;
#endif

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinChannel()
        {
            _service.JoinChannel(_token, _channelName, _modulesToActiveWhenJoin, _clientRoleType, _channelProfileType, _videoEncoderConfig.AgoraRtcVideoEncoderConfiguration, _setMediaOptionsWhenJoin ? _channelMediaOptions.GetChannelMediaOptions() : null);
        }

        public void JoinChannel(string channelName)
        {
            _service.JoinChannel(_token, channelName, _modulesToActiveWhenJoin, _clientRoleType, _channelProfileType, _videoEncoderConfig.AgoraRtcVideoEncoderConfiguration, _setMediaOptionsWhenJoin ? _channelMediaOptions.GetChannelMediaOptions() : null);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void LeaveChannel()
        {
            _service.LeaveChannel();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableAudio()
        {
            _service.EnableAudio();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisableAudio()
        {
            _service.DisableAudio();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableVideo()
        {
            _service.EnableVideo();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisableVideo()
        {
            _service.DisableVideo();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableLocalVideo()
        {
            if ((_modulesToActiveWhenJoin & JoinChannelModules.EnableLocalVideo) == 0)
            {
                _service.EnableLocalVideo(true);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisableLocalVideo()
        {
            if ((_modulesToActiveWhenJoin & JoinChannelModules.EnableLocalVideo) == 0)
            {
                _service.EnableLocalVideo(false);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableLocalAudio()
        {
            _service.EnableLocalAudio(true);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DisableLocalAudio()
        {
            _service.EnableLocalAudio(false);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void PauseMedia()
        {
            var options = new ChannelMediaOptions();
            options.publishMicrophoneTrack.SetValue(false);
            options.publishCameraTrack.SetValue(false);

            _service.UpdateChannelMediaOptions(options);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ResumeMedia()
        {
            var options = new ChannelMediaOptions();
            options.publishMicrophoneTrack.SetValue(true);
            options.publishCameraTrack.SetValue(true);
            _service.UpdateChannelMediaOptions(options);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UpdateChannelMediaOptions()
        {
            _service.RtcEngine.UpdateChannelMediaOptions(_channelMediaOptions.GetChannelMediaOptions());
        }
    }
}
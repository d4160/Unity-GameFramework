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

#if UNITY_EDITOR
        private bool _IsVideoModule => (_modulesToActiveWhenJoin & JoinChannelModules.EnableVideo) != 0;
#endif

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinChannel()
        {
            _service.JoinChannel(_token, _channelName, _modulesToActiveWhenJoin, _clientRoleType, _channelProfileType, _videoEncoderConfig.AgoraRtcVideoEncoderConfiguration);
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
        public void EnableVideo()
        {
            _service.EnableVideo();
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
        public void DisableVideo()
        {
            _service.DisableVideo();
        }
    }
}
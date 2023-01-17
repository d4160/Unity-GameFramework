using Agora.Rtc;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Settings")]
    public class AgoraRtcSettingsSO : ScriptableObject
    {
        [SerializeField] private string _appID = "your_appid";
        [SerializeField] private ulong _context = 0;
        [SerializeField] private CHANNEL_PROFILE_TYPE _channelProfileType = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        [SerializeField] private AUDIO_SCENARIO_TYPE _audioScenarioType = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;

        public string AppID => _appID;
        public ulong Context => _context;
        public CHANNEL_PROFILE_TYPE ChannelProfileType => _channelProfileType;
        public AUDIO_SCENARIO_TYPE AudioScenarioType => _audioScenarioType;
    }
}
using Agora.Rtc;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Settings")]
    public class AgoraRtcSettingsSO : ScriptableObject
    {
        [SerializeField] private string _appID = "your_appid";
        [SerializeField] private ulong _context = 0;
        [SerializeField] private string _license = "";
        [SerializeField] private CHANNEL_PROFILE_TYPE _channelProfileType = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        [SerializeField] private AUDIO_SCENARIO_TYPE _audioScenarioType = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        [SerializeField] private AREA_CODE _areaCode = AREA_CODE.AREA_CODE_GLOB;

        [SerializeField] private bool _useExternalEglContext = false;
        [SerializeField] private bool _domainLimit = false;
        [SerializeField] private bool _autoRegisterAgoraExtensions = true;

        // AppID Contiverso: 2a048ad297754c119bf5e00c39fcf313
        // AppID Temp: b0f346799b524fa1b625ae7d9eb5f9d0

        public string AppID => _appID;
        public ulong Context => _context;
        public string License => _license;
        public CHANNEL_PROFILE_TYPE ChannelProfileType => _channelProfileType;
        public AUDIO_SCENARIO_TYPE AudioScenarioType => _audioScenarioType;
        public AREA_CODE AreaCode => _areaCode;
        public bool UseExternalEglContext => _useExternalEglContext;
        public bool DomainLimit => _domainLimit;
        public bool AutoRegisterAgoraExtensions => _autoRegisterAgoraExtensions;

        public void SetAppId(string appId)
        {
            _appID = appId;
        }


        public RtcEngineContext GetRtcEngineContext()
        {
            var context = new RtcEngineContext();
            context.appId = _appID;
            context.context = _context;
            context.license = _license;
            context.areaCode = _areaCode;
            context.channelProfile = _channelProfileType;
            context.audioScenario = _audioScenarioType;
            context.useExternalEglContext = _useExternalEglContext;
            context.domainLimit = _domainLimit;
            context.autoRegisterAgoraExtensions = _autoRegisterAgoraExtensions;

            return context;
        }
    }
}
using d4160.Events;
using d4160.Logging;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Service")]
    public class AgoraRtcServiceSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private AgoraRtcSettingsSO _settings;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private LoggerSO _logger;
        [SerializeField] private IntStringEventSO _onError;
        [SerializeField] private JoinChannelEventSO _onJoinChannelSuccess;
        [SerializeField] private JoinChannelEventSO _onRejoinChannelSuccess;
        [SerializeField] private LeaveChannelEventSO _onLeaveChannelSuccess;
        [SerializeField] private ClientRoleChangedEventSO _onClientRoleChanged;
        [SerializeField] private UserJoinedEventSO _onUserJoined;
        [SerializeField] private UserOfflineEventSO _onUserOffline;

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

        public void Setup()
        {
            _service.Settings = _settings;
            _service.Logger = _logger;
        }

        public void RegisterEvents()
        {
            _service.OnError = _onError;
            _service.OnJoinChannelSuccess = _onJoinChannelSuccess;
            _service.OnRejoinChannelSuccess = _onRejoinChannelSuccess;
            _service.OnLeaveChannelSuccess = _onLeaveChannelSuccess;
            _service.OnClientRoleChanged = _onClientRoleChanged;
            _service.OnUserJoined = _onUserJoined;
            _service.OnUserOffline = _onUserOffline;
        }

        public void UnregisterEvents()
        {
            _service.OnError = null;
            _service.OnJoinChannelSuccess = null;
            _service.OnRejoinChannelSuccess = null;
            _service.OnLeaveChannelSuccess = null;
            _service.OnClientRoleChanged = null;
            _service.OnUserJoined = null;
            _service.OnUserOffline = null;
        }

        public void InitRtcEngine()
        {
            _service.InitRtcEngine();
        }

        public void DisposeRtcEngine()
        {
            _service.DisposeRtcEngine();
        }
    }
}
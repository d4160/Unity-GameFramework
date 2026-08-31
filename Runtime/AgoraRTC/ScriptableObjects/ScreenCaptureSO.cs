using Agora.Rtc;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/ScreenCapture")]
    public class ScreenCaptureSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ScreenCaptureParametersSO _screenCaptureParameters;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DeviceOptionsSO _deviceOptions;

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public int StartScreenCapture()
        {
            if (_service.RtcEngine == null) { Debug.LogWarning("[ScreenCaptureSO] StartScreenCapture skipped — RtcEngine is null."); return -1; }

            int nRet = -1;
            ScreenCaptureSourceInfo selectedInfo = _deviceOptions != null && _deviceOptions.ScreenCaptureSourceInfoRuntimeSet != null
                ? _deviceOptions.ScreenCaptureSourceInfoRuntimeSet.SelectedScreenCaptureSource
                : null;

            // BugFix: If no screen capture source was selected yet (because discovery is now on-demand),
            // fetch available sources immediately and default to the primary screen/window.
            if (selectedInfo == null && _deviceOptions != null)
            {
                var sources = _deviceOptions.GetScreenCaptureSources("Monitor");
                if (sources != null && sources.Count > 0)
                {
                    _deviceOptions.SetScreenCaptureSource(0);
                    selectedInfo = _deviceOptions.ScreenCaptureSourceInfoRuntimeSet != null
                        ? _deviceOptions.ScreenCaptureSourceInfoRuntimeSet.SelectedScreenCaptureSource
                        : null;
                }
            }

            if (selectedInfo != null)
            {
                _service.RtcEngine.StopScreenCapture();

                switch (selectedInfo.type)
                {
                    case ScreenCaptureSourceType.ScreenCaptureSourceType_Unknown:
                        break;
                    case ScreenCaptureSourceType.ScreenCaptureSourceType_Window:
                        nRet = _service.RtcEngine.StartScreenCaptureByWindowId(selectedInfo.sourceId, default,
                            _screenCaptureParameters.GetScreenCaptureParameters());
                        break;
                    case ScreenCaptureSourceType.ScreenCaptureSourceType_Screen:
                        nRet = _service.RtcEngine.StartScreenCaptureByDisplayId((uint)selectedInfo.sourceId, default,
                            _screenCaptureParameters.GetScreenCaptureParameters());
                        break;
                    case ScreenCaptureSourceType.ScreenCaptureSourceType_Custom:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogWarning("[ScreenCaptureSO] StartScreenCapture: No screen capture source found to share.");
            }

            return nRet;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public int StopScreenCapture()
        {
            if (_service.RtcEngine == null) { Debug.LogWarning("[ScreenCaptureSO] StopScreenCapture skipped — RtcEngine is null."); return -1; }
            return _service.RtcEngine.StopScreenCapture();
        }
    }
}
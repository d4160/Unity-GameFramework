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
            int nRet = -1;
            ScreenCaptureSourceInfo selectedInfo = _deviceOptions.ScreenCaptureSourceInfoRuntimeSet.SelectedScreenCaptureSource;

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

            return nRet;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public int StopScreenCapture() => _service.RtcEngine.StopScreenCapture();
    }
}
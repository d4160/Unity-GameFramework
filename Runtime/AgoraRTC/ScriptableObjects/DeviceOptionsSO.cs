using Agora.Rtc;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
using System.Collections.Generic;
#endif
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/DeviceOptions")]
    public class DeviceOptionsSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private VideoSurfaceProviderSO _vSurfaceProvider;

        [Header("PreviewSize")]
        public Vector2Int thumbSize = new (640, 360);
        public Vector2Int iconSize = new (256, 256);
        public bool includeScreen = true;

        [Header("Variables")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private IVideoDeviceManagerVarSO _videoManagerVar;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private IAudioDeviceManagerVarSO _audioManagerVar;

        [Header("Collections")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ScreenCaptureSourceInfoRuntimeSetSO _captureSourceInfoRuntimeSet;

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

        public IVideoDeviceManagerVarSO VideoManagerVar => _videoManagerVar;
        public IAudioDeviceManagerVarSO AudioManagerVar => _audioManagerVar;
        public ScreenCaptureSourceInfoRuntimeSetSO ScreenCaptureSourceInfoRuntimeSet => _captureSourceInfoRuntimeSet;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StartPreview()
        {
            _service.StartPreview();
            _vSurfaceProvider.EnableLocalVideoSurface();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StopPreview()
        {
            _vSurfaceProvider.DisableLocalVideoSurface();
            _service.StopPreview();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public DeviceInfo[] GetVideoDevices()
        {
            if (_service.RtcEngine == null) return null;
            _videoManagerVar.Value = new (_service.GetVideoDeviceManager());
            return _videoManagerVar.Value.EnumerateVideoDevices();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void GetAudioDeviceManager()
        {
            _audioManagerVar.Value = _service.GetAudioDeviceManager();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public List<ScreenCaptureSourceInfo> GetScreenCaptureSources(params string[] ignores)
        {
            if (_service.RtcEngine == null) return null;

            SIZE thumbS = new (thumbSize.x, thumbSize.y);
            SIZE iconS = new (iconSize.x, iconSize.y);

            ScreenCaptureSourceInfo[] sources = _service.GetScreenCaptureSources(thumbS, iconS, includeScreen);
            
            if (sources == null || sources.Length == 0) return null;

            _captureSourceInfoRuntimeSet.SetScreenCaptureSources(sources, ignores);

            return _captureSourceInfoRuntimeSet.Items;
        }

        public List<Texture2D> GetScreenCaptureSourceThumbs() => _captureSourceInfoRuntimeSet.GetScreenCaptureSourceThumbs();
        public List<Texture2D> GetScreenCaptureSourceIcons() => _captureSourceInfoRuntimeSet.GetScreenCaptureSourceIcons();

        public int SetVideoDevice(int index)
        {
            if (_videoManagerVar.Value != null)
            {
                return _videoManagerVar.Value.SetDevice(index);
            }

            return -1;
        }

        public void SetScreenCaptureSource(int index) => _captureSourceInfoRuntimeSet.SetScreenCaptureSource(index);
    }
}
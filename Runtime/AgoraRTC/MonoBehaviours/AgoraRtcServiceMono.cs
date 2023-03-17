using Agora.Rtc;
using d4160.MonoBehaviours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.AgoraRtc
{
    public class AgoraRtcServiceMono : MonoBehaviourUnityData<AgoraRtcServiceSO, VideoSurfaceProviderSO, UsersRuntimeSetSO>
    {
        [Header("Instances")]
        [SerializeField] private VideoSurface _localVSurface;
        [SerializeField] private List<VideoSurface> _staticVSurfaces;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        [ContextMenu("ScaleVideoSurfaces")]
        private void ScaleVideoSurfaces()
        {
            if (_localVSurface)
            {
                _data2.FixVideoSurface(_localVSurface);
            }

            for (int i = 0; i < _staticVSurfaces.Count; i++)
            {
                var surface = _staticVSurfaces[i];
                _data2.FixVideoSurface(surface);
            }
        }

        private void Awake()
        {
            if (_data1)
            {
                _data1.Setup();
                _data1.InitRtcEngine();
            }

            if (_data2)
            {
                _data2.LocalVideoSurface = _localVSurface;
                _data2.StaticVideoSurfaces = _staticVSurfaces;
                _data2.Setup();
            }

            if (_data3)
            {
                _data3.Setup();
            }
        }

        private void OnEnable()
        {
            if (_data1) _data1.RegisterEvents();
            if (_data3) _data3.RegisterEvents();
        }

        private void OnDisable()
        {
            if (_data1) _data1.UnregisterEvents();
            if (_data3) _data3.UnregisterEvents();
        }

        private void OnDestroy()
        {
            if (_data1) _data1.DisposeRtcEngine();
        }
    }
}
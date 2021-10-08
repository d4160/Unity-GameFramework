#if AGORA
using agora_gaming_rtc;
using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using d4160.MonoBehaviourData;
using UltEvents;

namespace d4160.Chat.Agora
{
    public class AgoraScreenShareBehaviour : MonoBehaviourUnityData<AgoraScreenShareSO>
    {
        [SerializeField] VideoSurface _videoSurface;
        
        [Header("EVENTS")]
        [SerializeField] private UltEvent _onStartScreenCaptureEvent;
        [SerializeField] private UltEvent _onStopScreenCaptureEvent;

        void OnEnable () {
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnStartScreenCaptureEvent += _onStartScreenCaptureEvent.Invoke;
                _data.OnStopScreenCaptureEvent += _onStopScreenCaptureEvent.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnStartScreenCaptureEvent -= _onStartScreenCaptureEvent.Invoke;
                _data.OnStopScreenCaptureEvent -= _onStopScreenCaptureEvent.Invoke;
            }
        }   
        
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StartScreenCapture() {
            if (_data)
            {
                _data.VideoSurface = _videoSurface;
                _data.StartScreenCapture();
            }
        }

        public void StartScreenCapture(ScreenCaptureParametersStruct sparams)
        {
            if (_data)
            {
                _data.VideoSurface = _videoSurface;
                _data.StartScreenCapture(sparams);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StopScreenCapture(){
            if (_data)
                _data.StopScreenCapture();
        } 
    }
}
#endif
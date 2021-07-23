#if AGORA
using agora_gaming_rtc;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace d4160.Chat.Agora
{
    [CreateAssetMenu(menuName = "d4160/Agora/ScreenShare")]
    public class AgoraScreenShareSO : ScriptableObject
    {
        public event Action OnStartScreenCaptureEvent;
        public event Action OnStopScreenCaptureEvent;

        [SerializeField] private ScreenCaptureParametersStruct _screenCaptureParams;

        private readonly AgoraScreenShareService _screenShareService = AgoraScreenShareService.Instance;

        /// <summary>
        /// The first and default display
        /// </summary>
        /// <value></value>
        public int DisplayID => _screenShareService.DisplayID;
        public VideoSurface VideoSurface { get; set; }

        public void RegisterEvents () {
            AgoraScreenShareService.OnStartScreenCaptureEvent += OnStartScreenCaptureEvent;
            AgoraScreenShareService.OnStopScreenCaptureEvent += OnStopScreenCaptureEvent;
        }

        public void UnregisterEvents(){
            AgoraScreenShareService.OnStartScreenCaptureEvent -= OnStartScreenCaptureEvent;
            AgoraScreenShareService.OnStopScreenCaptureEvent -= OnStopScreenCaptureEvent;
        }   

        [Button]
        public void StartScreenCapture() {
            StartScreenCapture(_screenCaptureParams);
        }

        public void StartScreenCapture(ScreenCaptureParametersStruct sparams)
        {
            _screenShareService.VideoSurface = VideoSurface;
            _screenShareService.StartScreenCapture(sparams);
        }

        [Button]
        public void StopScreenCapture(){
            _screenShareService.StopScreenCapture();
        } 
    }
}
#endif
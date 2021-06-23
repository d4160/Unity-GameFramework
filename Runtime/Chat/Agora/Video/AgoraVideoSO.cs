#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using Logger = d4160.Logging.M31Logger;
using NaughtyAttributes;

namespace d4160.Chat.Agora
{
    [CreateAssetMenu(menuName = "d4160/Chat/Agora Video")]
    public class AgoraVideoSO : ScriptableObject
    {
        [SerializeField] private bool _enableVideo;

        public event Action<uint, int, int, int> OnVideoSizeChangedEvent;

        private readonly AgoraVideoService _videoService = AgoraVideoService.Instance; 

        public VideoSurface VideoSurface { get => _videoService.VideoSurface; set => _videoService.VideoSurface = value; }

        private void CallOnVideoSizeChanged(uint uid, int width, int height, int rotation) => OnVideoSizeChangedEvent?.Invoke(uid, width, height, rotation);

        public void RegisterEvents () {
            AgoraVideoService.OnVideoSizeChangedEvent += CallOnVideoSizeChanged;
        }

        public void UnregisterEvents(){
            AgoraVideoService.OnVideoSizeChangedEvent -= CallOnVideoSizeChanged;
        }   

        [Button]
        public void SetEnableVideo() {
            SetEnableVideo(_enableVideo);
        }

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableVideo(bool enableVideo)
        {
            _videoService.SetEnableVideo(enableVideo);
        }
        
        [Button]
        public void StartVideoPreview() {
            _videoService.StartVideoPreview();
        }

        [Button]
        public void StopVideoPreview() {
            _videoService.StopVideoPreview();
        }
    }
}
#endif
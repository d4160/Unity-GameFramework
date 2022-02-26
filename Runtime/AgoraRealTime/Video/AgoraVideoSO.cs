#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using Logger = d4160.Logging.LoggerM31;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Chat.Agora
{
    [CreateAssetMenu(menuName = "d4160/Agora/Video")]
    public class AgoraVideoSO : ScriptableObject
    {
        [Tooltip("Enable o disable video module, this is required to join the call as video mode")]
        [SerializeField] private bool _enableVideoModule;

        [Tooltip("To get raw video data (can be modified pre and post), and to send the video pictures directly to the app instead of to the traditional view renderer")]
        [SerializeField] private bool _enableVideoObserver;

        [Tooltip("Whether to enable the camera to create the local video stream. This is faster than EnableVideo().")]
        [SerializeField] private bool _enableLocalVideo = true;

        [Tooltip("Whether to publish the local video stream. This is faster than EnableLocalVideo().")]
        [SerializeField] private bool _muteLocalVideoStream = false;

        public event Action<uint, int, int, int> OnVideoSizeChangedEvent;

        private readonly AgoraVideoService _videoService = AgoraVideoService.Instance; 

        public VideoSurface VideoSurface { get => _videoService.VideoSurface; set => _videoService.VideoSurface = value; }

        private void CallOnVideoSizeChanged(uint uid, int width, int height, int rotation) => OnVideoSizeChangedEvent?.Invoke(uid, width, height, rotation);

        public void RegisterEvents () {
            _videoService.RegisterEvents();
            AgoraVideoService.OnVideoSizeChangedEvent += CallOnVideoSizeChanged;
        }

        public void UnregisterEvents(){
            _videoService.UnregisterEvents();
            AgoraVideoService.OnVideoSizeChangedEvent -= CallOnVideoSizeChanged;
        }   

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetEnableVideo() {
            SetEnableVideo(_enableVideoModule, _enableVideoObserver, _enableLocalVideo, _muteLocalVideoStream);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableLocalVideo() {
            EnableLocalVideo(_enableLocalVideo);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void MuteLocalVideoStream() {
            MuteLocalVideoStream(_muteLocalVideoStream);
        }

        /// <summary>
        ///   Enable/Disable video module
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableVideo(bool enableVideo, bool enableVideoObserver, bool enableLocalVideo = true, bool muteLocalVideoStream = false)
        {
            _videoService.SetEnableVideo(enableVideo, enableVideoObserver, enableLocalVideo, muteLocalVideoStream);
        }

        public void EnableLocalVideo(bool enableLocalVideo)
        {
            _videoService.EnableLocalVideo(enableLocalVideo);
        }

        public void EnableVideo(bool enableVideo)
        {
            _videoService.EnableVideo(enableVideo);
        }

        public void EnableVideoObserver(bool enableVideoObserver)
        {
            _videoService.EnableVideoObserver(enableVideoObserver);
        }

        public void MuteLocalVideoStream(bool muteLocalVideoStream)
        {
            _videoService.MuteLocalVideoStream(muteLocalVideoStream);
        }

        public void MuteRemoteVideoStream(uint uid, bool mute) {
            _videoService.MuteRemoteVideoStream(uid, mute);
        }

        public void MuteAllRemoteVideoStreams(bool mute) {
            _videoService.MuteAllRemoteVideoStreams(mute);
        }
        
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StartVideoPreview() {
            _videoService.StartVideoPreview();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StopVideoPreview() {
            _videoService.StopVideoPreview();
        }
    }
}
#endif
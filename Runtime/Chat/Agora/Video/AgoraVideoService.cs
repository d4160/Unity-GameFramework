#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using M31Logger = d4160.Logging.M31Logger;

namespace d4160.Chat.Agora
{
    public class AgoraVideoService
    {
        public static event Action<uint, int, int, int> OnVideoSizeChangedEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraAuthService _authService = AgoraAuthService.Instance; 
        public static AgoraVideoService Instance => _instance ?? (_instance = new AgoraVideoService());
        private static AgoraVideoService _instance;

        public VideoSurface VideoSurface { get; set; }

        private void CallOnVideoSizeChanged(uint uid, int width, int height, int rotation)
        {
            M31Logger.LogInfo("CallOnVideoSizeChanged: uid = " + uid, LogLevel);
            OnVideoSizeChangedEvent?.Invoke(uid, width, height, rotation);
        }

        private AgoraVideoService()
        {
            _instance = this;
        }

        public void RegisterEvents () {
            _authService.RtcEngine.OnVideoSizeChanged += CallOnVideoSizeChanged;
        }

        public void UnregisterEvents(){
            _authService.RtcEngine.OnVideoSizeChanged -= CallOnVideoSizeChanged;
        }   

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableVideo(bool enableVideo)
        {
            if (CheckErrors()) return;

            if (enableVideo)
            {
                _authService.RtcEngine.EnableVideo();
            }
            else
            {
                _authService.RtcEngine.DisableVideo();
            }
        }

        public void StartVideoPreview() {
            if (CheckErrors()) return;

            VideoSurface.SetEnable(true);

            _authService.RtcEngine.EnableVideo();
            _authService.RtcEngine.EnableVideoObserver();
            _authService.RtcEngine.StartPreview();

            // VideoManager only works after video enabled
            CheckDevices();
        }

        public void StopVideoPreview() {
            _authService.RtcEngine.DisableVideo();
            _authService.RtcEngine.DisableVideoObserver();
            _authService.RtcEngine.StopPreview();
        }

        /// <summary>
        ///   This method shows the CheckVideoDeviceCount API call.  It should only be used
        //  after EnableVideo() call.
        /// </summary>
        /// <param name="engine">Video Engine </param>
        void CheckDevices()
        {
            VideoDeviceManager deviceManager = VideoDeviceManager.GetInstance(_authService.RtcEngine);
            deviceManager.CreateAVideoDeviceManager();

            int cnt = deviceManager.GetVideoDeviceCount();
            M31Logger.LogInfo("AGORA: Device count =============== " + cnt, LogLevel);
        }

        private bool CheckErrors() {
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_authService.RtcEngine == null) {
                M31Logger.LogWarning("AGORA: RtcEngine is null", LogLevel);
                return true;
            }

            if (VideoSurface == null) {
                M31Logger.LogWarning("AGORA: VideoSurface is null", LogLevel);
                return true;
            }

            return false;
        }
    }
}
#endif
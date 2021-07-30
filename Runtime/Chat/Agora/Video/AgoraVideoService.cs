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

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance; 
        public static AgoraVideoService Instance => _instance ?? (_instance = new AgoraVideoService());
        private static AgoraVideoService _instance;

        public VideoSurface VideoSurface { get; set; }

        private void RaiseOnVideoSizeChanged(uint uid, int width, int height, int rotation)
        {
            M31Logger.LogInfo("OnVideoSizeChanged: uid = " + uid, LogLevel);
            OnVideoSizeChangedEvent?.Invoke(uid, width, height, rotation);
        }

        private AgoraVideoService()
        {
            _instance = this;
        }

        public void RegisterEvents () {
            _connection.RtcEngine.OnVideoSizeChanged += RaiseOnVideoSizeChanged;
        }

        public void UnregisterEvents(){
            _connection.RtcEngine.OnVideoSizeChanged -= RaiseOnVideoSizeChanged;
        }   

        /// <summary>
        ///   Enable/Disable video module
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableVideo(bool enableVideo, bool enableVideoObserver, bool enableLocalVideo = true, bool muteLocalVideoStream = false)
        {
            if (CheckErrors()) return;

            if (enableVideo)
            {
                _connection.RtcEngine.EnableVideo();
                if (enableVideoObserver) _connection.RtcEngine.EnableVideoObserver();

                if (!enableLocalVideo) EnableLocalVideo(false);
                else { if(muteLocalVideoStream) MuteLocalVideoStream(true); }
            }
            else
            {
                _connection.RtcEngine.DisableVideo();
                if (!enableVideoObserver) _connection.RtcEngine.DisableVideoObserver();
            }
        }

        public void EnableLocalVideo(bool enabled) {
            if (CheckErrors()) return;

            _connection.RtcEngine.EnableLocalVideo(enabled);
        }

        public void MuteLocalVideoStream(bool mute) {
            if (CheckErrors()) return;

            _connection.RtcEngine.MuteLocalVideoStream(mute);
        }

        public void MuteRemoteVideoStream(uint uid, bool mute) {
            if (CheckErrors()) return;

            _connection.RtcEngine.MuteRemoteVideoStream(uid, mute);
        }

        public void MuteAllRemoteVideoStreams(bool mute) {
            if (CheckErrors()) return;

            _connection.RtcEngine.MuteAllRemoteVideoStreams(mute);
        }
        
        /// <summary>
        /// Start the video preview locally without a channel, need to call EnableVideo() before
        /// </summary>
        public void StartVideoPreview() {
            if (CheckErrors()) return;

            // video surface with 0 uid is local by default
            VideoSurface.SetEnable(true);

            _connection.RtcEngine.StartPreview();

            // VideoManager only works after video enabled
            CheckDevices();
        }

        public void StopVideoPreview() {
            _connection.RtcEngine.StopPreview();
        }

        /// <summary>
        ///   This method shows the CheckVideoDeviceCount API call.  It should only be used
        //  after EnableVideo() call.
        /// </summary>
        /// <param name="engine">Video Engine </param>
        void CheckDevices()
        {
            VideoDeviceManager deviceManager = VideoDeviceManager.GetInstance(_connection.RtcEngine);
            deviceManager.CreateAVideoDeviceManager();

            int cnt = deviceManager.GetVideoDeviceCount();
            M31Logger.LogInfo("AGORA: Device count =============== " + cnt, LogLevel);
        }

        private bool CheckErrors() {
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_connection.RtcEngine == null) {
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
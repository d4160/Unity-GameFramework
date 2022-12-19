#if AGORA
using System;
using d4160.Core;
using Agora.Rtc;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;
using System.Collections.Generic;

namespace d4160.Agora_
{
    public class AgoraVideoService
    {
        public static event Action<uint, int, int, int> OnVideoSizeChangedEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance; 
        public static AgoraVideoService Instance => _instance ?? (_instance = new AgoraVideoService());
        private static AgoraVideoService _instance;

        private VideoDeviceManager _deviceManager;
        public VideoDeviceManager DeviceManager
        {
            get
            {
                if (_connection.RtcEngine != null && _deviceManager == null)
                {
                    //_deviceManager = VideoDeviceManager.GetInstance(_connection.RtcEngine);
                }

                return _deviceManager;
            }
        }

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

        public void RegisterEvents () 
        {
            if (_connection.RtcEngine != null)
            {
                //_connection.RtcEngine.OnVideoSizeChanged += RaiseOnVideoSizeChanged;
            }
        }

        public void UnregisterEvents()
        {
            if (_connection.RtcEngine != null)
            {
                //_connection.RtcEngine.OnVideoSizeChanged -= RaiseOnVideoSizeChanged;
            }
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
                //if (enableVideoObserver) _connection.RtcEngine.EnableVideoObserver();

                if (!enableLocalVideo) EnableLocalVideo(false);
                else { if(muteLocalVideoStream) MuteLocalVideoStream(true); }
            }
            else
            {
                _connection.RtcEngine.DisableVideo();
                //if (!enableVideoObserver) _connection.RtcEngine.DisableVideoObserver();
            }
        }

        public void EnableLocalVideo(bool enabled) {
            if (CheckErrors()) return;

            _connection.RtcEngine.EnableLocalVideo(enabled);
        }

        public void EnableVideo(bool enabled)
        {
            if (CheckErrors()) return;

            if (enabled) _connection.RtcEngine.EnableVideo();
            else _connection.RtcEngine.DisableVideo();
        }

        public void EnableVideoObserver(bool enabled)
        {
            if (CheckErrors()) return;

            //if (enabled) _connection.RtcEngine.EnableVideoObserver();
            //else _connection.RtcEngine.DisableVideoObserver();
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
            VideoSurface.SetForUser(0);
            VideoSurface.SetEnable(true);

            _connection.RtcEngine.StartPreview();
        }

        /// <summary>
        /// It should only be used
        //  after EnableVideo() call.
        /// </summary>
        /// <param name="engine">Video Engine </param>
        public Dictionary<string,string> GetVideoDevices()
        {
            //DeviceManager.CreateAVideoDeviceManager();

            //int count = DeviceManager.GetVideoDeviceCount();

            //if (count > 0)
            //{
            //    Dictionary<string, string> devices = new Dictionary<string, string>();
            //    for (int i = 0; i < count; i++)
            //    {
            //        string deviceName = null, deviceId = null;
            //        DeviceManager.GetVideoDevice(i, ref deviceName, ref deviceId);
            //        devices.Add(deviceId, deviceName);
            //    }
                
            //    return devices;
            //}

            return null;
            // M31Logger.LogInfo("AGORA: Device count =============== " + cnt, LogLevel);
        }

        public void SetVideoDevice(string deviceId)
        {
            //DeviceManager.SetVideoDevice(deviceId);
        }

        public void StopVideoPreview() {
            _connection.RtcEngine.StopPreview();
        }

        private bool CheckErrors() {
            
            if(!Application.isPlaying) {
                M31Logger.LogError("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if (_connection.RtcEngine == null) {
                M31Logger.LogError("AGORA: RtcEngine is null", LogLevel);
                return true;
            }

            if (VideoSurface == null) {
                M31Logger.LogWarning("AGORA: VideoSurface is null", LogLevel);
                return false;
            }

            return false;
        }
    }
}
#endif
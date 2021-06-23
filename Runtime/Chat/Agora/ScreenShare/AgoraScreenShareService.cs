#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Auth.Agora;
using UnityEngine;
using M31Logger = d4160.Logging.M31Logger;
using System.Collections.Generic;

namespace d4160.Chat.Agora
{
    public class AgoraScreenShareService
    {
        public static event Action<int, string> OnAuthError;
        public static event Action<int, string> OnAuthWarning;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraAuthService _authService = AgoraAuthService.Instance; 
        public static AgoraScreenShareService Instance => _instance ?? (_instance = new AgoraScreenShareService());
        private static AgoraScreenShareService _instance;

        /// <summary>
        /// The first and default display
        /// </summary>
        /// <value></value>
        public int DisplayID { get; private set; } = 0;
        public VideoSurface VideoSurface { get; set; }

        private AgoraScreenShareService()
        {
            _instance = this;
        }

        public void StartScreenCapture(ScreenCaptureParametersStruct sparams)
        {
            if (CheckErrors()) return;
            
            VideoSurface.SetEnable(true);

            _authService.RtcEngine.EnableVideo();
            _authService.RtcEngine.EnableVideoObserver();
            _authService.RtcEngine.StopScreenCapture();

    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            _authService.RtcEngine.StartScreenCaptureByDisplayId(getDisplayId(DisplayID), default(Rectangle), sparams.GetScreenCaptureParams());  // 
    #else
            StartScreenCaptureByScreenRect(DisplayID, sparams.GetScreenCaptureParams());
    #endif
            DisplayID = 1 - DisplayID; // only 0 and 1
        }

        private void StartScreenCaptureByScreenRect(int order, ScreenCaptureParameters sparams)
        {
            // Assuming you have two display monitors, each of 1920x1080, position left to right:
            Rectangle screenRect = new Rectangle() { x = 0, y = 0, width = 1920 * 2, height = 1080 };
            Rectangle regionRect = new Rectangle() { x = order * 1920, y = 0, width = 1920, height = 1080 };

            int rc = _authService.RtcEngine.StartScreenCaptureByScreenRect(screenRect,
                regionRect,
                sparams
                );
            if (rc != 0) Debug.LogWarning("rc = " + rc);
        }

        public void StopScreenCapture(){
            if (CheckErrors()) return;

            _authService.RtcEngine.DisableVideo();
            _authService.RtcEngine.DisableVideoObserver();
            _authService.RtcEngine.StopScreenCapture(); 

            VideoSurface.SetEnable(false);
        } 

        private uint getDisplayId(int k)
        {
    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            List<uint> ids = AgoraNativeBridge.GetMacDisplayIds();
    #else
            List<uint> ids = new List<uint>();
    #endif

            if (k < ids.Count)
            {
                return ids[k];
            }
            return 0;
        }

        private bool CheckErrors(){
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode", LogLevel);
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

    [Serializable]
    public struct VideoDimensionsStruct //pixels
    {
        [Tooltip("Default: 1920")]
        public int width; // 1920
        [Tooltip("Default: 1080")]
        public int height; // 1080
    };

    [Serializable]
    public struct ScreenCaptureParametersStruct
    {
        [Tooltip("Default: 1920x1080")]
        public VideoDimensionsStruct dimensions; // 1920x1080
        [Tooltip("Default: 15")]
        public int frameRate; // 15
        [Tooltip("Default: 0")]
        public int bitrate; // 0
        [Tooltip("Default: true")]
        public bool captureMouseCursor; // true

        public ScreenCaptureParameters GetScreenCaptureParams() {
            return new ScreenCaptureParameters{
                dimensions = new VideoDimensions{ width = dimensions.width, height = dimensions.height },
                frameRate = frameRate,
                bitrate = bitrate,
                captureMouseCursor = captureMouseCursor
            };
        }
    };
}
#endif
#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;
using System.Collections.Generic;

namespace d4160.Agora
{
    public class AgoraScreenShareService
    {
        public static event Action OnStartScreenCaptureEvent;
        public static event Action OnStopScreenCaptureEvent;

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance; 
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

        /// <summary>
        /// Share your screen accordding to the platform you are current in... need to EnableLocalVideo first
        /// </summary>
        /// <param name="sparams"></param>
        public void StartScreenCapture(ScreenCaptureParametersStruct sparams)
        {
            if (CheckErrors()) return;

            if (!VideoSurface.gameObject.activeSelf) VideoSurface.gameObject.SetActive(true);
            VideoSurface.SetForUser(0); // 0 means yourself
            VideoSurface.SetEnable(true);

            // _connection.RtcEngine.EnableVideo();
            // _connection.RtcEngine.EnableVideoObserver();
            _connection.RtcEngine.StopScreenCapture();

    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            _connection.RtcEngine.StartScreenCaptureByDisplayId(getDisplayId(DisplayID), default(Rectangle), sparams.GetScreenCaptureParams());  // 
    #else
            StartScreenCaptureByScreenRect(DisplayID, sparams.GetScreenCaptureParams());
    #endif
            DisplayID = 1 - DisplayID; // only 0 and 1

            OnStartScreenCaptureEvent?.Invoke();
        }

        private void StartScreenCaptureByScreenRect(int order, ScreenCaptureParameters sparams)
        {
            // Assuming you have two display monitors, each of 1920x1080, position left to right:
            Rectangle screenRect = new Rectangle() { x = 0, y = 0, width = 1920 * 2, height = 1080 };
            Rectangle regionRect = new Rectangle() { x = order * 1920, y = 0, width = 1920, height = 1080 };

            int rc = _connection.RtcEngine.StartScreenCaptureByScreenRect(screenRect,
                regionRect,
                sparams
                );
            if (rc != 0) Debug.LogWarning("rc = " + rc);
        }

        public void StopScreenCapture(){
            if (CheckErrors()) return;

            // _connection.RtcEngine.DisableVideo();
            // _connection.RtcEngine.DisableVideoObserver();
            _connection.RtcEngine.StopScreenCapture(); 

            VideoSurface.SetEnable(false);

            OnStopScreenCaptureEvent?.Invoke();
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
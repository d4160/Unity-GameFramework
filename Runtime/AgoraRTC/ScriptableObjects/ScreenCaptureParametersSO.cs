using Agora.Rtc;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/ScreenCaptureParameters")]
    public class ScreenCaptureParametersSO : ScriptableObject
    {
        public Vector2Int dimensions = new (1920, 1080);
        public int frameRate = 30;
        public BITRATE bitrate = (int) BITRATE.STANDARD_BITRATE;
        public bool captureMouseCursor = true;
        public bool windowFocus = false;
        public ulong[] excludeWindowList = new ulong[0];
        public int excludeWindowCount = 0;
        public int highLightWidth = 0;
        public uint highLightColor = 0;
        public bool enableHighLight = false;

        public ScreenCaptureParameters GetScreenCaptureParameters()
        {
            return new ScreenCaptureParameters(dimensions.x, dimensions.y, frameRate, (int)bitrate, captureMouseCursor, windowFocus, excludeWindowList, excludeWindowCount) { 
                highLightWidth = highLightWidth,
                highLightColor = highLightColor,
                enableHighLight = enableHighLight
            };
        }
    }
}
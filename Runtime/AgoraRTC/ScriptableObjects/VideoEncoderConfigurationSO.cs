using Agora.Rtc;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/VideoEncoderConfiguration")]
    public class VideoEncoderConfigurationSO : ScriptableObject
    {
        [SerializeField] private VIDEO_CODEC_TYPE _codecType = VIDEO_CODEC_TYPE.VIDEO_CODEC_H264;
        [SerializeField] private int _dimensionsWidth = 640;
        [SerializeField] private int _dimensionsHeight = 360;
        [SerializeField] private int _frameRate = (int)FRAME_RATE.FRAME_RATE_FPS_15;
        [SerializeField] private int _bitrate = (int)BITRATE.STANDARD_BITRATE;
        [SerializeField] private int _minBitrate = (int)BITRATE.DEFAULT_MIN_BITRATE;
        [SerializeField] private ORIENTATION_MODE _orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_ADAPTIVE;
        [SerializeField] private DEGRADATION_PREFERENCE _degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY;
        [SerializeField] private VIDEO_MIRROR_MODE_TYPE _mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED;

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

        public int DimensionsWidth => _dimensionsWidth;
        public int DimensionsHeight => _dimensionsHeight;

        public VideoEncoderConfiguration AgoraRtcVideoEncoderConfiguration => new() { 
            codecType = _codecType,
            dimensions = new VideoDimensions(_dimensionsWidth, _dimensionsHeight),
            frameRate = _frameRate,
            bitrate = _bitrate,
            minBitrate = _minBitrate,
            orientationMode = _orientationMode,
            degradationPreference = _degradationPreference,
            mirrorMode = _mirrorMode
        };

        public void SetVideoEncoderConfiguration()
        {
            _service.SetVideoEncoderConfiguration(AgoraRtcVideoEncoderConfiguration);
        }
    }
}
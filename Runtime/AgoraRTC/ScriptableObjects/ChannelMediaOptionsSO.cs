using Agora.Rtc;
using UnityEngine;

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/ChannelMediaOptions")]
    public class ChannelMediaOptionsSO : ScriptableObject
    {
        public bool publishCameraTrack = true;
        public bool publishSecondaryCameraTrack = false;
        public bool publishMicrophoneTrack = true;
        public bool publishScreenTrack = false;
        public bool publishSecondaryScreenTrack = false;

        private ChannelMediaOptions _mediaOptions;

        private readonly AgoraRtcService _service = AgoraRtcService.Instance;

        public ChannelMediaOptions GetChannelMediaOptions(bool forceMakeNew = false)
        {
            if (_mediaOptions == null || forceMakeNew)
            {
                var options = new ChannelMediaOptions();
                options.publishCameraTrack.SetValue(publishCameraTrack);
                options.publishSecondaryCameraTrack.SetValue(publishSecondaryCameraTrack);
                options.publishMicrophoneTrack.SetValue(publishMicrophoneTrack);
                options.publishScreenTrack.SetValue(publishScreenTrack);
                options.publishSecondaryScreenTrack.SetValue(publishSecondaryScreenTrack);

                _mediaOptions = options;
            }

            return _mediaOptions;
        }

        public void UpdateChannelMediaOptions()
        {
            _service.RtcEngine.UpdateChannelMediaOptions(GetChannelMediaOptions());
        }
    }
}
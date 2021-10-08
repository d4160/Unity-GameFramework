#if AGORA
using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using d4160.MonoBehaviourData;

namespace d4160.Chat.Agora
{
    public class AgoraVideoBehaviour : MonoBehaviourUnityData<AgoraVideoSO>
    {
        [Header("EVENTS")]
        [SerializeField] private UltEvent<uint, int, int, int> _onVideoSizeChanged;

        void OnEnable () {
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnVideoSizeChangedEvent += _onVideoSizeChanged.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnVideoSizeChangedEvent -= _onVideoSizeChanged.Invoke;
            }
        }   

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetEnableVideo() => _data?.SetEnableVideo();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void EnableLocalVideo() => _data?.EnableLocalVideo();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void MuteLocalVideoStream() => _data?.MuteLocalVideoStream();

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableVideo(bool enableVideo, bool enableVideoObserver, bool enableLocalVideo = true, bool muteLocalVideoStream = false) => _data?.SetEnableVideo(enableVideo, enableVideoObserver, enableLocalVideo, muteLocalVideoStream);

        public void EnableLocalVideo(bool enableLocalVideo) => _data?.EnableLocalVideo(enableLocalVideo);

        public void MuteLocalVideoStream(bool muteLocalVideoStream) => _data?.MuteLocalVideoStream(muteLocalVideoStream);

        public void MuteRemoteVideoStream(uint uid, bool mute) => _data?.MuteRemoteVideoStream(uid, mute);

        public void MuteAllRemoteVideoStreams(bool mute) => _data?.MuteAllRemoteVideoStreams(mute);
        
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StartVideoPreview() => _data?.StartVideoPreview();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void StopVideoPreview() => _data?.StopVideoPreview();
    }
}
#endif
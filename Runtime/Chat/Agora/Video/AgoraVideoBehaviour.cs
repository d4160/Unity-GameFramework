#if AGORA
using UnityEngine;
using NaughtyAttributes;
using UltEvents;
using d4160.MonoBehaviourData;

namespace d4160.Chat.Agora
{
    public class AgoraVideoBehaviour : MonoBehaviourData<AgoraVideoSO>
    {
        [Header("EVENTS")]
        [SerializeField] private UltEvent<uint, int, int, int> _onVideoSizeChanged;

        void OnEnable () {
            if (_data)
            {
                _data.OnVideoSizeChangedEvent += _onVideoSizeChanged.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.OnVideoSizeChangedEvent -= _onVideoSizeChanged.Invoke;
            }
        }   

        [Button]
        public void SetEnableVideo() {
            if (_data)
                _data.SetEnableVideo();
        }

        /// <summary>
        ///   Enable/Disable video
        /// </summary>
        /// <param name="pauseVideo"></param>
        public void SetEnableVideo(bool enableVideo)
        {
            if (_data)
                _data.SetEnableVideo(enableVideo);
        }
        
        [Button]
        public void StartVideoPreview() {
            if (_data)
                _data.StartVideoPreview();
        }

        [Button]
        public void StopVideoPreview() {
            if (_data)
                _data.StopVideoPreview();
        }
    }
}
#endif
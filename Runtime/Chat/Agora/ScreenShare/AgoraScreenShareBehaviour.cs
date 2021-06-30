#if AGORA
using agora_gaming_rtc;
using UnityEngine;
using NaughtyAttributes;
using d4160.MonoBehaviourData;

namespace d4160.Chat.Agora
{
    public class AgoraScreenShareBehaviour : MonoBehaviourUnityData<AgoraScreenShareSO>
    {
        [SerializeField] VideoSurface _videoSurface;
        
        [Button]
        public void StartScreenCapture() {
            if (_data)
            {
                _data.VideoSurface = _videoSurface;
                _data.StartScreenCapture();
            }
        }

        public void StartScreenCapture(ScreenCaptureParametersStruct sparams)
        {
            if (_data)
            {
                _data.VideoSurface = _videoSurface;
                _data.StartScreenCapture(sparams);
            }
        }

        [Button]
        public void StopScreenCapture(){
            if (_data)
                _data.StopScreenCapture();
        } 
    }
}
#endif
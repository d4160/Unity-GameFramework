using d4160.MonoBehaviours;

namespace d4160.AgoraRtc
{
    public class AgoraRtcServiceMono : MonoBehaviourUnityData<AgoraRtcServiceSO>
    {
        private void Start()
        {
            if (_data)
            {
                _data.Setup();
                _data.InitRtcEngine();
            }
        }

        private void OnEnable()
        {
            if (_data) _data.RegisterEvents();
        }

        private void OnDisable()
        {
            if (_data) _data.UnregisterEvents();
        }

        private void OnDestroy()
        {
            if (_data) _data.DisposeRtcEngine();
        }
    }
}
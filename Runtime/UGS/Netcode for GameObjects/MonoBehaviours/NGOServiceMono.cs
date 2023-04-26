using UnityEngine;
using d4160.MonoBehaviours;
using d4160.Netcode;

namespace d4160.UGS.Authentication
{
    [DefaultExecutionOrder(-5)]
    public class NGOServiceMono : MonoBehaviourUnityData<NetworkManagerCallbacksSO>
    {
        private void OnEnable()
        {
            _data.RegisterEvents();
        }

        private void OnDisable()
        {
            _data.UnregisterEvents();
        }
    }
}

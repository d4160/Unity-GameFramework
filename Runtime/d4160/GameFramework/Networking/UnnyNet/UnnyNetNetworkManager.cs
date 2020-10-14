#if UNNYNET
using NaughtyAttributes;
using UnityEngine;

namespace d4160.GameFramework.Networking
{
    public class UnnyNetNetworkManager : MonoBehaviour
    {
        [Button]
        public void Connect()
        {
            UnnyNet.MainController.Init(new UnnyNet.Config
            {
                GameId = "b73c1131-c5c6-485e-af82-908f39bf02b2",
                PublicKey = "ZWY5NWFkOGItNTkxYy00",
                OnReadyCallback = responseData => { Debug.Log("UnnyNet Initialized: " + responseData.Success); },
                Environment = UnnyNet.Constants.Environment.Development,
                OpenAnimation = UnnyNet.UniWebViewTransitionEdge.Left,
                DefaultChannel = "general",
                OpenWithFade = true
            });
        }
    }
}
#endif
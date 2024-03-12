using System.Collections;
using UnityEngine;
using d4160.MonoBehaviours;
using Unity.Services.Core;
using Unity.Services.Authentication;
using NaughtyAttributes;

namespace d4160.UGS.Authentication
{
    public class AuthServiceMono : MonoBehaviourUnityData<AuthCallbacksSO>
    {
        [SerializeField] private bool _logInAnonymouslyInStart;

        private IEnumerator Start()
        {
            while (UnityServices.State != ServicesInitializationState.Initialized)
            {
                yield return null;
            }

            _data.RegisterEvents();

            if (_logInAnonymouslyInStart)
            {
                SignInAnonymously();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public async void SignInAnonymously()
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}

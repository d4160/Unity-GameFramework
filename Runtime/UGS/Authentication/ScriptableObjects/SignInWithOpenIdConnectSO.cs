using UnityEngine;
using d4160.Variables;
using Unity.Services.Authentication;
using System.Threading.Tasks;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.Authentication
{
    [CreateAssetMenu(menuName = "d4160/UGS/Authentication/SignInWithOpenIdConnect")]
    public class SignInWithOpenIdConnectSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _providerName;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _idToken;

        public StringVariableSO IdToken => _idToken;

        public async Task SignInWithOpenIdConnectAsync()
        {
            if (Unity.Services.Core.UnityServices.State == Unity.Services.Core.ServicesInitializationState.Initialized)
            {
                try 
                {
                    AuthenticationService.Instance.SignOut(true);
                    AuthenticationService.Instance.ClearSessionToken();
                    Debug.Log("[SignInWithOpenIdConnectSO] Purged previous UGS session successfully.");
                }
                catch (System.Exception e) { Debug.LogWarning($"[SignInWithOpenIdConnectSO] SignOut warning: {e.Message}"); }
            }

            string provider = _providerName != null ? _providerName.Value : "null";
            string token = _idToken != null ? _idToken.Value : "null";
            
            Debug.Log($"[SignInWithOpenIdConnectSO] providerName: {provider}, idToken length: {token?.Length}");

            await AuthenticationService.Instance.SignInWithOpenIdConnectAsync(
                provider, token);
        }
    }
}
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
            await AuthenticationService.Instance.SignInWithOpenIdConnectAsync(
                _providerName, _idToken);
        }
    }
}
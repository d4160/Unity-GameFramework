using UnityEngine;
using d4160.Variables;
using Unity.Services.Authentication;
using System.Threading.Tasks;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.Authentication
{
    [CreateAssetMenu(menuName = "d4160/UGS/Authentication/LinkWithOpenIdConnect")]
    public class LinkWithOpenIdConnectSO : ScriptableObject
    {
        public bool forceLink = false;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _providerName;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _idToken;

        public async Task LinkWithOpenIdConnectAsync()
        {
            await AuthenticationService.Instance.LinkWithOpenIdConnectAsync(
                _providerName, _idToken, new LinkOptions() { ForceLink = forceLink });
        }
    }
}
using Amazon.CognitoIdentityProvider.Model;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace d4160.AWS.Cognito
{
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/ResendConfirmationCode")]
    public class ResendConfirmationCodeRequestSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _clientId;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _usernameVar;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private ResendConfirmationCodeRequest _request;

        public StringVariableSO UsernameVar => _usernameVar;

        public ResendConfirmationCodeRequest GetRequest(bool forceCreateNew = false)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    ClientId = _clientId,
                    Username = _usernameVar,
                };
            }

            return _request;
        }

        public async Task<ResendConfirmationCodeResponse> ResendConfirmationCodeAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false)
        {
            return await _client.GetClient(forceCreateNewClient).ResendConfirmationCodeAsync(GetRequest(forceCreateNewRequest));
        }
    }
}
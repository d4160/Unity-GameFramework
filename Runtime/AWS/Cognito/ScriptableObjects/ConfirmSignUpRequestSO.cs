using Amazon.CognitoIdentityProvider.Model;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace d4160.AWS.Cognito
{
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/ConfirmSignUp")]
    public class ConfirmSignUpRequestSO : ScriptableObject
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
        [SerializeField] private StringVariableSO _confirmationCodeVar;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private ConfirmSignUpRequest _request;

        public StringVariableSO UsernameVar => _usernameVar;
        public StringVariableSO ConfirmationCodeVar => _confirmationCodeVar;

        public ConfirmSignUpRequest GetRequest(bool forceCreateNew = false)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    ClientId = _clientId,
                    Username = _usernameVar,
                    ConfirmationCode = _confirmationCodeVar
                };
            }

            return _request;
        }

        public async Task<ConfirmSignUpResponse> ConfirmSignUpAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false)
        {
            return await _client.GetClient(forceCreateNewClient).ConfirmSignUpAsync(GetRequest(forceCreateNewRequest));
        }
    }
}
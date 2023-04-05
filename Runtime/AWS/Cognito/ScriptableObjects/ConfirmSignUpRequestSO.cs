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

        public StringVariableSO Username => _usernameVar;
        public StringVariableSO ConfirmationCode => _confirmationCodeVar;

        public ConfirmSignUpRequest GetRequest(bool forceCreateNew = false, bool updateAttributes = true)
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
            else if (updateAttributes)
            {
                _request.Username = _usernameVar;
                _request.ConfirmationCode = _confirmationCodeVar;
            }

            return _request;
        }

        public async Task<ConfirmSignUpResponse> ConfirmSignUpAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool updateAttributes = true)
        {
            return await _client.GetClient(forceCreateNewClient).ConfirmSignUpAsync(GetRequest(forceCreateNewRequest, updateAttributes));
        }
    }
}
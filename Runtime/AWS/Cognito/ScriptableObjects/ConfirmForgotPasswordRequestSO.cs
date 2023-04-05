using Amazon.CognitoIdentityProvider.Model;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace d4160.AWS.Cognito
{
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/ConfirmForgotPassword")]
    public class ConfirmForgotPasswordRequestSO : ScriptableObject
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
        [SerializeField] private StringVariableSO _passwordVar;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private ConfirmForgotPasswordRequest _request;

        public StringVariableSO Username => _usernameVar;
        public StringVariableSO Password => _passwordVar;
        public StringVariableSO ConfirmationCode => _confirmationCodeVar;

        public ConfirmForgotPasswordRequest GetRequest(bool forceCreateNew = false, bool updateAttributes = true)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    ClientId = _clientId,
                    Username = _usernameVar,
                    ConfirmationCode = _confirmationCodeVar,
                    Password = _passwordVar
                };
            }
            else if (updateAttributes)
            {
                _request.Username = _usernameVar;
                _request.ConfirmationCode = _confirmationCodeVar;
                _request.Password = _passwordVar;
            }

            return _request;
        }

        public async Task<ConfirmForgotPasswordResponse> ConfirmForgotPasswordAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool updateAttributes = true)
        {
            return await _client.GetClient(forceCreateNewClient).ConfirmForgotPasswordAsync(GetRequest(forceCreateNewRequest, updateAttributes));
        }
    }
}
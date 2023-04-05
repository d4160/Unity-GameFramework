using Amazon.CognitoIdentityProvider.Model;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace d4160.AWS.Cognito
{
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/ForgotPassword")]
    public class ForgotPasswordRequestSO : ScriptableObject
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

        private ForgotPasswordRequest _request;

        public StringVariableSO Username => _usernameVar;

        public ForgotPasswordRequest GetRequest(bool forceCreateNew = false, bool updateAttributes = true)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    ClientId = _clientId,
                    Username = _usernameVar
                };
            }
            else if (updateAttributes)
            {
                _request.Username = _usernameVar;
            }

            return _request;
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool updateAttributes = true)
        {
            return await _client.GetClient(forceCreateNewClient).ForgotPasswordAsync(GetRequest(forceCreateNewRequest, updateAttributes));
        }
    }
}
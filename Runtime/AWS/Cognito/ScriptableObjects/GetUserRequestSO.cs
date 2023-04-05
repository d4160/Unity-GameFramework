using Amazon.CognitoIdentityProvider.Model;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace d4160.AWS.Cognito
{
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/GetUser")]
    public class GetUserRequestSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _accessToken;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private GetUserRequest _request;

        public StringVariableSO AccessToken => _accessToken;

        public GetUserRequest GetRequest(bool forceCreateNew = false, bool forceUpdateUserAttrs = true)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    AccessToken = _accessToken
                };
            }
            else if (forceUpdateUserAttrs)
            {
                _request.AccessToken = _accessToken;
            }

            return _request;
        }

        public async Task<GetUserResponse> GetUserAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool forceUpdateUserAttrs = true)
        {
            return await _client.GetClient(forceCreateNewClient).GetUserAsync(GetRequest(forceCreateNewRequest, forceUpdateUserAttrs));
        }
    }
}
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
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/UpdateUserAttributes")]
    public class UpdateUserAttributesRequestSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _accessToken;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private VariableSOBase[] _userAttributes;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private UpdateUserAttributesRequest _request;

        public VariableSOBase[] UserAttributes => _userAttributes;
        public StringVariableSO AccessToken => _accessToken;

        private List<AttributeType> GetUserAttributes()
        {
            var attributes = new List<AttributeType>();
            for (int i = 0; i < _userAttributes.Length; i++)
            {
                if (_userAttributes[i] is IDictionaryItem<string> dic)
                {
                    attributes.Add(new() { Name = dic.Key, Value = dic.InnerStringValue });
                }
            }
            return attributes;
        }

        public UpdateUserAttributesRequest GetRequest(bool forceCreateNew = false, bool forceUpdateUserAttrs = true)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    AccessToken = _accessToken,
                    UserAttributes = GetUserAttributes()
                };
            }
            else if (forceUpdateUserAttrs)
            {
                _request.AccessToken = _accessToken;
                _request.UserAttributes = GetUserAttributes();
            }

            return _request;
        }

        public async Task<UpdateUserAttributesResponse> UpdateUserAttributesAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool forceUpdateUserAttrs = true)
        {
            return await _client.GetClient(forceCreateNewClient).UpdateUserAttributesAsync(GetRequest(forceCreateNewRequest, forceUpdateUserAttrs));
        }
    }
}
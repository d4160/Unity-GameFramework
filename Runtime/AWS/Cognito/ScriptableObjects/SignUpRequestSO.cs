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
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/SignUp")]
    public class SignUpRequestSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _clientId;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringStringDictionaryItemSO[] _attributes;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _usernameVar;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _passwordVar;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private SignUpRequest _request;

        public StringStringDictionaryItemSO[] UserAttributes => _attributes;
        public StringVariableSO Username => _usernameVar;
        public StringVariableSO Password => _passwordVar;

        private List<AttributeType> GetUserAttributes()
        {
            var attributes = new List<AttributeType>();
            for (int i = 0; i < _attributes.Length; i++)
            {
                attributes.Add(new() { Name = _attributes[i].Key, Value = _attributes[i].Value });
            }
            return attributes;
        }

        public SignUpRequest GetRequest(bool forceCreateNew = false, bool forceUpdateUserAttrs = true)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    ClientId = _clientId,
                    UserAttributes = GetUserAttributes(),
                    Username = _usernameVar,
                    Password = _passwordVar
                };
            }
            else if (forceUpdateUserAttrs)
            {
                _request.Username = _usernameVar;
                _request.Password = _passwordVar;
                _request.UserAttributes = GetUserAttributes();
            }

            return _request;
        }

        public async Task<SignUpResponse> SignUpAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool forceUpdateUserAttrs = true)
        {
            return await _client.GetClient(forceCreateNewClient).SignUpAsync(GetRequest(forceCreateNewRequest, forceUpdateUserAttrs));
        }
    }
}
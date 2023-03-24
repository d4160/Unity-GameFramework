using Amazon.CognitoIdentityProvider;
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
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/Requests/InitiateAuth")]
    public class InitiateAuthRequestSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _clientId;
        [SerializeField] private AuthFlowType _authFlow = AuthFlowType.USER_PASSWORD_AUTH;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringStringDictionaryItemSO[] _authParameters;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private CognitoIdentityProviderClientSO _client;

        private InitiateAuthRequest _request;

        public StringStringDictionaryItemSO[] AuthParameters => _authParameters;

        private Dictionary<string, string> GetAuthParameters()
        {
            var authParameters = new Dictionary<string, string>();
            for (int i = 0; i < _authParameters.Length; i++)
            {
                authParameters.Add(_authParameters[i].Key, _authParameters[i].Value);
            }
            return authParameters;
        }

        public InitiateAuthRequest GetRequest(bool forceCreateNew = false, bool forceUpdateAuthParams = true)
        {
            if (_request == null || forceCreateNew)
            {
                _request = new()
                {
                    ClientId = _clientId,
                    AuthParameters = GetAuthParameters(),
                    AuthFlow = _authFlow.GetAuthFlowType()
                };
            }
            else if(forceUpdateAuthParams)
            {
                _request.AuthParameters = GetAuthParameters();
            }

            return _request;
        }

        public async Task<InitiateAuthResponse> InitiateAuthAsync(bool forceCreateNewClient = false, bool forceCreateNewRequest = false, bool forceUpdateAuthParams = true)
        {
            //try {
            return await _client.GetClient(forceCreateNewClient).InitiateAuthAsync(GetRequest(forceCreateNewRequest, forceUpdateAuthParams));
            //}
            //catch(AmazonCognitoIdentityProviderException ex)
            //{
            //    Debug.LogException(ex);
            //    return null;
            //}
        }
    }
}
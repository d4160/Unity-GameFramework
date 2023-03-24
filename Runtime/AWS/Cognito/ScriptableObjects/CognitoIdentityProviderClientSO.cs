using Amazon.CognitoIdentityProvider;
using UnityEngine;
using d4160.AWS.Core;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.AWS.Cognito
{
    [CreateAssetMenu(menuName = "d4160/AWS/Cognito/IdentityProviderClient")]
    public class CognitoIdentityProviderClientSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private AWSCredentialsSO _credentials;
        [SerializeField] private AWSRegionEndpoint _regionEndpoint = AWSRegionEndpoint.SAEast1;

        private AmazonCognitoIdentityProviderClient _client;

        public AmazonCognitoIdentityProviderClient GetClient(bool forceCreateNew = false, bool forceCreateNewCredentials = false)
        {
            if (_client == null || forceCreateNew)
            {
                _client = new AmazonCognitoIdentityProviderClient(_credentials.GetCredentials(forceCreateNewCredentials), _regionEndpoint.GetRegionEndpoint());
            }

            return _client;
        }
    }

    public static class CognitoExtensions
    {
        public static Amazon.CognitoIdentityProvider.AuthFlowType GetAuthFlowType(this AuthFlowType selected)
        {
            switch (selected)
            {
                case AuthFlowType.ADMIN_NO_SRP_AUTH:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.ADMIN_NO_SRP_AUTH;
                case AuthFlowType.ADMIN_USER_PASSWORD_AUTH:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.ADMIN_USER_PASSWORD_AUTH;
                case AuthFlowType.CUSTOM_AUTH:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.CUSTOM_AUTH;
                case AuthFlowType.REFRESH_TOKEN:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.REFRESH_TOKEN;
                case AuthFlowType.REFRESH_TOKEN_AUTH:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.REFRESH_TOKEN_AUTH;
                case AuthFlowType.USER_PASSWORD_AUTH:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.USER_PASSWORD_AUTH;
                case AuthFlowType.USER_SRP_AUTH:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.USER_SRP_AUTH;
                default:
                    return Amazon.CognitoIdentityProvider.AuthFlowType.USER_PASSWORD_AUTH;
            }
        }
    }

    public enum AuthFlowType
    {
        ADMIN_NO_SRP_AUTH,
        ADMIN_USER_PASSWORD_AUTH,
        CUSTOM_AUTH,
        REFRESH_TOKEN,
        REFRESH_TOKEN_AUTH,
        USER_PASSWORD_AUTH,
        USER_SRP_AUTH
    }
}

using System;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/TokenRequest")]
    public class TokenExchangeRequestSO : ScriptableObject
    {
        [SerializeField] private string[] _scopes;

        public TokenExchangeRequest GetRequest()
        {
            return new TokenExchangeRequest()
            {
                scopes = _scopes
            };
        }
    }

    [Serializable]
    public class TokenExchangeRequest
    {
        public string[] scopes;
    }

    public class TokenExchangeResponse
    {
        public string accessToken;
    }
}
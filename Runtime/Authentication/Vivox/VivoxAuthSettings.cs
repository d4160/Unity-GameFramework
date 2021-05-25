#if VIVOX
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Authentication
{
    [CreateAssetMenu(menuName = "d4160/Authentication/Vivox/Auth Settings")]
    public class VivoxAuthSettings : ScriptableObject
    {
        public Uri ServerUri
        {
            get => new Uri(_server);

            set => _server = value.ToString();
        }

        [SerializeField] private string _server = "https://GETFROMPORTAL.www.vivox.com/api2";
        [SerializeField] private string _domain = "GET VALUE FROM VIVOX DEVELOPER PORTAL";
        [SerializeField] private string _tokenIssuer = "GET VALUE FROM VIVOX DEVELOPER PORTAL";
        [SerializeField] private string _tokenKey = "GET VALUE FROM VIVOX DEVELOPER PORTAL";
        [SerializeField] private double _tokenExpirationSeconds = 90;

        public TimeSpan TokenExpiration => TimeSpan.FromSeconds(_tokenExpirationSeconds);

        public string Domain => _domain;

        public string TokenIssuer => _tokenIssuer;

        public string TokenKey => _tokenKey;

        public bool IsInvalid => _server == "https://GETFROMPORTAL.www.vivox.com/api2" ||
                                _domain == "GET VALUE FROM VIVOX DEVELOPER PORTAL" ||
                                _tokenKey == "GET VALUE FROM VIVOX DEVELOPER PORTAL" ||
                                _tokenIssuer == "GET VALUE FROM VIVOX DEVELOPER PORTAL";
    }
}
#endif
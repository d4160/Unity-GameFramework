using System;
using System.Text;
using UnityEngine;

namespace d4160.UGS.Core
{
    [CreateAssetMenu(menuName = "d4160/UGS/Core/Data/ServiceAccount")]
    public class ServiceAccountSO : ScriptableObject
    {
        [SerializeField] private string keyId;
        [SerializeField] private string keySecret;

        public string KeyBase64
        {
            get
            {
                byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
                string keyBase64 = Convert.ToBase64String(keyByteArray);

                return keyBase64;
            }
        }
    }
}

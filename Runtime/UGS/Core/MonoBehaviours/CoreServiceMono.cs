using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace d4160.UGS.Core
{
    public class CoreServiceMono : MonoBehaviour
    {
        public int id = 3;

        async void Awake()
        {
            try
            {
                var options = new InitializationOptions();
                options.SetProfile($"d4160_2031_{id}");
                await UnityServices.InitializeAsync(options);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
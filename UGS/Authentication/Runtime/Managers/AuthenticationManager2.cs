using d4160.Runtime.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace d4160.Runtime.UGS.Authentication
{
    public class AuthenticationManager2 : Singleton2<AuthenticationManager2>
    {
        public const string InvalidUsernameOrPassword = "Invalid username or password";
        public const string PasswordDoesNotMatchRequirements = "Password does not match requirements";
        public const string UsernameAlreadyExists = "username already exists";

        public event Action OnInvalidUsernameOrPassword;
        public event Action OnPasswordDoesNotMatchRequirements;
        public event Action OnUsernameAlreadyExists;
        public event Action OnSignedInFailed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnSubsystemsInit()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    _ = WaitUntilUnityServicesInitialized(() =>
                    {
                        try
                        {
                            if (AuthenticationService.Instance != null)
                                AuthenticationService.Instance.SignInFailed += OnSignedInFailedCallback;
                        }
                        catch (Exception) { }
                    });
                }
                else
                {
                    AuthenticationService.Instance.SignInFailed += OnSignedInFailedCallback;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AuthenticationManager2] UnityServices state check deferred: {ex.Message}");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance != null)
                {
                    AuthenticationService.Instance.SignInFailed -= OnSignedInFailedCallback;
                }
            }
            catch (Exception) { }
        }

        protected virtual void OnSignedInFailedCallback(RequestFailedException exception)
        {
            switch (exception.Message)
            {
                case UsernameAlreadyExists:
                    OnUsernameAlreadyExists?.Invoke();
                    break;
            }

            OnSignedInFailed?.Invoke();
        }

        protected async Task WaitUntilUnityServicesInitialized(Action callback)
        {
            while (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await Task.Yield();
            }

            callback?.Invoke();
        }

        public void InvokeRequestException(RequestFailedException ex)
        {
            if (ex.Message.Contains("."))
            {
                switch (ex.Message.Split('.')[0])
                {
                    case PasswordDoesNotMatchRequirements:
                        OnPasswordDoesNotMatchRequirements?.Invoke();
                        break;
                }
            }
            else
            {
                switch (ex.Message)
                {
                    case InvalidUsernameOrPassword:
                        OnInvalidUsernameOrPassword?.Invoke();
                        break;
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Reflection;
using d4160.Core.MonoBehaviours;
using UnityEngine;
using UnityEngine.Promise;

namespace d4160.GameFramework.Authentication
{
    public class GameAuth : MonoBehaviour
    {
        enum AuthenticationStatus
        {
            NotAuthenticated,
            Authenticating,
            Authenticated,
            Failed
        }

        /// <summary>
        ///     Event raised when Game Auth is successfully authenticated.
        /// </summary>
        public static event Action authenticated;

        /// <summary>
        ///     Event raised when Game Auth failed its authenticated.
        ///     The provided exception is the reason of the failure.
        /// </summary>
        public static event Action<Exception> authenticatedFailed;

        /// <summary>
        ///     Event raised when Game Auth is unauthenticated.
        /// </summary>
        public static event Action unauthenticated;

        public static string currentVersion { get; private set; }

        static AuthenticationStatus _sAuthenticationStatus = AuthenticationStatus.NotAuthenticated;

        /// <summary>
        /// Check if the Game Auth is already authenticated.
        /// </summary>
        /// <returns>
        /// Whether the Game Auth is authenticated or not.
        /// </returns>
        public static bool IsAuthenticated => _sAuthenticationStatus == AuthenticationStatus.Authenticated;

        static PromiseGenerator s_PromiseGenerator;

        /// <summary>
        /// The current Authentication Access Layer used by GameAuth.
        /// </summary>
        internal static IAuthService authLayer { get; private set; }

        public static void Authenticate(IAuthService authLayer, 
            Action onCompleted = null,
            Action<Exception> onFailed = null)
        {
            if (_sAuthenticationStatus == AuthenticationStatus.Authenticating ||
                _sAuthenticationStatus == AuthenticationStatus.Authenticated)
            {
                const string message = "GameAuth is already authenticated and cannot be authenticated again.";
                Debug.LogWarning(message);
                onFailed?.Invoke(new Exception(message));

                return;
            }

            if(s_PromiseGenerator == null)
                s_PromiseGenerator = new PromiseGenerator();

            GameAuth.authLayer = authLayer;

            _sAuthenticationStatus = AuthenticationStatus.Authenticating;

            var routine = AuthenticateRoutine(onCompleted, onFailed);

#if !UNITY_EDITOR
            CoroutineManager.Instance.StartCoroutine(routine);
#else

            //Schedule initialization when playing
            if (Application.isPlaying)
            {
                CoroutineManager.Instance.StartCoroutine(routine);
            }

            //Force initialization in editor (can happen for editor tests)
            else
            {
                void PlayCoroutine(IEnumerator coroutine)
                {
                    bool hasNext;
                    do
                    {
                        if (coroutine.Current is IEnumerator subRoutine)
                        {
                            PlayCoroutine(subRoutine);
                        }

                        hasNext = coroutine.MoveNext();
                    } while (hasNext);
                }

                PlayCoroutine(routine);
            }
#endif
        }

        /// <summary>
        /// Routine to initialize Game Foundation asynchronously.
        /// </summary>
        /// <param name="onInitializeCompleted">
        /// Called if the initialization is a success.
        /// </param>
        /// <param name="onInitializeFailed">
        /// Called if the initialization is a failure.
        /// </param>
        static IEnumerator AuthenticateRoutine(Action onCompleted, Action<Exception> onFailed)
        {
            void FailInitialization(Exception reason)
            {
                Unauthenticate();

                Debug.LogWarning($"Game Auth can't be initialized: {reason}");

                _sAuthenticationStatus = AuthenticationStatus.Failed;

                onFailed?.Invoke(reason);

                //Raise event.
                authenticatedFailed?.Invoke(reason);
            }

            s_PromiseGenerator.GetPromiseHandles(out var dalInitDeferred, out var dalInitCompleter);

            authLayer.Authenticate(dalInitCompleter);

            if (!dalInitDeferred.isDone)
                yield return dalInitDeferred.Wait();

            var isFulfilled = dalInitDeferred.isFulfilled;
            var error = dalInitDeferred.error;
            dalInitDeferred.Release();

            if (!isFulfilled)
            {
                FailInitialization(error);

                yield break;
            }

            _sAuthenticationStatus = AuthenticationStatus.Authenticated;

            currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Debug.Log($"Successfully authenticated Game Auth version {currentVersion}");

            onCompleted?.Invoke();

            //Raise event.
            authenticated?.Invoke();
        }

        public static void Unauthenticate()
        {
            authLayer?.Unauthenticate();

            _sAuthenticationStatus = AuthenticationStatus.NotAuthenticated;

            currentVersion = null;
            s_PromiseGenerator = null;
            authLayer = null;

            //Raise event.
            unauthenticated?.Invoke();
        }
    }
}
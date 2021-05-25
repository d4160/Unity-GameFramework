using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Promise;
using d4160.Coroutines;

namespace d4160.Authentication
{
    public class GameAuthSdk : MonoBehaviour
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

        /// <summary>
        /// The current Authentication Access Layer used by GameAuth.
        /// </summary>
        internal static IAuthService authLayer { get; private set; }

        public static bool HasSession => IsAuthenticated && authLayer != null && authLayer.HasSession;
        public static string Id => authLayer?.Id;
        public static string SessionTicket => authLayer?.SessionTicket;
        public static string DisplayName => authLayer?.DisplayName;

        //static GameFoundationDebug k_GFLogger = GameFoundationDebug.Get(typeof(GameFoundationSdk));

        /// <summary>
        ///     Initializes some static values of the <see cref="GameAuthSdk"/>.
        /// </summary>
        static GameAuthSdk()
        {
            currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public static Deferred Authenticate(IAuthService authLayer)
        {
            Promises.GetHandles(out var deferred, out var completer);

            if (RejectIfArgNull(authLayer, nameof(authLayer), completer)) return deferred;


            if (_sAuthenticationStatus == AuthenticationStatus.Authenticating ||
                _sAuthenticationStatus == AuthenticationStatus.Authenticated)
            {
                const string message = "GameAuth is already authenticated and cannot be authenticated again.";
                Debug.LogWarning(message);
                //k_GFLogger.LogWarning(message);
                completer.Reject(new Exception(message));

                return deferred;
            }

#if UNITY_EDITOR

            // This might happen if Game Auth is initialized in editor while not running the game.
            // In this case, initializing Game Auth is disallowed.
            if (!Application.isPlaying)
            {
                const string message = nameof(GameAuthSdk) +
                                       " was attempted to be initialized while in Edit Mode, which is unsupported.";
                Debug.LogWarning(message);
                completer.Reject(new Exception(message));
                return deferred;
            }
#endif

            _sAuthenticationStatus = AuthenticationStatus.Authenticating;

            GameAuthSdk.authLayer = authLayer;

            var routine = AuthenticateRoutine(completer);

            // Works on both Editor and binary
            if (Application.isPlaying)
            {
                CoroutineHelper.Instance.StartCoroutine(routine);
            }

            //Force initialization in editor (can happen for editor tests)
            //void PlayCoroutine(IEnumerator coroutine)
            //{
            //    bool hasNext;
            //    do
            //    {
            //        if (coroutine.Current is IEnumerator subRoutine)
            //        {
            //            PlayCoroutine(subRoutine);
            //        }

            //        hasNext = coroutine.MoveNext();
            //    } while (hasNext);
            //}

            //PlayCoroutine(routine);

            return deferred;
        }

        static IEnumerator AuthenticateRoutine(Completer completer)
        {
            void FailInitialization(Completer aCompleter, Exception reason)
            {
                Unauthenticate();

                Debug.LogWarning($"{nameof(GameAuthSdk)} failed to authenticate: {reason}");

                _sAuthenticationStatus = AuthenticationStatus.Failed;

                aCompleter.Reject(reason);

                //Raise event.
                authenticatedFailed?.Invoke(reason);
            }

            Promises.GetHandles(out var dalInitDeferred, out var dalInitCompleter);

            try
            {
                authLayer.Authenticate(dalInitCompleter);
            }
            catch (Exception e)
            {
                dalInitCompleter.Reject(e);
            }

            if (!dalInitDeferred.isDone)
                yield return dalInitDeferred.Wait();

            var isFulfilled = dalInitDeferred.isFulfilled;
            var error = dalInitDeferred.error;
            dalInitDeferred.Release();

            if (!isFulfilled)
            {
                FailInitialization(completer, error);

                yield break;
            }

            _sAuthenticationStatus = AuthenticationStatus.Authenticated;

            Debug.Log($"Successfully authenticated Game Auth version {currentVersion}");

            completer.Resolve();

            //Raise event.
            authenticated?.Invoke();
        }

        public static void Unauthenticate()
        {
            authLayer?.Unauthenticate();

            _sAuthenticationStatus = AuthenticationStatus.NotAuthenticated;

            authLayer = null;

            //Raise event.
            unauthenticated?.Invoke();
        }

        public static bool RejectIfArgNull(object value, string name, Rejectable rejectable)
        {
            if (value is null)
            {
                rejectable.Reject(new ArgumentNullException(name));
                return true;
            }

            return false;
        }
    }
}
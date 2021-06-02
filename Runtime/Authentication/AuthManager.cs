using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Promise;
using d4160.Coroutines;
using d4160.Core;
using Logger = d4160.Logging.Logger;

namespace d4160.Authentication
{
    public static class AuthManager
    {
        enum AuthenticationStatus
        {
            NotAuthenticated, // LaggedOut
            SigningUp,
            SignedUp,
            RegisterFailed,
            LoggingIn,
            LoggedIn,
            LoginFailed,
            LoggingOut,
            LogoutFailed
        }

        /// <summary>
        ///     Event raised when Game Auth is successfully authenticated.
        /// </summary>
        public static event Action OnLoggedOn;
        public static event Action OnSignedUp;
        public static event Action OnLoggedOut;

        /// <summary>
        ///     Event raised when AuthManager failed its authenticated.
        ///     The provided exception is the reason of the failure.
        /// </summary>
        public static event Action<Exception> OnLoginFailed;
        public static event Action<Exception> OnRegisterFailed;
        public static event Action<Exception> OnLogoutFailed;

        public static string CurrentVersion { get; private set; }
        public static LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        static AuthenticationStatus _authenticationStatus = AuthenticationStatus.NotAuthenticated;

        /// <summary>
        /// Check if the Game Auth is already authenticated.
        /// </summary>
        /// <returns>
        /// Whether the Game Auth is authenticated or not.
        /// </returns>
        public static bool IsLoggedOn => _authenticationStatus == AuthenticationStatus.LoggedIn;
        public static bool IsSignedUp => _authenticationStatus == AuthenticationStatus.SignedUp;
        public static bool IsNotAuthenticated => _authenticationStatus == AuthenticationStatus.NotAuthenticated;

        /// <summary>
        /// The current Authentication Access Layer used by GameAuth.
        /// </summary>
        internal static IAuthService AuthLayer { get; private set; }

        public static bool HasSession => IsLoggedOn && AuthLayer != null && AuthLayer.HasSession;
        public static string Id => AuthLayer?.Id;
        public static string SessionTicket => AuthLayer?.SessionTicket;
        public static string DisplayName => AuthLayer?.DisplayName;

        //static GameFoundationDebug k_GFLogger = GameFoundationDebug.Get(typeof(GameFoundationSdk));

        /// <summary>
        ///     Initializes some static values of the <see cref="AuthManager"/>.
        /// </summary>
        static AuthManager()
        {
            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        static void StartCoroutine(IEnumerator routine) {
            // Works on both Editor and playing
            if (Application.isPlaying)
            {
                routine.StartCoroutine();
            }
            else
            {
#if UNITY_EDITOR
                // Force initialization in editor (can happen for editor tests)
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
#endif
            }
        }

        static void FailInitialization(Completer aCompleter, Exception reason, string message, LogLevelType logLevelToCheck, AuthenticationStatus newStatus)
        {
            Clear();

            Logger.Log(message, LogLevel, logLevelToCheck);

            _authenticationStatus = newStatus;

            aCompleter.Reject(reason);

            switch(newStatus) {
                case AuthenticationStatus.LoginFailed:
                    OnLoginFailed?.Invoke(reason);
                    break;
                case AuthenticationStatus.RegisterFailed:
                    OnRegisterFailed?.Invoke(reason);
                    break;
                case AuthenticationStatus.LogoutFailed:
                    OnLogoutFailed?.Invoke(reason);
                    break;
            }
        }

        public static void Login(IAuthService authService) {

            if(IsLoggedOn){
                Logger.LogWarning("You are already logged on.", LogLevel);
                return;
            }

            LoginRoutine(authService).StartCoroutine();
        }

        private static IEnumerator LoginRoutine(IAuthService authService) {

            using (var loginDeferred = LoginInternal(authService))
            {
                yield return loginDeferred.Wait();
            }
        }

        public static Deferred LoginInternal(IAuthService authService)
        {
            Promises.GetHandles(out var deferred, out var completer);

            if (RejectIfArgNull(authService, nameof(authService), completer)) return deferred;

            if (_authenticationStatus == AuthenticationStatus.LoggingIn ||
                _authenticationStatus == AuthenticationStatus.LoggedIn)
            {
                string message = "AuthManager is already logged on and cannot be logged on again.";
                Logger.LogWarning(message, LogLevel);

                completer.Reject(new Exception(message));

                return deferred;
            }

            _authenticationStatus = AuthenticationStatus.LoggingIn;

            AuthManager.AuthLayer = authService;

            var routine = LoginRoutine(completer);

            StartCoroutine(routine);

            return deferred;
        }

        public static void Register(IAuthService authService) {

            if(IsSignedUp){
                Logger.LogWarning("You are already signed up.", LogLevel);
                return;
            }

            RegisterRoutine(authService).StartCoroutine();
        }

        private static IEnumerator RegisterRoutine(IAuthService authService) {

            using (var registerDeferred = RegisterInternal(authService))
            {
                yield return registerDeferred.Wait();
            }
        }

        public static Deferred RegisterInternal(IAuthService authLayer)
        {
            Promises.GetHandles(out var deferred, out var completer);

            if (RejectIfArgNull(authLayer, nameof(authLayer), completer)) return deferred;

            if (_authenticationStatus == AuthenticationStatus.SigningUp ||
                _authenticationStatus == AuthenticationStatus.SignedUp)
            {
                string message = "AuthManager is already signed up and cannot be signed up again.";
                Logger.Log(message, LogLevel, LogLevelType.Warning);

                completer.Reject(new Exception(message));

                return deferred;
            }

            _authenticationStatus = AuthenticationStatus.SigningUp;

            AuthManager.AuthLayer = authLayer;

            var routine = RegisterRoutine(completer);

            StartCoroutine(routine);

            return deferred;
        }

        public static void Logout() {

            if(IsSignedUp){
                Logger.LogWarning("You are not authenticated.", LogLevel);
                return;
            }

            LogoutRoutine().StartCoroutine();
        }

        private static IEnumerator LogoutRoutine() {

            using (var logoutDeferred = LogoutInternal())
            {
                yield return logoutDeferred.Wait();
            }
        }

        public static Deferred LogoutInternal()
        {
            Promises.GetHandles(out var deferred, out var completer);

            if (RejectIfArgNull(AuthLayer, nameof(AuthLayer), completer)) return deferred;

            if (_authenticationStatus == AuthenticationStatus.NotAuthenticated)
            {
                string message = "AuthManager is already not authenticated and cannot do anything.";
                Logger.Log(message, LogLevel, LogLevelType.Warning);

                completer.Reject(new Exception(message));

                return deferred;
            }

            _authenticationStatus = AuthenticationStatus.LoggingOut;

            var routine = LogoutRoutine(completer);
            StartCoroutine(routine);

            return deferred;
        }

        static IEnumerator LoginRoutine(Completer completer)
        {
            Promises.GetHandles(out var dalInitDeferred, out var dalInitCompleter);

            try
            {
                AuthLayer?.Login(dalInitCompleter);
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
                FailInitialization(completer, error, $"{nameof(AuthManager)} failed to login: {error}", LogLevelType.Warning, AuthenticationStatus.LoginFailed);

                yield break;
            }

            _authenticationStatus = AuthenticationStatus.LoggedIn;

            Logger.LogInfo($"Successfully logged on AuthManager version {CurrentVersion}", LogLevel);

            completer.Resolve();
            OnLoggedOn?.Invoke();
        }

        static IEnumerator RegisterRoutine(Completer completer)
        {
            Promises.GetHandles(out var dalInitDeferred, out var dalInitCompleter);

            try
            {
                AuthLayer?.Register(dalInitCompleter);
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
                FailInitialization(completer, error, $"{nameof(AuthManager)} failed to register: {error}", LogLevelType.Warning, AuthenticationStatus.RegisterFailed);

                yield break;
            }

            _authenticationStatus = AuthenticationStatus.SignedUp;

            Logger.LogInfo($"Successfully registered AuthManager version {CurrentVersion}", LogLevel);

            completer.Resolve();
            OnLoggedOn?.Invoke();
        }

        static IEnumerator LogoutRoutine(Completer completer)
        {
            Promises.GetHandles(out var dalInitDeferred, out var dalInitCompleter);

            try
            {
                AuthLayer?.Logout(dalInitCompleter);
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
                FailInitialization(completer, error, $"{nameof(AuthManager)} failed to logout: {error}", LogLevelType.Warning, AuthenticationStatus.LogoutFailed);

                yield break;
            }

            _authenticationStatus = AuthenticationStatus.NotAuthenticated;

            Logger.LogInfo($"Successfully logged out AuthManager version {CurrentVersion}", LogLevel);

            completer.Resolve();
            OnLoggedOut?.Invoke();
        }

        private static void Clear(){
            _authenticationStatus = AuthenticationStatus.NotAuthenticated;
            AuthLayer = null;
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
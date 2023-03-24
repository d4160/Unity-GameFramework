using Unity.Services.Authentication;
using UnityEngine;
using d4160.Logging;
using d4160.Events;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.Authentication
{
    [CreateAssetMenu(menuName = "d4160/UGS/Authentication/AuthCallbacks")]
    public class AuthCallbacksSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] LoggerSO _logger;

        [Header("Events")]
        [SerializeField] private VoidEventSO _onSignedIn;
        [SerializeField] private RequestFailedEventSO _onSignInFailed;
        [SerializeField] private VoidEventSO _onSignedOut;
        [SerializeField] private VoidEventSO _onExpired;

        public void RegisterEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                if (_onSignedIn)
                {
                    _onSignedIn.Invoke();
                }

                if (_logger) _logger.LogInfo($"[SignedIn] PlayerId: {AuthenticationService.Instance.PlayerId};"); // AccessToken: {AuthenticationService.Instance.AccessToken};
            };

            AuthenticationService.Instance.SignInFailed += (err) =>
            {
                if (_onSignInFailed)
                {
                    _onSignInFailed.Invoke(err);
                }

                if (_logger) _logger.LogError($"[SignInFailed] Error: {err}");
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                if (_onSignedOut)
                {
                    _onSignedOut.Invoke();
                }

                if (_logger) _logger.LogInfo($"[SignedOut] Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                if (_onExpired)
                {
                    _onExpired.Invoke();
                }

                if (_logger) _logger.LogInfo($"[Expired] Player session could not be refreshed and expired.");
            };
        }
    }
}
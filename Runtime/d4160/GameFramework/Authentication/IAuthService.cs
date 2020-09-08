using UnityEngine.GameFoundation.DataAccessLayers;
using UnityEngine.Promise;

namespace d4160.GameFramework.Authentication
{
    /// <summary>
    /// Contract for authentication
    /// systems (PlayFab, ...).
    /// </summary>
    public interface IAuthService
    {
        string DisplayName { get; }
        string Id { get; }

        /// <summary>
        /// Authenticate this auth layer.
        /// </summary>
        /// <param name="completer">
        /// When done, this completer is resolved or rejected.
        /// </param>
        void Authenticate(Completer completer);

        void Unauthenticate();
    }
}
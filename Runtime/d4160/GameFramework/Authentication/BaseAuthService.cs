using UnityEngine.Promise;

namespace d4160.GameFramework.Authentication
{
    /// <summary>
    /// Base for auth access layer.
    /// </summary>
    public abstract class BaseAuthService : IAuthService
    {
        public string DisplayName { get; protected set; }
        public string Id { get; protected set; }

        /// <inheritdoc />
        public abstract void Authenticate(Completer completer);

        public abstract void Unauthenticate();
    }
}
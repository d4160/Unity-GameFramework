using UnityEngine.Promise;

namespace d4160.Authentication
{
    /// <summary>
    /// Base for auth access layer.
    /// </summary>
    public abstract class BaseAuthService : IAuthService
    {
        public virtual string DisplayName { get; protected set; }
        public virtual string Id { get; protected set; }
        public virtual bool HasSession { get; protected set; }
        public virtual string SessionTicket { get; protected set; }

        /// <inheritdoc />
        public abstract void Authenticate(Completer completer);

        public abstract void Unauthenticate();
    }
}
using UnityEngine.Promise;

namespace d4160.UGS.Authentication
{
    /// <summary>
    /// Base for auth access layer.
    /// </summary>
    public abstract class BaseAuthService : IAuthService
    {
        public virtual string DisplayName { get; set; }
        public virtual string Id { get; set; }
        public virtual bool HasSession => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(SessionTicket);
        public virtual string SessionTicket { get; set; }

        /// <inheritdoc />
        public abstract void Login(Completer completer);

        public abstract void Register(Completer completer);

        public abstract void Logout(Completer completer);
    }
}
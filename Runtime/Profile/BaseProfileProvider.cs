using UnityEngine.Promise;

namespace d4160.Profile
{
    /// <summary>
    /// Base for auth access layer.
    /// </summary>
    public abstract class BaseProfileService : IProfileService
    {
        public virtual string DisplayName { get; protected set; }

        /// <inheritdoc />
        public abstract void GetPlayerProfile(Completer completer);
    }
}
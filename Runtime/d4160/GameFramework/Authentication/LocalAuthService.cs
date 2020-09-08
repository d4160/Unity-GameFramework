using UnityEngine;
using UnityEngine.Promise;

namespace d4160.GameFramework.Authentication
{
    public class LocalAuthService : BaseAuthService
    {
        public LocalAuthService(string displayName)
        {
            DisplayName = displayName;
        }

        public override void Authenticate(Completer completer)
        {
            Id = System.Guid.NewGuid().ToString();

            completer.Resolve();
        }

        public override void Unauthenticate()
        {
            Id = string.Empty;
        }
    }
}

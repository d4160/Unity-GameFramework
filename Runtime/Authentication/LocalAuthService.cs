using UnityEngine.Promise;

namespace d4160.Authentication
{
    public class LocalAuthService : BaseAuthService
    {
        public LocalAuthService(string displayName)
        {
            DisplayName = displayName;
        }

        public override void Login(Completer completer)
        {
            GenerateIdAndSessionTicket();
            completer.Resolve();
        }

        public override void Register(Completer completer)
        {
            GenerateIdAndSessionTicket();
            completer.Resolve();
        }

        private void GenerateIdAndSessionTicket() {
            Id = System.Guid.NewGuid().ToString();
            SessionTicket = System.Guid.NewGuid().ToString();
        }

        public override void Logout(Completer completer)
        {
            Id = string.Empty;
            SessionTicket = string.Empty;

            completer.Resolve();
        }
    }
}

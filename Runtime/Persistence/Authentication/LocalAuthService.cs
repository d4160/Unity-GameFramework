
namespace d4160.UGS.Authentication
{
    public class LocalAuthService : BaseAuthService
    {
        public LocalAuthService(string displayName)
        {
            DisplayName = displayName;
        }

        public override void Login()
        {
            GenerateIdAndSessionTicket();
            //completer.Resolve();
        }

        public override void Register()
        {
            GenerateIdAndSessionTicket();
            //completer.Resolve();
        }

        private void GenerateIdAndSessionTicket() {
            Id = System.Guid.NewGuid().ToString();
            SessionTicket = System.Guid.NewGuid().ToString();
        }

        public override void Logout()
        {
            Id = string.Empty;
            SessionTicket = string.Empty;

            //completer.Resolve();
        }
    }
}

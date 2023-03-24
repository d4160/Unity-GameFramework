using UnityEngine;
using Unity.Services.Authentication;

namespace d4160.UGS.Authentication
{
    [CreateAssetMenu(menuName = "d4160/UGS/Authentication/SignOut")]
    public class SignOutSO : ScriptableObject
    {
        public bool clearCredentials;

        public void SignOut()
        {
            AuthenticationService.Instance.SignOut(clearCredentials);
        }
    }
}
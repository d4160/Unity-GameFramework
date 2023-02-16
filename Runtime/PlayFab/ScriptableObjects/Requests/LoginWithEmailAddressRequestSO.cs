using UnityEngine;
using PlayFab.ClientModels;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Requests/LoginWithEmailAddress")]

    public class LoginWithEmailAddressRequestSO : LoginRequestBaseSO
    {
        public LoginWithEmailAddressRequest request;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public PlayerCombinedInfoParamsModelSO infoRequestParams;

        public override void Login()
        {
            request.InfoRequestParameters = infoRequestParams.requestParams;
            _service.LoginWithEmailAddress(request, 
                (r) => {
                    _resultpayload.SetResultPayload(r);
                    if (_loginEvent) _loginEvent.Invoke(r);
            }, (e) => {
                if (_errorEvent) _errorEvent.Invoke(e);
            });
        }
    }
}
using UnityEngine;
using d4160.Events;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab 
{
    public abstract class LoginRequestBaseSO : PlayFabSOBase
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] protected PlayerCombinedInfoPayloadModelSO _resultpayload;

        [Header("EVENTS")]
        [SerializeField] protected LoginResultEventSO _loginEvent;
        [SerializeField] protected VoidEventSO _logoutEvent;
        [SerializeField] protected PlayFabErrorEventSO _errorEvent;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public abstract void Login();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public virtual void Logout() 
        { 
            _service.Logout(() => {
                if (_logoutEvent) _logoutEvent.Invoke();
            });
        }
    }
}
using d4160.Variables;
using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Requests/UpdateUserTitleDisplayName")]
    public class UpdateUserTitleDisplayNameRequestSO : PlayFabRequestSOBase
    {
        public UpdateUserTitleDisplayNameResultEventSO resultEvent;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public StringVariableSO displayNameVar;

        public void UpdateUserTitleDisplayName() 
        {
            _service.UpdateUserTitleDisplayName(new() { DisplayName = displayNameVar }, (r) => {
                if (resultEvent) resultEvent.Invoke(r);
            }, (e) => {
                if (_errorEvent) _errorEvent.Invoke(e);
            });
        }
    }
}
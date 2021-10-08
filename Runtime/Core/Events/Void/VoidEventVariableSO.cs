using System.Collections;
using System.Collections.Generic;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Events
{
    [CreateAssetMenu(menuName = "d4160/Variables/Void Event")]
    public class VoidEventVariableSO : VariableSOBase<VoidEventSO>
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke() {
            _value?.Invoke();
        }
    }

    [System.Serializable]
    public class VoidEventReference : VariableReference<VoidEventVariableSO, VoidEventSO>
    {
    }
}

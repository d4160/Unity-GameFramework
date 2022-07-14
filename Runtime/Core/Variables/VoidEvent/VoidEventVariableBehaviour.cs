using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Events
{
    public class VoidEventVariableBehaviour : VariableBehaviourBase<VoidEventVariableSO, VoidEventSO>
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke() => _data?.Invoke();
    }
}
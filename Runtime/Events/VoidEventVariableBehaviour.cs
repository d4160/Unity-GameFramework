using d4160.Variables;
using NaughtyAttributes;

namespace d4160.Events
{
    public class VoidEventVariableBehaviour : VariableBehaviourBase<VoidEventVariableSO, VoidEventSO>
    {
        [Button]
        public void Invoke() => _data?.Invoke();
    }
}
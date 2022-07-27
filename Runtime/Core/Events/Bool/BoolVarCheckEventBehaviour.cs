namespace d4160.Variables
{
    using d4160.Events;
    using UltEvents;
    using UnityEngine;

    public class BoolVarCheckEventBehaviour : EmptyVarCheckEventBehaviourBase<BoolVariableSO, bool>
    {
        [SerializeField] private UltEvent _onTrueEvent;
        [SerializeField] private UltEvent _onFalseEvent;

        public override void Invoke()
        {
            if (_data.Value)
            {
                _onTrueEvent?.Invoke();
            }
            else
            {
                _onFalseEvent?.Invoke();
            }
        }
    }
}
namespace d4160.Variables
{
    using d4160.Events;
    using UltEvents;
    using UnityEngine;

    public class BoolSwitchEventBehaviour : EmptyEventBehaviourBase<BoolEventSO, bool>
    {
        [SerializeField] private UltEvent _onTrueEvent;
        [SerializeField] private UltEvent _onFalseEvent;

        protected override void OnInvokedInternal(bool param)
        {
            if (param)
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
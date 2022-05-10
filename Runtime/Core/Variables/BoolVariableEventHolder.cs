namespace d4160.Variables
{
    using UltEvents;
    using UnityEngine;

    public class BoolVariableEventHolder : VariableBehaviourBase<BoolVariableSO, bool>
    {
        [SerializeField]
        private UltEvent _onTrueEvent;

        /// <summary>The encapsulated event.</summary>
        public UltEvent OnTrueEvent
        {
            get
            {
                if (_onTrueEvent == null)
                    _onTrueEvent = new UltEvent();
                return _onTrueEvent;
            }
            set { _onTrueEvent = value; }
        }

        [SerializeField]
        private UltEvent _onFalseEvent;

        /// <summary>The encapsulated event.</summary>
        public UltEvent OnFalseEvent
        {
            get
            {
                if (_onFalseEvent == null)
                    _onFalseEvent = new UltEvent();
                return _onFalseEvent;
            }
            set { _onFalseEvent = value; }
        }

        [SerializeField]
        private UltEvent<bool> _onToggleEvent;

        /// <summary>The encapsulated event.</summary>
        public UltEvent<bool> OnToggleEvent
        {
            get
            {
                if (_onToggleEvent == null)
                    _onToggleEvent = new UltEvent<bool>();
                return _onToggleEvent;
            }
            set { _onToggleEvent = value; }
        }


        public virtual void Invoke()
        {
            var value = GetValue();
            // Debug.Log($"{value} from {name}");
            if(value)
            {
                OnTrueEvent?.Invoke();
            }
            else
            {
                OnFalseEvent?.Invoke();
            }

            _onToggleEvent?.Invoke(value);
        }
    }
}
using d4160.Events;
using UnityEngine;

namespace d4160.Variables
{
    public abstract class VariableSOBase<T> : VariableSOBase
    {
        //[SerializeField, Multiline] protected string _editorDescription;
        [SerializeField] protected T _value;
        [SerializeField] protected EventSOBase<T> _onChange;

        public override object RawValue { get => _value; set => Value = (T)value; }
        public EventSOBase<T> OnChange => _onChange;

        public virtual T Value
        {
            get => _value;
            set
            {
                bool invokeOnChange = _value != null ? !_value.Equals(value) : value != null;
                _value = value;
                if (_onChange && invokeOnChange) _onChange.Invoke(_value);
            }
        }

        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }

        public void SetValue(VariableSOBase<T> variable)
        {
            Value = variable.Value;
        }

        public void SetValue(T value)
        {
            Value = value;
        }

        public static implicit operator T(VariableSOBase<T> variable)
        {
            return variable.Value;
        }

        public override void ResetValue()
        {
            SetValueWithoutNotify(default);
        }
    }

    public abstract class VariableSOBase : ScriptableObject
    {
        public abstract object RawValue { get; set; }

        public abstract void ResetValue();
    }
}
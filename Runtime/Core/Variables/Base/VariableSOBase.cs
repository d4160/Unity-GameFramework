using d4160.Events;
using UnityEngine;

namespace d4160.Variables
{
    public abstract class VariableSOBase<T> : ScriptableObject
    {
        [SerializeField, Multiline] protected string _editorDescription;
        [SerializeField] protected T _value;
        [SerializeField] protected EventSOBase<T> _onChange;

        public T Value 
        { 
            get => _value;
            set
            {
                _value = value;
                _onChange?.Invoke(_value);
            }
        }

        public void SetValue(VariableSOBase<T> variable) {
            _value = variable.Value;

            _onChange?.Invoke(_value);
        }

        public static implicit operator T(VariableSOBase<T> variable)
        {
            return variable.Value;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Variables
{
    public abstract class VariableSOBase<T> : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, Multiline] protected string _editorDescription;
#endif
        [SerializeField] protected T _value;

        public T Value { get => _value; set => _value = value; }

        public void SetValue(VariableSOBase<T> variable) {
            _value = variable.Value;
        }

        public static implicit operator T(VariableSOBase<T> variable)
        {
            return variable.Value;
        }
    }
}
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
using System;
#endif
using UnityEngine;

namespace d4160.Variables
{
    [System.Serializable]
    public class VariableReference<TVarSO, T> where TVarSO : VariableSOBase<T>
    {
        [SerializeField] public bool _useConstant = true;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_useConstant")]
#endif
        [SerializeField] public T _constantValue;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [HideIf("_useConstant"), Expandable]
#endif
        [SerializeField] protected TVarSO _variable;

        public TVarSO Variable => _variable;
        public T Value
        {
            get => _useConstant ? _constantValue : _variable.Value;
            set { if (_useConstant) _constantValue = value; else _variable.Value = value; }
        }

        public VariableReference()
        {
        }

        public VariableReference(T value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public static implicit operator T(VariableReference<TVarSO, T> reference)
        {
            return reference.Value;
        }
    }

    [System.Serializable]
    public class VariableReferenceBase<TVarSO, T> where TVarSO : VariableSOBase
    {
        [SerializeField] public bool _useConstant = true;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_useConstant")]
#endif
        [SerializeField] public T _constantValue;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [HideIf("_useConstant"), Expandable]
#endif
        [SerializeField] protected TVarSO _variable;

        public TVarSO Variable => _variable;
        public T Value 
        { 
            get => _useConstant ? _constantValue : (Type.GetTypeCode(typeof(T)) == TypeCode.String ? (T)(object)_variable.StringValue : (T)_variable.RawValue);
            set { if (_useConstant) _constantValue = value; else _variable.RawValue = value; }
        }
        public string StringValue 
        { 
            get => _useConstant ? _constantValue.ToString() : _variable.StringValue;
            set { if (_useConstant) ParseConstantValue(value); else _variable.StringValue = value; }
        }

        public virtual void ParseConstantValue(string value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    _constantValue = (T)(object)value;
                    break;
                case TypeCode.Int32:
                    _constantValue = (T)(object)int.Parse(value);
                    break;
                case TypeCode.Boolean:
                    _constantValue = (T)(object)bool.Parse(value);
                    break;
                case TypeCode.Decimal:
                    _constantValue = (T)(object)float.Parse(value);
                    break;
            }
        }


        public VariableReferenceBase()
        {
        }

        public VariableReferenceBase(T value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public static implicit operator T(VariableReferenceBase<TVarSO, T> reference)
        {
            return reference.Value;
        }
    }

    public class StringReferenceBase : VariableReferenceBase<VariableSOBase, string> { }
}
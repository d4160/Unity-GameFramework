using d4160.Events;
using UnityEngine;
using UnityEngine.Serialization;
using Mono.CSharp;
using System;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Variables
{
    public abstract class VariableSOBase<T> : VariableSOBase
    {
        //[SerializeField, Multiline] protected string _editorDescription;
        [SerializeField] protected T _value;
        [SerializeField, FormerlySerializedAs("_onChange")] protected EventSOBase<T> _onValueChange;

        public override object RawValue { get => _value; set => Value = (T)value; }
        public EventSOBase<T> OnValueChange => _onValueChange;

        public virtual T Value
        {
            get => _value;
            set
            {
                bool invokeOnChange = _value != null ? !_value.Equals(value) : value != null;
                _value = value;
                if (_onValueChange && invokeOnChange) _onValueChange.Invoke(_value);
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

    public abstract class VariableSOBase<T1, T2> : VariableSOBase<T2>, IDictionaryItem<T1>
    {
        //[SerializeField, Multiline] protected string _editorDescription;
        [SerializeField] protected T1 _key;
        [SerializeField] protected EventSOBase<T1> _onKeyChange;

        public object RawKey { get => _key; set => Key = (T1)value; }
        public EventSOBase<T1> OnKeyChange => _onKeyChange;

        public virtual T1 Key
        {
            get => _key;
            set
            {
                bool invokeOnChange = _key != null ? !_key.Equals(value) : value != null;
                _key = value;
                if (_onKeyChange && invokeOnChange) _onKeyChange.Invoke(_key);
            }
        }

        public object InnerRawValue { get => Value; set => Value = (T2)value; }

        public void SetKeyWithoutNotify(T1 value)
        {
            _key = value;
        }

        public void SetKey(VariableSOBase<T1, T2> variable)
        {
            Key = variable.Key;
        }

        public void SetKey(T1 key)
        {
            Key = key;
        }

        public static implicit operator T1(VariableSOBase<T1, T2> variable)
        {
            return variable.Key;
        }

        public void ResetKey()
        {
            SetKeyWithoutNotify(default);
        }

        public void ParseInnerValue(string value)
        {
            switch (Type.GetTypeCode(typeof(T2)))
            {
                case TypeCode.String:
                    Value = (T2)(object)value;
                    break;
                case TypeCode.Int32:
                    Value = (T2)(object)int.Parse(value);
                    break;
                case TypeCode.Boolean:
                    Value = (T2)(object)bool.Parse(value);
                    break;
                case TypeCode.Decimal:
                    Value = (T2)(object)float.Parse(value);
                    break;
            }
        }
    }

    public abstract class InnerVariableSOBase<T1, T2> : VariableSOBase, IInnerVariable where T1 : VariableSOBase<T2>
    {
        //[SerializeField, Multiline] protected string _editorDescription;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] protected T1 _value;
        [SerializeField] protected EventSOBase<T1> _onValueChange;

        public override object RawValue { get => _value; set => Value = (T1)value; }
        public object InnerRawValue { get => InnerValue; set => InnerValue = (T2)value; }
        public EventSOBase<T1> OnValueChange => _onValueChange;

        public virtual T1 Value
        {
            get => _value;
            set
            {
                bool invokeOnChange = _value != null ? !_value.Equals(value) : value != null;
                _value = value;
                if (_onValueChange && invokeOnChange) _onValueChange.Invoke(_value);
            }
        }

        public virtual T2 InnerValue
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        public virtual void ParseInnerValue(string value)
        {
            switch (Type.GetTypeCode(typeof(T2)))
            {
                case TypeCode.String:
                    InnerValue = (T2)(object)value;
                    break;
                case TypeCode.Int32:
                    InnerValue = (T2)(object)int.Parse(value);
                    break;
                case TypeCode.Boolean:
                    InnerValue = (T2)(object)bool.Parse(value);
                    break;
                case TypeCode.Decimal:
                    InnerValue = (T2)(object)float.Parse(value);
                    break;
            }
        }

        public void SetValueWithoutNotify(T1 value)
        {
            _value = value;
        }

        public void SetValue(VariableSOBase<T1> variable)
        {
            Value = variable.Value;
        }

        public void SetValue(T1 value)
        {
            Value = value;
        }

        public void SetInnerValue(T2 value)
        {
            Value.Value = value;
        }

        public static implicit operator T1(InnerVariableSOBase<T1, T2> variable)
        {
            return variable.Value;
        }

        public static implicit operator T2(InnerVariableSOBase<T1, T2> variable)
        {
            return variable.Value;
        }

        public override void ResetValue()
        {
            SetValueWithoutNotify(default);
        }
    }

    public abstract class DictionaryItemSOBase<T1, T2, T3> : InnerVariableSOBase<T2, T3>, IDictionaryItem<T1> where T2 : VariableSOBase<T3>
    {
        //[SerializeField, Multiline] protected string _editorDescription;
        [SerializeField] protected T1 _key;
        [SerializeField] protected EventSOBase<T1> _onKeyChange;

        public object RawKey { get => _key; set => Key = (T1)value; }
        public EventSOBase<T1> OnKeyChange => _onKeyChange;

        public virtual T1 Key
        {
            get => _key;
            set
            {
                bool invokeOnChange = _key != null ? !_key.Equals(value) : value != null;
                _key = value;
                if (_onKeyChange && invokeOnChange) _onKeyChange.Invoke(_key);
            }
        }

        public void SetKeyWithoutNotify(T1 value)
        {
            _key = value;
        }

        public void SetKey(VariableSOBase<T1, T2> variable)
        {
            Key = variable.Key;
        }

        public void SetKey(T1 key)
        {
            Key = key;
        }

        public static implicit operator T1(DictionaryItemSOBase<T1, T2, T3> variable)
        {
            return variable.Key;
        }

        public void ResetKey()
        {
            SetKeyWithoutNotify(default);
        }
    }

    public interface IVariable
    {
        object RawValue { get; set; }
    }

    public interface IInnerVariable : IVariable
    { 
        object InnerRawValue { get; set; }

        void ParseInnerValue(string value);
    }

    public interface IDictionaryItem<T> : IInnerVariable
    {
        T Key { get; }
    }

    public abstract class VariableSOBase : ScriptableObject, IVariable
    {
        public abstract object RawValue { get; set; }

        public abstract void ResetValue();
    }
}
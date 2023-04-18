using d4160.Events;
using UnityEngine;
using UnityEngine.Serialization;
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
        public override string StringValue { get => Value.ToString(); set => ParseValue(value); }
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

        public void ParseValue(string value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    Value = (T)(object)value;
                    break;
                case TypeCode.Int32:
                    Value = (T)(object)int.Parse(value);
                    break;
                case TypeCode.UInt32:
                    Value = (T)(object)uint.Parse(value);
                    break;
                case TypeCode.Int64:
                    Value = (T)(object)long.Parse(value);
                    break;
                case TypeCode.UInt64:
                    Value = (T)(object)ulong.Parse(value);
                    break;
                case TypeCode.Boolean:
                    Value = (T)(object)bool.Parse(value);
                    break;
                case TypeCode.Decimal:
                    Value = (T)(object)float.Parse(value);
                    break;
                case TypeCode.Double:
                    Value = (T)(object)double.Parse(value);
                    break;
            }
        }
    }

    public abstract class VariableSOBase<T1, T2> : VariableSOBase<T2>, IDictionaryItem<T1, T2>
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
        public string InnerStringValue { get => Value.ToString(); set => ParseInnerValue(value); }

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
        [SerializeField, FormerlySerializedAs("_value")] protected T1 _valueVar;
        [SerializeField] protected EventSOBase<T1> _onValueChange;

        public override object RawValue { get => _valueVar; set => Value = (T1)value; }
        public override string StringValue { get => InnerStringValue; set => InnerStringValue = value; }
        public object InnerRawValue { get => InnerValue; set => InnerValue = (T2)value; }
        public string InnerStringValue { get => InnerValue.ToString(); set => ParseInnerValue(value); }
        public EventSOBase<T1> OnValueChange => _onValueChange;

        public virtual T1 Value
        {
            get => _valueVar;
            set
            {
                bool invokeOnChange = _valueVar != null ? !_valueVar.Equals(value) : value != null;
                _valueVar = value;
                if (_onValueChange && invokeOnChange) _onValueChange.Invoke(_valueVar);
            }
        }

        public virtual T2 InnerValue
        {
            get => _valueVar.Value;
            set => _valueVar.Value = value;
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
            _valueVar = value;
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

    public abstract class DictionaryItemSOBase<T2, T3, T4> : InnerVariableSOBase<T3, T4>, IDictionaryItem<T2> where T3 : VariableSOBase<T4>
    {
        //[SerializeField, Multiline] protected string _editorDescription;
        [SerializeField] protected T2 _key;
        [SerializeField] protected EventSOBase<T2> _onKeyChange;

        public object RawKey { get => _key; set => Key = (T2)value; }
        public EventSOBase<T2> OnKeyChange => _onKeyChange;

        public virtual T2 Key
        {
            get => _key;
            set
            {
                bool invokeOnChange = _key != null ? !_key.Equals(value) : value != null;
                _key = value;
                if (_onKeyChange && invokeOnChange) _onKeyChange.Invoke(_key);
            }
        }

        public void SetKeyWithoutNotify(T2 value)
        {
            _key = value;
        }

        public void SetKey(VariableSOBase<T2, T3> variable)
        {
            Key = variable.Key;
        }

        public void SetKey(T2 key)
        {
            Key = key;
        }

        public static implicit operator T3(DictionaryItemSOBase<T2, T3, T4> variable)
        {
            return variable.Value;
        }

        public void ResetKey()
        {
            SetKeyWithoutNotify(default);
        }
    }

    public abstract class DictionaryItemSOBase2<T1, T2, T3> : VariableSOBase<T3>, IDictionaryItem<T2> where T1 : VariableSOBase<T2>
    {
        //[SerializeField, Multiline] protected string _editorDescription;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] protected T1 _keyVar;
        [SerializeField] protected EventSOBase<T1> _onKeyVarChange;

        public object RawKey { get => _keyVar; set => KeyVar = (T1)value; }
        public object InnerRawKey { get => _keyVar.Value; set => Key = (T2)value; }
        public EventSOBase<T1> OnKeyChange => _onKeyVarChange;

        public virtual T1 KeyVar
        {
            get => _keyVar;
            set
            {
                bool invokeOnChange = _keyVar != null ? !_keyVar.Equals(value) : value != null;
                _keyVar = value;
                if (_onKeyVarChange && invokeOnChange) _onKeyVarChange.Invoke(_keyVar);
            }
        }

        // Is the InnerKey to reuse IDictionaryItem<T>
        public T2 Key
        {
            get => _keyVar.Value;
            set => _keyVar.Value = value;
        }
        public object InnerRawValue { get => RawValue; set => RawValue = value; }
        public string InnerStringValue { get => Value.ToString(); set => ParseInnerValue(value); }

        public void SetKeyWithoutNotify(T1 value)
        {
            _keyVar = value;
        }

        public void SetInnerKeyWithoutNotify(T2 value)
        {
            _keyVar.SetValueWithoutNotify(value);
        }

        public void SetInnerKey(VariableSOBase<T2, T3> variable)
        {
            Key = variable.Key;
        }

        public void SetKey(T1 keyVar)
        {
            KeyVar = keyVar;
        }

        public void SetInnerKey(T2 key)
        {
            Key = key;
        }

        public static implicit operator T3(DictionaryItemSOBase2<T1, T2, T3> variable)
        {
            return variable.Value;
        }

        public void ResetKey()
        {
            SetKeyWithoutNotify(default);
        }

        public void ResetInnerKey()
        {
            SetInnerKeyWithoutNotify(default);
        }

        public void ParseInnerValue(string value)
        {
            switch (Type.GetTypeCode(typeof(T3)))
            {
                case TypeCode.String:
                    Value = (T3)(object)value;
                    break;
                case TypeCode.Int32:
                    Value = (T3)(object)int.Parse(value);
                    break;
                case TypeCode.Boolean:
                    Value = (T3)(object)bool.Parse(value);
                    break;
                case TypeCode.Decimal:
                    Value = (T3)(object)float.Parse(value);
                    break;
            }
        }
    }

    public abstract class DictionaryItemSOBase<T1, T2, T3, T4> : InnerVariableSOBase<T3, T4>, IDictionaryItem<T2> where T3 : VariableSOBase<T4> where T1 : VariableSOBase<T2>
    {
        //[SerializeField, Multiline] protected string _editorDescription;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] protected T1 _keyVar;
        [SerializeField] protected EventSOBase<T1> _onKeyVarChange;

        public object RawKey { get => _keyVar; set => KeyVar = (T1)value; }
        public object InnerRawKey { get => _keyVar.Value; set => Key = (T2)value; }
        public EventSOBase<T1> OnKeyChange => _onKeyVarChange;

        public virtual T1 KeyVar
        {
            get => _keyVar;
            set
            {
                bool invokeOnChange = _keyVar != null ? !_keyVar.Equals(value) : value != null;
                _keyVar = value;
                if (_onKeyVarChange && invokeOnChange) _onKeyVarChange.Invoke(_keyVar);
            }
        }

        // Is the InnerKey to reuse IDictionaryItem<T>
        public T2 Key 
        {
            get => _keyVar.Value;
            set => _keyVar.Value = value;
        }

        public void SetKeyWithoutNotify(T1 value)
        {
            _keyVar = value;
        }

        public void SetInnerKeyWithoutNotify(T2 value)
        {
            _keyVar.SetValueWithoutNotify(value);
        }

        public void SetInnerKey(VariableSOBase<T2, T4> variable)
        {
            Key = variable.Key;
        }

        public void SetKey(T1 keyVar)
        {
            KeyVar = keyVar;
        }

        public void SetInnerKey(T2 key)
        {
            Key = key;
        }

        public static implicit operator T4(DictionaryItemSOBase<T1, T2, T3, T4> variable)
        {
            return variable.InnerValue;
        }

        public void ResetKey()
        {
            SetKeyWithoutNotify(default);
        }

        public void ResetInnerKey()
        {
            SetInnerKeyWithoutNotify(default);
        }
    }

    public interface IVariable
    {
        object RawValue { get; set; }
    }

    public interface IInnerVariable : IVariable
    { 
        object InnerRawValue { get; set; }

        string InnerStringValue { get; set; }

        void ParseInnerValue(string value);
    }

    public interface IDictionaryItem<T1, T2> : IDictionaryItem<T1>
    {
        T2 Value { get; }
    }

    public interface IDictionaryItem<T> : IInnerVariable
    {
        T Key { get; }
    }

    public abstract class VariableSOBase : ScriptableObject, IVariable
    {
        public abstract object RawValue { get; set; }

        public abstract string StringValue { get; set; }

        public abstract void ResetValue();

        public T GetAs<T>() where T : VariableSOBase
        {
            return this as T;
        }

        public bool TryGetValue<T>(out T value)
        {
            try
            {
                value = (this as VariableSOBase<T>).Value;
                return true;
            }
            catch
            {
                // ignored
            }
            value = default;
            return false;
        }

        public bool TrySetValue<T>(T newValue)
        {
            try
            {
                (this as VariableSOBase<T>).Value = newValue;
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public T GetValueUnsafe<T>() => (this as VariableSOBase<T>).Value;

        public T SetValueUnsafe<T>(T newVal) => (this as VariableSOBase<T>).Value = newVal;
    }
}
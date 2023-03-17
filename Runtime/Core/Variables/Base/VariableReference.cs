#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
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
        public T Value => _useConstant ? _constantValue : _variable.Value;

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
}
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Variables
{
    [System.Serializable]
    public class VariableReference<TVarSO, T> where TVarSO : VariableSOBase<T>
    {
        [SerializeField] public bool _useConstant = true;
        [ShowIf("_useConstant")]
        [SerializeField] public T _constantValue;
        [HideIf("_useConstant")]
        [SerializeField] protected TVarSO _variable;

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
using d4160.MonoBehaviourData;

namespace d4160.Variables
{
    public abstract class VariableBehaviourBase<TVarSo, TObj> : MonoBehaviourUnityData<TVarSo> where TVarSo : VariableSOBase<TObj>
    {
        public TObj GetValue() => _data ? _data.Value : default;

        public void SetValue(TObj obj) {
            if (_data) _data.Value = obj;
        }

        public static implicit operator TObj(VariableBehaviourBase<TVarSo, TObj> variable)
        {
            return variable.GetValue();
        }
    }
}
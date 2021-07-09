using d4160.MonoBehaviourData;

namespace d4160.Variables
{
    public abstract class VariableBehaviourBase<TSo, TObj> : MonoBehaviourUnityData<TSo> where TSo : VariableSOBase<TObj>
    {
        public TObj GetValue() => _data ? _data.Value : default;

        public void SetValue(TObj obj) {
            if (_data) _data.Value = obj;
        }

        public static implicit operator TObj(VariableBehaviourBase<TSo, TObj> variable)
        {
            return variable.GetValue();
        }
    }
}
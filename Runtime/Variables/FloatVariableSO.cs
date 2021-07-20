using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/Float")]
    public class FloatVariableSO : VariableSOBase<float>
    {

    }

    [System.Serializable]
    public class FloatReference : VariableReference<FloatVariableSO, float>
    {
    }
}

using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/Double")]
    public class DoubleVariableSO : VariableSOBase<double>
    {

    }

    [System.Serializable]
    public class DoubleReference : VariableReference<DoubleVariableSO, double>
    {
    }
}

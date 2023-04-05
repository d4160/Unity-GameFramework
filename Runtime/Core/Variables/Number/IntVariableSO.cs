using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/Integer")]
    public class IntVariableSO : VariableSOBase<int>
    {

    }

    [System.Serializable]
    public class IntReference : VariableReference<IntVariableSO, int>
    {
    }
}

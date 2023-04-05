using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/ULong")]
    public class ULongVariableSO : VariableSOBase<ulong>
    {

    }

    [System.Serializable]
    public class ULongReference : VariableReference<ULongVariableSO, ulong>
    {
    }
}

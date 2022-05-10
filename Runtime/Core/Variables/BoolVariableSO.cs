using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/Bool")]
    public class BoolVariableSO : VariableSOBase<bool>
    {

    }

    [System.Serializable]
    public class BoolReference : VariableReference<BoolVariableSO, bool>
    {
    }
}

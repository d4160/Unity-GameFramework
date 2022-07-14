using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/String")]
    public class StringVariableSO : VariableSOBase<string>
    {

    }

    [System.Serializable]
    public class StringReference : VariableReference<StringVariableSO, string>
    {
    }
}

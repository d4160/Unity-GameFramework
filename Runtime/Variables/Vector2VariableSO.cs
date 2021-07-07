using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/Vector2")]
    public class Vector2VariableSO : VariableSOBase<Vector2>
    {

    }

    [System.Serializable]
    public class Vector2Reference : VariableReference<Vector2, Vector2VariableSO>
    {
    }
}

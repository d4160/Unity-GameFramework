using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/GameObject")]
    public class GameObjectVariableSO : VariableSOBase<GameObject>
    {

    }

    [System.Serializable]
    public class GameObjectReference : VariableReference<GameObject, GameObjectVariableSO>
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Variables
{
    [CreateAssetMenu(menuName = "d4160/Variables/GameObject")]
    public class GameObjectVariableSO : VariableSOBase<GameObject>
    {
        public void SetActive(bool active)
        {
            if (Value) Value.SetActive(active);
        }
    }

    [System.Serializable]
    public class GameObjectReference : VariableReference<GameObjectVariableSO, GameObject>
    {
        public void SetActive(bool active)
        {
            if (Variable) Variable.SetActive(active);
        }
    }
}

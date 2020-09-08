using NaughtyAttributes;
using UnityEngine;

namespace d4160.Core.MonoBehaviours
{
    public class GameObjectActiver : UnityObjectList<GameObject>
    {
        [BoxGroup("Activer Options")]
        [SerializeField] protected bool _activeState;

        public override void Init()
        {
            base.Init();

            SetActiveState(_activeState);
        }

        public virtual void SetActiveState(bool active)
        {
            ForEach((go) => go.SetActive(active));
        }
    }
}
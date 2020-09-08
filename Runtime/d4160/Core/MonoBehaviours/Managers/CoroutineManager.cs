using UnityEngine;

namespace d4160.Core.MonoBehaviours
{
    public class CoroutineManager : Singleton<CoroutineManager>
    {
        protected override void Awake()
        {
            base.Awake();

            gameObject.hideFlags = HideFlags.HideAndDontSave
                                   | HideFlags.HideInInspector;
        }
    }
}
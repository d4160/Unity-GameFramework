namespace d4160.Loops
{
    using UnityEngine;

    public class LateUpdateLoop : MonoBehaviour
    {
        public static event DeltaTimeDelegate OnLateUpdate;

        protected virtual void LateUpdate()
        {
            OnLateUpdate?.Invoke(Time.deltaTime);
        }
    }
}
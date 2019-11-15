namespace d4160.Loops
{
    using UnityEngine;

    public class FixedUpdateLoop : MonoBehaviour
    {
        public static event DeltaTimeDelegate OnFixedUpdate;

        protected virtual void FixedUpdate()
        {
            OnFixedUpdate?.Invoke(Time.fixedDeltaTime);
        }
    }
}

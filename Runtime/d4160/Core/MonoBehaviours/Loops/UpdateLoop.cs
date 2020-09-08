namespace d4160.Loops
{
    using UnityEngine;

    public delegate void DeltaTimeDelegate(float deltaTime);

    public class UpdateLoop : MonoBehaviour
    {
        public static event DeltaTimeDelegate OnUpdate;

        protected virtual void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
        }
    }
}
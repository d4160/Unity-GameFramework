using UnityEngine;

namespace d4160.Instancers
{
    public class GoPoolableObject : MonoBehaviour, IPoolableObject<GameObject>
    {
        public IObjectPool<GameObject> Pool { get; set; }

        public void Destroy()
        {
            Pool?.Destroy(this.gameObject);
        }
    }
}

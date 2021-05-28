using UnityEngine;

namespace d4160.Instancers
{
    public class GoPoolObject : MonoBehaviour, IPoolObject<GameObject>
    {
        public IObjectPool<GameObject> Pool { get; set; }

        public void Destroy()
        {
            Pool?.Destroy(this.gameObject);
        }
    }
}

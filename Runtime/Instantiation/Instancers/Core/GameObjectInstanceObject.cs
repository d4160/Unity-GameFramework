using UnityEngine;

namespace d4160.Instancers
{
    public class GameObjectInstanceObject : MonoBehaviour, IInstanceObject<GameObject>
    {
        public IProvider<GameObject> Provider { get; set; }

        public void Destroy()
        {
            Provider?.Destroy(this.gameObject);
        }
    }
}

using UnityEngine;

namespace d4160.Instancers
{
    public class GameObjectInstanceObject : MonoBehaviour, IInstanceObject<GameObject>
    {
        [SerializeField, Tooltip("Keep empty when use the ObjectPoll")] GameObjectProviderSOBase _provider;

        public IProvider<GameObject> Provider { get; set; }

        void Start()
        {
            if (_provider) Provider = _provider.Provider;
        }

        public void Destroy()
        {
            Provider?.Destroy(this.gameObject);
        }
    }
}

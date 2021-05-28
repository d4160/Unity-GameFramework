using UnityEngine;


namespace d4160.Instancers
{
    public class GameObjectFactory : IObjectFactory<GameObject>
    {
        protected GameObject _prefab;

        public GameObject Prefab { get => _prefab; set => _prefab = value; }

        public virtual void Destroy(GameObject instance)
        {
            if (instance)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(instance);
                }
                else
                {
                    GameObject.DestroyImmediate(instance);
                }
            }
        }

        public virtual GameObject Instantiate()
        {
            if (_prefab)
            {
                return GameObject.Instantiate(_prefab);
            }

            return null;
        }
    }
}

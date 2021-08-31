#if PHOTON_UNITY_NETWORKING
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using Photon.Pun;
using UltEvents;
using UnityEngine;

namespace d4160.Instancers.Photon
{
    public class PhotonFactoryBehaviour : MonoBehaviourUnityData<PhotonFactorySO>
    {
        [SerializeField] private Transform _parent;

        [Header("EVENTS")]
        [SerializeField] private UltEvent<PhotonView> _onInstanced;
        [SerializeField] private UltEvent<PhotonView> _onDestroy;

        private void CallOnInstanced(GameObject go) => _onInstanced?.Invoke(go.GetComponent<PhotonView>());
        private void CallOnDestroy(GameObject go) => _onDestroy?.Invoke(go.GetComponent<PhotonView>());

        void OnEnable() {
            if (_data) {
                _data.RegisterEvents();
                _data.OnInstanced += CallOnInstanced;
                _data.OnDestroy += CallOnDestroy;
            }
        }

        void OnDisable() {
            if (_data) {
                _data.UnregisterEvents();
                _data.OnInstanced -= CallOnInstanced;
                _data.OnDestroy -= CallOnDestroy;
            }
        }

        void Start() {
            if (_data) {
                _data.Parent = _parent;
                _data.Setup();
            }
        }

        [Button]
        public PhotonView Instantiate() => _data?.InstantiateAs<PhotonView>();
        public PhotonView Instantiate(Vector3 position, Quaternion rotation) => _data?.InstantiateAs<PhotonView>(position, rotation, _parent);
        public PhotonView Instantiate(Vector3 position, Quaternion rotation, Transform parent) => _data?.InstantiateAs<PhotonView>(position, rotation, parent);
        public PhotonView Instantiate(Transform parent, bool worldPositionStays = true) => _data?.InstantiateAs<PhotonView>(parent, worldPositionStays);

        public T InstantiateAs<T>() where T : PhotonView => _data?.InstantiateAs<T>();
        public T InstantiateAs<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : PhotonView => _data?.InstantiateAs<T>(position, rotation, parent);
        public T InstantiateAs<T>(Transform parent, bool worldPositionStays = true) where T : PhotonView => _data?.InstantiateAs<T>(parent, worldPositionStays);

        public void Destroy(PhotonView instance) => _data?.Destroy(instance);

        public void Destroy<T>(T instance) where T : PhotonView => _data?.Destroy(instance);
    }
}
#endif
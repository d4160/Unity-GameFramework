using d4160.MonoBehaviourData;
using NaughtyAttributes;
using Photon.Pun;
using UltEvents;
using UnityEngine;

namespace d4160.Instancers.Photon
{
    public class PhotonFactoryBehaviour : MonoBehaviourUnityData<PhotonFactorySO>
    {
        [SerializeField] private Transform _instancesParent;

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
                _data.Parent = _instancesParent;
                _data.Setup();
            }
        }

        [Button]
        public PhotonView Instantiate() {
            if (_data) return _data.InstantiateAs<PhotonView>(); return null;
        }

        public T InstantiateAs<T>() where T : PhotonView
        {
            if (_data) return _data.Instantiate() as T; return null;
        }

        public void Destroy(PhotonView instance) {
            if (_data) _data.Destroy(instance);
        }

        public void Destroy<T>(T instance) where T : PhotonView
        {
            if (_data) _data.Destroy(instance);
        }
    }
}
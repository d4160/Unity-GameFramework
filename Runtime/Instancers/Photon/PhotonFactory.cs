#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using UnityEngine;

namespace d4160.Instancers.Photon
{
    public class PhotonFactory : GameObjectFactory
    {
        protected byte _group = 0;
        protected object[] _data = null;
        protected bool _isRoomObject = false;

        public PhotonFactory()
        {
        }
        
        public PhotonFactory(PhotonView prefab)
        {
            _prefab = prefab.gameObject;
        }

        public byte Group { get => _group; set => _group = value; }
        public object[] Data { get => _data; set => _data = value; }
        public bool IsRoomObject { get => _isRoomObject; set => _isRoomObject = value; }

        public void Destroy(PhotonView instance) => Destroy(instance.gameObject);

        public override void Destroy(GameObject instance)
        {
            if (instance)
            {
                if (Application.isPlaying)
                {
                    InvokeOnDestroyEvent(instance);
                    PhotonNetwork.Destroy(instance.GetComponent<PhotonView>());
                }
                else
                {
                    GameObject.DestroyImmediate(instance);
                }
            }
        }

        public PhotonView InstantiateAsPhotonView() => InstantiateAs<PhotonView>();
        public PhotonView InstantiateAsPhotonView(Vector3 position, Quaternion rotation, Transform parent = null) => InstantiateAs<PhotonView>(position, rotation, parent);
        public PhotonView InstantiateAsPhotonView(Transform parent, bool worldPositionStays = true) => InstantiateAs<PhotonView>(parent, worldPositionStays);

        protected override GameObject Instantiate(Vector3 position, Quaternion rotation, bool setPositionAndRotation, Transform parent, bool worldPositionStays) {
            if (_prefab)
            {
                GameObject newGo;
                newGo = _isRoomObject ? PhotonNetwork.InstantiateRoomObject(_prefab.name, position, rotation, _group, _data) : PhotonNetwork.Instantiate(_prefab.name, position, rotation, _group, _data);
                if (parent) newGo.transform.SetParent(parent, worldPositionStays);
                else if (_parent) newGo.transform.SetParent(_parent, worldPositionStays);

                InvokeOnInstancedEvent(newGo);
                return newGo;
            }

            Debug.LogWarning($"The Prefab for this Factory: '{typeof(PhotonFactory)}', is missing.");
            return null;
        }
    }
}
#endif
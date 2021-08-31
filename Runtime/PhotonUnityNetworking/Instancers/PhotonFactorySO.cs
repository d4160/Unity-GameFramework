#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using UnityEngine;

namespace d4160.Instancers.Photon
{
    [CreateAssetMenu(menuName = "d4160/Instancers/Photon Factory")]
    public class PhotonFactorySO : GameObjectProviderSOBase
    {
        [Header ("PHOTON OPTIONS")]
        [SerializeField] protected byte _group = 0;
        [SerializeField] protected bool _isRoomObject = false;

        public byte Group { get => _group; set => _group = value; }
        public bool IsRoomObject { get => _isRoomObject; set => _isRoomObject = value; }

        public void SetGroup(byte value) => _factory.Group = value;
        public void SetIsRoomObject(bool value) => _factory.IsRoomObject = value;

        private readonly PhotonFactory _factory = new PhotonFactory();
        public override IProvider<GameObject> Provider => _factory;

        public override void Setup()
        {
            base.Setup();

            _factory.Group = _group;
            _factory.IsRoomObject = _isRoomObject;
        }

        public PhotonView InstantiateAsPhotonView() => _factory.InstantiateAsPhotonView();
        public PhotonView InstantiateAsPhotonView(Vector3 position, Quaternion rotation, Transform parent = null) => _factory.InstantiateAsPhotonView(position, rotation, parent);
        public PhotonView InstantiateAsPhotonView(Transform parent, bool worldPositionStays = true) => _factory.InstantiateAsPhotonView(parent, worldPositionStays);

        public void Destroy(PhotonView instance) => _factory.Destroy(instance.gameObject);
    }
}
#endif
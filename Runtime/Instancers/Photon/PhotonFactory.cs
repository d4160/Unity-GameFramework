#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using UnityEngine;

namespace d4160.Instancers.Photon
{
    public class PhotonFactory : ComponentFactory<PhotonView>
    {
        protected Vector3 _position;
        protected Quaternion _rotation;
        protected byte _group;
        protected object[] _data;

        public PhotonFactory(PhotonView prefab) : base(prefab)
        {
        }

        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public byte Group { get => _group; set => _group = value; }
        public object[] Data { get => _data; set => _data = value; }

        public override void Destroy(PhotonView instance)
        {
            if (instance)
            {
                if (Application.isPlaying)
                {
                    PhotonNetwork.Destroy(instance);
                }
                else
                {
                    GameObject.DestroyImmediate(instance);
                }
            }
        }

        public override PhotonView Instantiate()
        {
            if (_prefab)
            {
                return PhotonNetwork.Instantiate(_prefab.name, _position, _rotation, _group, _data).GetComponent<PhotonView>();
            }

            return null;
        }
    }
}
#endif
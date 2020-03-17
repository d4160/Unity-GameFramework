using UnityEngine;

namespace d4160.Core
{
    public class TransformBehaviour : MonoBehaviour
    {
        [SerializeField] protected bool _local;

        protected Vector3 _position;
        protected Quaternion _rotation;

        public Vector3 StartPosition
        {
            get => _position;
            set => _position = value;
        }

        public Quaternion StartRotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        protected virtual void Awake()
        {
            _position = _local ? transform.localPosition : transform.position;
            _rotation = _local ? transform.localRotation : transform.rotation;
        }

        public virtual void RestoreStartPosition()
        {
            if (!_local)
            {
                transform.position = _position;
            }
            else
            {
                transform.localPosition = _position;
            }
        }

        public virtual void RestoreStartRotation()
        {
            if (!_local)
            {
                transform.rotation = _rotation;
            }
            else
            {
                transform.localRotation = _rotation;
            }
        }

        public virtual void RestoreStartTransform()
        {
            if (!_local)
            {
                transform.SetPositionAndRotation(_position, _rotation);
            }
            else
            {
                RestoreStartPosition();
                RestoreStartRotation();
            }
        }
    }
}

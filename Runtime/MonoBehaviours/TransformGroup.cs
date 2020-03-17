using UnityEngine;
using UnityEngine.Events;

namespace d4160.Core
{
    public class TransformGroup : MonoBehaviour
    {
        [SerializeField] protected bool _searchInChildren;
        [SerializeField] protected TransformBehaviour[] _transforms;
        [SerializeField] protected UnityEvent _onChildrenDisabled;

        protected virtual void Awake()
        {
            if (_searchInChildren)
            {
                _transforms = GetComponentsInChildren<TransformBehaviour>();
            }
        }

        public virtual void RestoreStartPositions()
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].RestoreStartPosition();
            }
        }

        public virtual void RestoreStartRotations()
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].RestoreStartRotation();
            }
        }

        public virtual void RestoreStartTransforms()
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].RestoreStartTransform();
            }
        }

        public virtual void SetGroupActive(bool active)
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                if (!_transforms[i].gameObject.activeSelf)
                    _transforms[i].gameObject.SetActive(active);
            }
        }

        public virtual void OnChildDisabled()
        {
            bool allDisabled = true;
            for (int i = 0; i < _transforms.Length; i++)
            {
                if (_transforms[i].gameObject.activeSelf)
                {
                    allDisabled = false;
                    break;
                }
            }

            if (allDisabled)
            {
                _onChildrenDisabled?.Invoke();
            }
        }
    }
}
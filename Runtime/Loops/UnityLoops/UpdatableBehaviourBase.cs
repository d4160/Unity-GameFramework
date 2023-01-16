using UnityEngine;

namespace d4160.Loops
{
    public abstract class UpdatableBehaviourBase : MonoBehaviour, IUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.RemoveListener(this);
        }

        public abstract void OnUpdate(float deltaTime);
    }

    public abstract class FixedUpdatableBehaviourBase : MonoBehaviour, IFixedUpdateObject
    {
        protected virtual void OnEnable()
        {
            FixedUpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            FixedUpdateManager.RemoveListener(this);
        }

        public abstract void OnFixedUpdate(float deltaTime);
    }

    public abstract class LateUpdatableBehaviourBase : MonoBehaviour, ILateUpdateObject
    {
        protected virtual void OnEnable()
        {
            LateUpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            LateUpdateManager.RemoveListener(this);
        }

        public abstract void OnLateUpdate(float deltaTime);
    }

    public abstract class FixedWithUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, IFixedUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.AddListener(this);
            FixedUpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.RemoveListener(this);
            FixedUpdateManager.RemoveListener(this);
        }

        public abstract void OnUpdate(float deltaTime);

        public abstract void OnFixedUpdate(float deltaTime);
    }

    public abstract class LateWithUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, ILateUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.AddListener(this);
            LateUpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.RemoveListener(this);
            LateUpdateManager.RemoveListener(this);
        }

        public abstract void OnUpdate(float deltaTime);

        public abstract void OnLateUpdate(float deltaTime);
    }

    public abstract class LateWithFixedUpdatableBehaviourBase : MonoBehaviour, IFixedUpdateObject, ILateUpdateObject
    {
        protected virtual void OnEnable()
        {
            FixedUpdateManager.AddListener(this);
            LateUpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            FixedUpdateManager.RemoveListener(this);
            LateUpdateManager.RemoveListener(this);
        }

        public abstract void OnFixedUpdate(float deltaTime);

        public abstract void OnLateUpdate(float deltaTime);
    }

    public abstract class AllUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, IFixedUpdateObject, ILateUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.AddListener(this);
            FixedUpdateManager.AddListener(this);
            LateUpdateManager.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.RemoveListener(this);
            FixedUpdateManager.RemoveListener(this);
            LateUpdateManager.RemoveListener(this);
        }

        public abstract void OnUpdate(float deltaTime);

        public abstract void OnFixedUpdate(float deltaTime);

        public abstract void OnLateUpdate(float deltaTime);
    }
}
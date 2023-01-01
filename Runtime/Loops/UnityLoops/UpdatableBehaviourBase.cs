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

    public abstract class WithFixedUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, IFixedUpdateObject
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

    public abstract class WithLateUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, ILateUpdateObject
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

    public abstract class WithLateFixedUpdatableBehaviourBase : MonoBehaviour, IFixedUpdateObject, ILateUpdateObject
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
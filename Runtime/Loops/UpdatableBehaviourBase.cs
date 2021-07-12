using UnityEngine;

namespace d4160.Loops
{
    public abstract class UpdatableBehaviourBase : MonoBehaviour, IUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.RegisterObject(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.UnregisterObject(this);
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }
    }

    public abstract class FixedUpdatableBehaviourBase : MonoBehaviour, IFixedUpdateObject
    {
        protected virtual void OnEnable()
        {
            FixedUpdateManager.RegisterObject(this);
        }

        protected virtual void OnDisable()
        {
            FixedUpdateManager.UnregisterObject(this);
        }

        public virtual void OnFixedUpdate(float deltaTime)
        {
        }
    }

    public abstract class LateUpdatableBehaviourBase : MonoBehaviour, ILateUpdateObject
    {
        protected virtual void OnEnable()
        {
            LateUpdateManager.RegisterObject(this);
        }

        protected virtual void OnDisable()
        {
            LateUpdateManager.UnregisterObject(this);
        }

        public virtual void OnLateUpdate(float deltaTime)
        {
        }
    }

    public abstract class WithFixedUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, IFixedUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.RegisterObject(this);
            FixedUpdateManager.RegisterObject(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.UnregisterObject(this);
            FixedUpdateManager.UnregisterObject(this);
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
        }
    }

    public abstract class AllUpdatableBehaviourBase : MonoBehaviour, IUpdateObject, IFixedUpdateObject, ILateUpdateObject
    {
        protected virtual void OnEnable()
        {
            UpdateManager.RegisterObject(this);
            FixedUpdateManager.RegisterObject(this);
            LateUpdateManager.RegisterObject(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.UnregisterObject(this);
            FixedUpdateManager.UnregisterObject(this);
            LateUpdateManager.UnregisterObject(this);
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
        }

        public void OnLateUpdate(float deltaTime)
        {
        }
    }
}
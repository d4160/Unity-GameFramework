#if PHOTON_UNITY_NETWORKING
using UnityEngine;
using Photon.Pun;

namespace d4160.Loops.Photon
{
    public abstract class UpdatableBehaviourPunBase : MonoBehaviourPun, IUpdateObject
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

    public abstract class FixedUpdatableBehaviourPunBase : MonoBehaviourPun, IFixedUpdateObject
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

    public abstract class LateUpdatableBehaviourPunBase : MonoBehaviourPun, ILateUpdateObject
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

    public abstract class WithFixedUpdatableBehaviourPunBase : MonoBehaviourPun, IUpdateObject, IFixedUpdateObject
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

    public abstract class AllUpdatableBehaviourPunBase : MonoBehaviourPun, IUpdateObject, IFixedUpdateObject, ILateUpdateObject
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
#endif
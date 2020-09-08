// UltEvents // Copyright 2019 Kybernetik //

using Lean.Pool;
using UnityEngine;

namespace UltEvents
{
    /// <summary>
    /// Holds <see cref="UltEvent"/>s which are called by <see cref="IPoolable"/> interface:
    /// <see cref="LeanPoolEvents"/>.
    /// </summary>
    [AddComponentMenu(UltEventUtils.ComponentMenuPrefix + "LeanPool Events")]
    [DisallowMultipleComponent]
    public class LeanPoolEvents : MonoBehaviour, IPoolable
    {
        [SerializeField] protected UltEvent _onSpawned;

        public UltEvent OnSpawned
        {
            get => _onSpawned ?? (_onSpawned = new UltEvent());
            set => _onSpawned = value;
        }

        [SerializeField] protected UltEvent _onDespawned;

        public UltEvent OnDespawned
        {
            get => _onDespawned ?? (_onDespawned = new UltEvent());
            set => _onDespawned = value;
        }

        public virtual void OnSpawn()
        {
            OnSpawned.Invoke();
        }

        public virtual void OnDespawn()
        {
            OnDespawned.Invoke();
        }
    }
}
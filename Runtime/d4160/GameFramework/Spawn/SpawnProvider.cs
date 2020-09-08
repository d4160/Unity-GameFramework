using d4160.Core;
using Lean.Pool;
using UnityEngine;
using UnityExtensions;

namespace d4160.Core
{
    public class SpawnProvider : MonoBehaviour, ISpawnProvider
    {
        [Header("POOL OPTIONS")]
        [SerializeField] protected Transform _parent;
        [InspectInline(canEditRemoteTarget = true)]
        [SerializeField] protected LeanGameObjectPool[] _pools;
        [SerializeField] protected Vector3 _offset;

        protected Vector3? _overrideSpawnPosition = null;
        protected GameObject _lastGameObject;

        public int SelectedSourceIndex { get; set; } = 0;

        public LeanGameObjectPool SelectedPool =>
            _pools.IsValidIndex(SelectedSourceIndex) ? _pools[SelectedSourceIndex] : null;

        public virtual bool CanSpawn => true;
        public virtual Vector3 SpawnPosition => SelectedPool.transform.position + _offset;
        public virtual Quaternion SpawnRotation => SelectedPool.transform.rotation;

        public Vector3? OverrideSpawnPosition
        {
            get => _overrideSpawnPosition;
            set => _overrideSpawnPosition = value;
        }

        public GameObject LastGameObject => _lastGameObject;

        public virtual LeanGameObjectPool GetPool(int sourceIndex) => _pools.IsValidIndex(sourceIndex) ? _pools[sourceIndex] : SelectedPool;

        public virtual void Spawn(int sourceIndex = -1)
        {
            if (!CanSpawn) return;

            var pool = sourceIndex == -1 ? SelectedPool : GetPool(sourceIndex);
            if (pool)
            {
                GameObjectSpawn(pool.Spawn(_overrideSpawnPosition ?? SpawnPosition, SpawnRotation, _parent));
            }
        }

        public virtual void Despawn(GameObject instance, int sourceIndex = -1, float delay = 0f)
        {
            if (!instance) return;

            var pool = GetPool(sourceIndex);
            if (pool)
            {
                pool.Despawn(instance, delay);
            }
        }

        protected virtual void GameObjectSpawn(GameObject instance)
        {
            _lastGameObject = instance;
        }
    }
}
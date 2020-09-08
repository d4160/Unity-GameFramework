using System.Collections;
using System.Collections.Generic;
using System.Linq;
using d4160.Core;
using Lean.Pool;
using UltEvents;
#if UNITY_ECS
using Unity.Entities;
using Unity.Transforms;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityExtensions;
using WeightedRandomization;

namespace d4160.Core   
{
    public class Spawner : MonoBehaviour, ISpawner
    {
        [Header("PROVIDER OPTIONS")]
        [InspectInline(canEditRemoteTarget = true)]
        [SerializeField] protected SpawnProvider _provider;
        // Can use director timeline to call random spawns and designed bunches spawns  
        [SerializeField] protected bool _useDirectorAsSpawner;
        [SerializeField] protected PlayableDirector _spawnDirector;

        [Header("SPAWN OPTIONS")]
        [InspectInline(canEditRemoteTarget = true)]
        [SerializeField] protected SpawnDataSO _spawnDataSO;
        [SerializeField] protected bool _startSpawnAtStart;
#if UNITY_ECS
        [SerializeField] protected bool _useECS;
#endif
        [SerializeField] protected Transform _waveSpawnTransform;
        [Tooltip("If is infinite use a random wave by default. To control the difficulty for this I recommend to use an external source to active and disable spawners like a Timeline, a Timer or a SpawnController.")]
        [SerializeField] protected bool _infiniteWaves;
        [SerializeField] protected LoopType _loopType;
        [SerializeField] protected IntWeightedRandomizer _waveRandomizer;

        [Header("EVENTS")] 
        [SerializeField] protected IntUltEvent _onWaveSpawnStarted;
        [SerializeField] protected IntUltEvent _onWaveSpawnCompleted;
        [SerializeField] protected UltEvent _onSpawnStarted;
        [SerializeField] protected UltEvent _onSpawnStopped;
        [SerializeField] protected UltEvent _onSpawnPaused;
        [SerializeField] protected UltEvent _onSpawnResumed;

        protected WaitForSeconds _waitForSpawn;
        protected Coroutine _spawnCoroutine;
        protected int _currentWaveIndex;
        protected bool _pingPongInverseDirection;
#if UNITY_ECS
        protected Entity _entityPrefab;
        protected EntityManager _entityManager;
        protected GameObjectConversionSettings _conversionSettings;
        protected BlobAssetStore _blobAssetStore;
#endif

        public Vector2 SpawnRate
        {
            get
            {
                if (_spawnDataSO)
                    return _spawnDataSO.MinMaxSpawnRate;

                return default;
            }
            set
            {
                if (_spawnDataSO)
                    _spawnDataSO.MinMaxSpawnRate = value;

                CalculateWaitForSeconds();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("FixWaveRandomizerWeights")]
        private void FixWaveRandomizerWeights()
        {
            _waveRandomizer?.FixRandomizerWeights();
        }

        [ContextMenu("FixSpawnDataRandomizerWeights")]
        private void FixSpawnDataRandomizerWeights()
        {
            if (_spawnDataSO)
            {
                foreach (var wave in _spawnDataSO.Waves)
                {
                    foreach (var step in wave.instantiationStep.instantiations)
                    {
                        step.FixRandomizerWeights();
                    }
                }
            }
        }
#endif

        protected virtual void Awake()
        {
            if (!_waveSpawnTransform)
                _waveSpawnTransform = transform;

#if UNITY_ECS
            if (_useECS)
            {
                _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                _blobAssetStore = new BlobAssetStore();
                _conversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
            }
#endif
        }

        protected virtual void Start()
        {
            if (_startSpawnAtStart)
            {
                if (_spawnDataSO)
                {
                    _currentWaveIndex = _spawnDataSO.Waves.Length > 0 ? 0 : -1;

                    if (_currentWaveIndex >= 0)
                    {
                        StartSpawn();
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            StopSpawn();
        }

        protected virtual void OnDestroy()
        {
#if UNITY_ECS
            _blobAssetStore?.Dispose();
#endif
        }

        public virtual void SpawnWave(int waveIndex = -1)
        {
            if (_currentWaveIndex < 0 && waveIndex < 0) return;

            StartCoroutine(InstantiateRoutine(waveIndex));

            _onWaveSpawnStarted?.Invoke(waveIndex);
        }

        protected virtual IEnumerator InstantiateRoutine(int waveIndex = -1)
        {
            if (_provider == null) yield break;

            var currentWaveIndex = waveIndex <= -1 ? _currentWaveIndex : waveIndex;

            var waveData = _spawnDataSO.Waves[currentWaveIndex];

            if (waveData.instantiationStep.wavePrefab)
            {
                InstantiateWavePrefab(waveData.instantiationStep.wavePrefab);
                yield return null;
            }
            else
            {
                var waitFor = new WaitForSeconds(waveData.minMaxInstantiationRate.Random());
                for (int i = 0; i < waveData.instantiationStep.instantiations.Length; i++)
                {
                    var instantiation = waveData.instantiationStep.instantiations[i];
                    var count = instantiation.MinMaxNumber.Random();

                    for (int j = 0; j < count; j++)
                    {
                        _provider.Spawn(instantiation.GetNext());
                        yield return waitFor;
                    }
                }

                _onWaveSpawnCompleted?.Invoke(waveIndex);
            }
        }

        protected virtual void InstantiateWavePrefab(GameObject prefab)
        {
#if UNITY_ECS
            if (_useECS)
            {
                _entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, _conversionSettings);
                var newEntity = _entityManager.Instantiate(_entityPrefab);
                _entityManager.SetComponentData(newEntity, new Translation(){ Value = _waveSpawnTransform.position });
                _entityManager.SetComponentData(newEntity, new Rotation(){ Value = _waveSpawnTransform.rotation });
            }
            else
#endif
            {
                Instantiate(prefab, _waveSpawnTransform.position, _waveSpawnTransform.rotation);
            }
        }

        protected virtual IEnumerator SpawnRoutine()
        {
            while (true)
            {
                SpawnWave();

                if (SpawnRate == Vector2.zero)
                    yield break;

                yield return _waitForSpawn;

                if (SpawnRate.x != SpawnRate.y)
                    CalculateWaitForSeconds();

                if (!CalculateCurrentWaveIndex())
                    yield break;
            }
        }

        public bool CalculateCurrentWaveIndex()
        {
            if (_infiniteWaves)
            {
                if (_spawnDataSO.Waves.Length == 1) return true;
                
                switch (_loopType)
                {
                    case LoopType.Random:
                        _currentWaveIndex = _waveRandomizer.GetNext();
                        break;

                    case LoopType.PingPong:

                        _currentWaveIndex = _pingPongInverseDirection ? _currentWaveIndex - 1 : _currentWaveIndex + 1;
                        if (!_spawnDataSO.Waves.IsValidIndex(_currentWaveIndex))
                        {
                            _currentWaveIndex = _currentWaveIndex == -1 ? 1 : _spawnDataSO.Waves.LastIndex() - 1;
                            _pingPongInverseDirection = !_pingPongInverseDirection;
                        }
                        break;

                    case LoopType.Restart:
                        _currentWaveIndex++;
                        if (!_spawnDataSO.Waves.IsValidIndex(_currentWaveIndex))
                            _currentWaveIndex = 0;
                        break;
                }
            }
            else
            {
                _currentWaveIndex++;
                if (!_spawnDataSO.Waves.IsValidIndex(_currentWaveIndex))
                {
                    return false;
                }
            }

            return true;
        }

        protected void CalculateWaitForSeconds()
        {
            _waitForSpawn = new WaitForSeconds(_spawnDataSO.MinMaxSpawnRate.Random());
        }

        public virtual void StartSpawn()
        {
            if (_useDirectorAsSpawner)
            {
                _spawnDirector?.Play();
            }
            else
            {
                if (_spawnCoroutine != null) return;

                if (_waitForSpawn == null)
                {
                    if (SpawnRate != Vector2.zero)
                    {
                        CalculateWaitForSeconds();
                    }
                }

                _spawnCoroutine = StartCoroutine(SpawnRoutine());
            }

            _onSpawnStarted?.Invoke();
        }

        public virtual void StopSpawn()
        {
            if (_useDirectorAsSpawner)
            {
                _spawnDirector?.Stop();
            }
            else
            {
                if (_spawnCoroutine != null)
                {
                    StopCoroutine(_spawnCoroutine);
                }
            }

            _onSpawnStopped?.Invoke();
        }

        public virtual void PauseSpawn()
        {
            if (_useDirectorAsSpawner)
            {
                _spawnDirector?.Pause();
            }
            else
            {
                if (_spawnCoroutine != null)
                {
                    StopCoroutine(_spawnCoroutine);
                }
            }

            _onSpawnPaused?.Invoke();
        }

        public void ResumeSpawn()
        {
            if (_useDirectorAsSpawner)
            {
                _spawnDirector?.Resume();
            }
            else
            {
                if (_spawnCoroutine != null) return;

                if (_waitForSpawn == null)
                {
                    if (SpawnRate != Vector2.zero)
                    {
                        CalculateWaitForSeconds();
                    }
                }

                _spawnCoroutine = StartCoroutine(SpawnRoutine());
            }

            _onSpawnResumed?.Invoke();
        }
    }
}


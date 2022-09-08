#if NODE_GRAPH_PROCESSOR
using System.Collections;
using UnityEngine;
using d4160.Instancers;
using GraphProcessor;
using d4160.Coroutines;
using d4160.NodeGraphProcessor;
using d4160.Core;
using d4160.Events;

namespace d4160.Spawners
{
    public class Spawner<T> where T : class
    {
        public VoidEventSO OnWaveEnded { get; set; }
        public ObjectProviderSOBase<T> InstancerSO { get; set; }
        public BaseGraph Graph { get; set; }
        public Vector2 TimeUntilFirstSpawn { get; set; }

        protected StepProcessor _graphProcessor;
        protected WaveNode _currentWave;
        protected Coroutine _spawnRoutine;
        protected float _waitInSeconds;
        protected WaitForSeconds _waitForSeconds;
        protected bool _paused = false;

        protected T Spawn() => InstancerSO?.Instantiate();
        protected T Spawn(Transform parent, bool worldPositionStays = true) => InstancerSO?.Instantiate(parent, worldPositionStays);
        protected T Spawn(Vector3 position, Quaternion rotation, Transform parent = null) => InstancerSO?.Instantiate(position, rotation, parent);

        public void Setup() 
        {
            if(_graphProcessor == null && Graph != null)
                _graphProcessor = new StepProcessor(Graph);

            NextWave();
        }

        public void NextWave()
        {
            StopSpawn();

            _graphProcessor?.Step();
            _currentWave = _graphProcessor?.Current as WaveNode;
        }

        public void StartSpawn() 
        {
            if(_spawnRoutine != null) return;

            _paused = false;
            _spawnRoutine = SpawnRoutine().StartCoroutine();
        }

        private IEnumerator SpawnRoutine() 
        {
            if (_currentWave == null) yield break;

            float waitForFirstSpawn = TimeUntilFirstSpawn.Random();
            if (waitForFirstSpawn > 0)
            {
                yield return new WaitForSeconds(waitForFirstSpawn);
            }

            int spawnsNumber = _currentWave.spawnsNumber.Random();
            int spawnsCount = 0;

            while (spawnsNumber == 0 || spawnsCount < spawnsNumber)
            {
                if (!_paused)
                {
                    float newWaitValue = _currentWave.timeBetweenSpawns.Random();
                    if (newWaitValue != _waitInSeconds)
                    {
                        _waitInSeconds = newWaitValue;
                        _waitForSeconds = new WaitForSeconds(_waitInSeconds);
                    }

                    int instancesNumber = _currentWave.instancesInEachSpawn.Random();
                    for (int i = 0; i < instancesNumber; i++)
                    {
                        Spawn();
                    }

                    spawnsCount++;

                    yield return _waitForSeconds;
                }
                else {
                    yield return null;
                }
            }

            _spawnRoutine = null;
            OnWaveEnded?.Invoke();
        }

        public void StopSpawn() 
        {
            if (_spawnRoutine != null) 
            {
                SpawnRoutine().StopCoroutine();
                _spawnRoutine = null;
                OnWaveEnded?.Invoke();
            }
        }

        public void PauseSpawn() {
            if (_paused) return;

            _paused = true;
        }

        public void ResumeSpawn() {
            if (!_paused) return;

            _paused = false;
        }

    }
}
#endif
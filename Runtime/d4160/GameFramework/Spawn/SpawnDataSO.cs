using Malee;
using UnityEngine;
using WeightedRandomization;

namespace d4160.Core
{
    [CreateAssetMenu]
    public class SpawnDataSO : ScriptableObject
    {
        [SerializeField] protected Vector2 _minMaxSpawnRate;
        [Reorderable(paginate = true, pageSize = 10)]
        [SerializeField] protected ReorderableWaveArray _waves;

        public Vector2 MinMaxSpawnRate
        {
            get => _minMaxSpawnRate;
            set => _minMaxSpawnRate = value;
        }

        public ReorderableWaveArray Waves => _waves;

#if UNITY_EDITOR
        [ContextMenu("FixRandomizerWeights")]
        private void FixSpawnDataRandomizerWeights()
        {
            foreach (var wave in Waves)
            {
                foreach (var step in wave.instantiationStep.instantiations)
                {
                    step.FixRandomizerWeights();
                }
            }
        }
#endif
    }

    [System.Serializable]
    public struct WaveData
    {
        public Vector2 minMaxInstantiationRate;
        public InstantiationStep instantiationStep;
    }

    [System.Serializable]
    public struct InstantiationStep
    {
        public GameObject wavePrefab;
        [Reorderable(paginate = true, pageSize = 10)]
        public ReorderableInstantiationWeightArray instantiations;
    }

    [System.Serializable]
    public class InstantiationWeight : IntWeightedRandomizer
    {
        [SerializeField] protected Vector2Int _minMaxNumber;

        public Vector2Int MinMaxNumber => _minMaxNumber;
    }

    [System.Serializable]
    public class ReorderableWaveArray : ReorderableArray<WaveData>
    {
    }

    [System.Serializable]
    public class ReorderableInstantiationWeightArray : ReorderableArray<InstantiationWeight>
    {
    }
}
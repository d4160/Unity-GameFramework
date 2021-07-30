using GraphProcessor;
using UnityEngine;
//using WeightedRandomization;

namespace d4160.NodeGraphProcessor
{
    [System.Serializable, NodeMenuItem("Spawn/Wave")]
    public class WaveNode : StepNodeBase
    {
        [ShowInInspector(true)]
        public Vector2Int spawnsNumber;
        [ShowInInspector(true)]
        public Vector2 timeBetweenSpawns;
        [ShowInInspector(true)]
        public Vector2Int instancesInEachSpawn;
        //[ShowInInspector(true)]
        //public GameObjectLibrary spawnLibrary;
        //public WeightedRandomizer<GameObject> libraryChances = new WeightedRandomizer<GameObject>();

        public override string name => "Wave";

        protected override void Process()
        {
        }
    }
}

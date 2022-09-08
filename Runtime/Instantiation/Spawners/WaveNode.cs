#if NODE_GRAPH_PROCESSOR
using d4160.NodeGraphProcessor;
using GraphProcessor;
using UnityEngine;
//using WeightedRandomization;

namespace d4160.Spawners
{
    [System.Serializable, NodeMenuItem("Spawn/Wave")]
    public class WaveNode : StepNodeBase
    {
        [ShowInInspector(true)]
        public Vector2Int spawnsNumber = new Vector2Int(5, 5);
        [ShowInInspector(true)]
        public Vector2 timeBetweenSpawns = new Vector2(5f, 5f);
        [ShowInInspector(true)]
        public Vector2Int instancesInEachSpawn = new Vector2Int(5, 5);
        //[ShowInInspector(true)]
        //public GameObjectLibrary spawnLibrary;
        //public WeightedRandomizer<GameObject> libraryChances = new WeightedRandomizer<GameObject>();

        public override string name => "Wave";

        protected override void Process()
        {
        }
    }
}
#endif
using UnityEngine;

namespace d4160.Instancers
{
    [CreateAssetMenu(menuName = "d4160/Instancers/GameObject Factory")]
    public class GameObjectFactorySO : GameObjectProviderSOBase
    {
        private readonly GameObjectFactory _factory = new GameObjectFactory();

        public override IProvider<GameObject> Provider => _factory;
    }
}
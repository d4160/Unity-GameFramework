using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers.Photon
{
    [CreateAssetMenu(menuName = "d4160/Instancers/Photon Factory")]
    public class PhotonFactorySO : GameObjectProviderSOBase
    {
        private readonly PhotonFactory _factory = new PhotonFactory();

        public override IOutProvider<GameObject> OutProvider => _factory;
        public override IInProvider<GameObject> InProvider => _factory;
    }
}
using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    [CreateAssetMenu(menuName = "d4160/Instancers/UniqueComponent Provider")]
    public class UniqueComponentProviderSO : ComponentProviderSOBase
    {
        private readonly UniqueComponentProvider<Component> _provider = new UniqueComponentProvider<Component>();

        public override IProvider<Component> Provider => _provider;

        public override Component Prefab { get; set; }
    }
}
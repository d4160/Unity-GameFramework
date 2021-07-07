using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    [CreateAssetMenu(menuName = "d4160/Instancers/UniqueComponent Provider")]
    public class UniqueComponentProviderSO : ComponentProviderSOBase
    {
        private readonly UniqueComponentProvider<Component> _provider = new UniqueComponentProvider<Component>();

        public override IOutProvider<Component> OutProvider => _provider;
        public override IInProvider<Component> InProvider => _provider;
    }
}
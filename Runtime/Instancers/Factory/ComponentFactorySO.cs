using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    [CreateAssetMenu(menuName = "d4160/Instancers/Component Factory")]
    public class ComponentFactorySO : ComponentProviderSOBase
    {
        private readonly ComponentFactory<Component> _factory = new ComponentFactory<Component>();

        public override IOutProvider<Component> OutProvider => _factory;
        public override IInProvider<Component> InProvider => _factory;
    }
}
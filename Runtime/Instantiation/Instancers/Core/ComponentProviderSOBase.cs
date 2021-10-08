using System;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Instancers
{
    public abstract class ComponentProviderSOBase : ObjectProviderSOBase<Component>
    {
        public override T2 InstantiateAs<T2>()
        {
            return GetAs<T2>(Instantiate());
        }

        public override T2 InstantiateAs<T2>(Transform parent, bool worldPositionStays = true)
        {
            return GetAs<T2>(Instantiate(parent, worldPositionStays));
        }

        public override T2 InstantiateAs<T2>(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return GetAs<T2>(Instantiate(position, rotation, parent));
        }

        private T2 GetAs<T2>(Component newComp) where T2 : class => typeof(T2) == typeof(GameObject) ? newComp.gameObject as T2 : newComp as T2;

        public override void Destroy<T2>(T2 instance) => Provider.Destroy(typeof(T2) == typeof(GameObject) ? (instance as GameObject).transform : instance as Component);
    }
}
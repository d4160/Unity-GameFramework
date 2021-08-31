using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    public abstract class GameObjectProviderSOBase : ObjectProviderSOBase<GameObject>
    {
        public override T InstantiateAs<T>() where T : class => Instantiate().GetComponent<T>();
        public override T InstantiateAs<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : class => Instantiate(position, rotation, parent).GetComponent<T>();
        public override T InstantiateAs<T>(Transform parent, bool worldPositionStays = true) where T : class => Instantiate(parent, worldPositionStays).GetComponent<T>();

        public override void Destroy<T>(T instance) where T : class => Provider.Destroy((instance as Component)?.gameObject);
    }
}
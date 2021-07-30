using UnityEngine;

namespace d4160.Collections
{
    [CreateAssetMenu(menuName = "d4160/Collections/Component Library")]
    public class ComponentLibrarySO : LibrarySOBase<Component>
    {
        public override T2 GetAs<T2>(int i) => typeof(T2) == typeof(GameObject) ? this[i]?.gameObject as T2 : base.GetAs<T2>(i);
        public override T2 RandomAs<T2>() => typeof(T2) == typeof(GameObject) ? Random.gameObject as T2 : base.RandomAs<T2>();
    }
}
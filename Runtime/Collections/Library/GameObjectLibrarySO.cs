using UnityEngine;

namespace d4160.Collections
{
    [CreateAssetMenu(menuName = "d4160/Collections/GameObject Library")]
    public class GameObjectLibrarySO : LibrarySOBase<GameObject>
    {
        public override T2 GetAs<T2>(int i) => this[i]?.GetComponent<T2>();
        public override T2 RandomAs<T2>() => Random.GetComponent<T2>();
    }
}
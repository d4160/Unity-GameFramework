using System.Collections.Generic;
using UnityEngine;

namespace d4160.Instancers
{
    public class ComponentPool<T> : ComponentFactory<T> where T : Component
    {
        protected Transform _poolParent;

        protected int _initialCount = 0;
        [Tooltip("When there is no more items in the pool, the next numbers of items to instantiate")]
        protected int _batchSize = 1;

        protected Queue<T> _pool = new Queue<T>();

        public ComponentPool(int initialCount, int batchSize, T prefab, Transform parent) : base(prefab)
        {
            _parent = parent;
            _initialCount = initialCount < 0 ? 0 : initialCount;
            _batchSize = batchSize < 0 ? 0 : batchSize;

            GenerateInitialBatch();
        }

        private void GenerateInitialBatch() {
            for (var i = 0; i < _initialCount; i++)
            {
                _pool.Enqueue(InstantiateGameObject());
            }
        }

        private void GenerateBatch() {
            for (var i = 0; i < _batchSize; i++)
            {
                _pool.Enqueue(InstantiateGameObject());
            }
        }

        private T InstantiateGameObject() {
            T newComp = _poolParent ? base.Instantiate(_poolParent, false) : base.Instantiate();
            newComp.gameObject.SetActive(false);
 
            if(newComp is IInstanceObject<T> poolObj) {
                poolObj.Provider = this;
            }
            else {
                poolObj = newComp.GetComponent<IInstanceObject<T>>();
                if(poolObj != null) {
                    poolObj.Provider = this;
                }
            }
            return newComp;
        }

        protected override T Instantiate(Vector3 position, Quaternion rotation, bool setPositionAndRotation, Transform parent, bool worldPositionStays) {

            if(_pool.Count == 0) {
                GenerateBatch();
            }

            T comp = _pool.Dequeue();
            if(_parent) comp.transform.SetParent(parent, worldPositionStays);
            if (setPositionAndRotation) comp.transform.SetPositionAndRotation(position, rotation);
            comp.gameObject.SetActive(true);
            RaiseOnInstancedEvent(comp);
            return comp;
        }

        public override void Destroy(T comp)
        {
            if(!_pool.Contains(comp)) {
                RaiseOnDestroyEvent(comp);
                comp.gameObject.SetActive(false);
                if(_poolParent) comp.transform.SetParent(_poolParent, _worldPositionStays);
                _pool.Enqueue(comp);
            }
        }
    }
}

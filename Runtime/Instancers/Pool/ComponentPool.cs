using System.Collections.Generic;
using UnityEngine;

namespace d4160.Instancers
{
    public class ComponentPool<T> : ComponentFactory<T>, IObjectPool<T> where T : Component
    {
        protected Transform _parent;
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
            T newComp = base.Instantiate();
            newComp.gameObject.SetActive(false);
            if(_parent) newComp.transform.SetParent(_parent);
            
            if(newComp is IPoolableObject<T> poolObj) {
                poolObj.Pool = this;
            }
            else {
                poolObj = newComp.GetComponent<IPoolableObject<T>>();
                if(poolObj != null) {
                    poolObj.Pool = this;
                }
            }
            return newComp;
        }

        public override T Instantiate()
        {
            if(_pool.Count == 0) {
                GenerateBatch();
            }

            T comp = _pool.Dequeue();
            if(_parent) comp.transform.SetParent(null);
            comp.gameObject.SetActive(true);
            return comp;
        }

        public override void Destroy(T instance)
        {
            if(!_pool.Contains(instance)) {
                instance.gameObject.SetActive(false);
                if(_parent) instance.transform.SetParent(_parent);
                _pool.Enqueue(instance);
            }
        }
    }
}

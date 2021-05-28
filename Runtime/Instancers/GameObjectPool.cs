using System.Collections.Generic;
using UnityEngine;

namespace d4160.Instancers
{
    public class GameObjectPool : GameObjectFactory, IObjectPool<GameObject>
    {
        protected Transform _parent;
        protected int _initialCount = 0;
        [Tooltip("When there is no more items in the pool, the next numbers of items to instantiate")]
        protected int _batchSize = 1;

        protected Queue<GameObject> _pool = new Queue<GameObject>();

        public GameObjectPool(int initialCount, int batchSize, Transform parent)
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

        private GameObject InstantiateGameObject() {
            GameObject newGo = base.Instantiate();
            newGo.SetActive(false);
            if(_parent) newGo.transform.SetParent(_parent);

            var poolObj = newGo.GetComponent<IPoolObject<GameObject>>();
            if(poolObj != null) {
                poolObj.Pool = this;
            }

            return newGo;
        }

        public override GameObject Instantiate()
        {
            if(_pool.Count == 0) {
                GenerateBatch();
            }

            GameObject go = _pool.Dequeue();
            if(_parent) go.transform.SetParent(null);
            go.SetActive(true);
            return go;
        }

        public override void Destroy(GameObject instance)
        {
            if(!_pool.Contains(instance)) {
                instance.SetActive(false);
                if(_parent) instance.transform.SetParent(_parent);
                _pool.Enqueue(instance);
            }
        }
    }
}

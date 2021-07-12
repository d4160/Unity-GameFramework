using System.Collections.Generic;
using UnityEngine;

namespace d4160.Instancers
{
    public class GameObjectPool : GameObjectFactory, IObjectPool<GameObject>
    {
        protected Transform _poolParent;

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
            GameObject newGo = _poolParent ? base.Instantiate(_poolParent, false) : base.Instantiate();
            newGo.SetActive(false);

            var poolObj = newGo.GetComponent<IPoolableObject<GameObject>>();
            if(poolObj != null) {
                poolObj.Pool = this;
            }

            return newGo;
        }

        protected override GameObject Instantiate(Vector3 position, Quaternion rotation, bool setPositionAndRotation, Transform parent, bool worldPositionStays) {

            if(_pool.Count == 0) {
                GenerateBatch();
            }

            GameObject newGo = _pool.Dequeue();
            if(_parent) newGo.transform.SetParent(parent, worldPositionStays);
            if (setPositionAndRotation) newGo.transform.SetPositionAndRotation(position, rotation);
            newGo.SetActive(true);
            InvokeOnInstancedEvent(newGo);
            return newGo;
        }

        public override void Destroy(GameObject go)
        {
            if(!_pool.Contains(go)) {
                InvokeOnDestroyEvent(go);
                go.SetActive(false);
                if(_poolParent) go.transform.SetParent(_poolParent, _worldPositionStays);
                _pool.Enqueue(go);
            }
        }
    }
}

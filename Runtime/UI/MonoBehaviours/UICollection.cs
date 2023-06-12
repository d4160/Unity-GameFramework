using d4160.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.UIs
{
    public abstract class UICollection<TCollec, TElem, TData> : UICollection where TElem : UIElement<TCollec, TData> where TCollec : UICollection<TCollec, TElem, TData>
    {
        [SerializeField] protected TElem _prefab;

        protected List<TElem> _items = new();
        private int _nextIndex = 0;

        protected Queue<TElem> _stack = new();

        protected override void Awake()
        {
            base.Awake();

            if (_prefab.gameObject.activeSelf)
                _prefab.gameObject.SetActive(false);
        }

        public TElem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public void AddOrUpdateElements(IList<TData> data, Transform parent = null, bool disableAllFirst = true)
        {
            if (disableAllFirst)
                DisableAllInstances();

            if (parent == null)
                parent = _parent;

            int nextI = disableAllFirst ? 0 : _items.Count;

            for (int i = 0; i < data.Count; i++, nextI++)
            {
                AddOrUpdateElement(data[i], parent);
            }
        }

        public TElem AddOrUpdateElement(TData data, Transform parent = null)
        {
            if (parent == null)
                parent = _parent;

            TElem instance = InstantiatePrefab(_nextIndex, parent);
            instance.Collection = this as TCollec;
            instance.SetData(data);

            _nextIndex = _items.Count;

            AddOrUpdateElementInternal(instance, data);

            return instance;
        }

        protected virtual void AddOrUpdateElementInternal(TElem instance, TData data)
        {
        }

        protected TElem InstantiatePrefab(int index, Transform parent)
        {
            TElem instance;
            if (_items.IsValidIndex(index)) // to reuse and override without exchange between stack and list
            {
                _items[index].gameObject.SetActive(true);
                _items[index].SetInteractable(true);
                instance = _items[index];
            }
            else
            {
                instance = InstantiatePrefab(parent);
            }
            instance.Index = index;
            return instance;
        }

        protected TElem InstantiatePrefab(TElem instance, Transform parent)
        {
            if (_items.Contains(instance))
            {
                instance.gameObject.SetActive(true);
                return instance;
            }

            return InstantiatePrefab(parent);
        }

        protected TElem InstantiatePrefab(Transform parent)
        {
            TElem instance = _stack.Count > 0 ? _stack.Dequeue() : Instantiate(_prefab, parent);
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            instance.gameObject.SetActive(true);
            instance.SetInteractable(true);
            _items.Add(instance);
            return instance;
        }

        public void DisableAllInstances()
        {
            _nextIndex = 0;
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                _items[i].gameObject.SetActive(false);
                _items[i].transform.SetAsLastSibling();
                _stack.Enqueue(_items[i]);
                _items.RemoveAt(i);
            }
        }

        public void DisableInstance(TElem elem)
        {
            if (_items.IsValidIndex(elem.Index) && _items[elem.Index].gameObject.activeSelf)
            {
                _items[elem.Index].gameObject.SetActive(false);
                _items[elem.Index].transform.SetAsLastSibling();
                _stack.Enqueue(_items[elem.Index]);
                _items.RemoveAt(elem.Index);

                // Fix indexes
                for (int i = elem.Index; i < _items.Count; i++)
                {
                    _items[i].Index = i;
                }

                _nextIndex = _items.Count;

                //for (int i = 0; i < _items.Count; i++)
                //{
                //    Debug.Log($"i:{i}, Index:{_items[i].Index}, Data:{_items[i].Data}");
                //}
            }
        }

        public void Swap(int fromIdx, int toIdx)
        {
            if (_items.IsValidIndex(fromIdx) && _items.IsValidIndex(toIdx))
            {
                _items[fromIdx].Swap(_items[toIdx]);
            }
        }

        public void SetInteractable(bool interactable)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].gameObject.activeSelf)
                {
                    _items[i].SetInteractable(interactable);
                }
            }

            SetInteractableInternal(interactable);
        }

        protected virtual void SetInteractableInternal(bool interactable) { }
    }

    public abstract class UICollection : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Transform _parent;

        protected virtual void Awake()
        {
            for (int i = 0; i < _parent.childCount; i++)
            {
                _parent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace d4160.Core.MonoBehaviours
{
    public abstract class UnityObjectList<T> : UnityObjectList, IList<T> where T : Object
    {
        [BoxGroup("List")]
        [SerializeField] protected List<T> _items = new List<T>();

        protected int _currentItemIndexToUpdate = 0;

        public override void CheckItemsFromContainer()
        {
            for (int i = _itemsContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = _itemsContainer.GetChild(i);

                switch (_currentContainerRecicleOption)
                {
                    case RecicleOption.None:
                        break;

                    case RecicleOption.Add:
                        if (typeof(T) == typeof(GameObject))
                        {
                            Add(child.gameObject as T);
                            child.gameObject.SetActive(true);
                        }
                        else
                        {
                            var tMono = child.GetComponent<T>();
                            if (tMono)
                            {
                                Add(tMono);
                                child.gameObject.SetActive(true);
                            }
                            else
                            {
                                DestroyUnityObject(child);
                            }
                        }
                        break;

                    case RecicleOption.Destroy:
                        DestroyUnityObject(child);
                        break;

                    case RecicleOption.Reuse:
                        if (typeof(T) == typeof(GameObject))
                        {
                            RemoveUnityObject(child.gameObject as T);
                        }
                        else
                        {
                            RemoveUnityObject(child.GetComponent<T>());
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        protected void DestroyUnityObject(Transform t)
        {
            if (typeof(T) == typeof(GameObject))
            {
                _items.Remove(t.gameObject as T);
            }
            else
            {
                _items.Remove(t.GetComponent<T>());
            }

            DestroyImmediate(t.gameObject);
        }

        protected void RemoveUnityObject(T item)
        {
            switch (_provider)
            {
                case IObjectDisposer<T> tDisposer:
                    tDisposer.Despawn(item);
                    break;
                case IObjectDisposer<GameObject> goDisposer:
                    switch (item)
                    {
                        case GameObject go:
                            goDisposer.Despawn(go);
                            break;
                        case Component c:
                            goDisposer.Despawn(c.gameObject);
                            break;
                    }
                    break;
            }
        }

        public virtual T AddObject(Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            if (_provider is IUnityObjectProvider<GameObject> goProvider)
            {
                GameObject go = goProvider.Spawn(position, rotation, parent ?? _itemsContainer, worldPositionStays);

                if (_normalizeScale)
                {
                    go.transform.localScale = Vector3.one;
                }

                if (typeof(T) == typeof(GameObject))
                {
                    Add(go as T);

                    return go as T;
                }
                else
                {
                    T newT = go.GetComponent<T>();
                    if (newT)
                    {
                        Add(newT);

                        return newT;
                    }
                }
            }
            else if (_provider is IUnityObjectProvider<T> tProvider)
            {
                T newT = tProvider.Spawn(position, rotation, parent ?? _itemsContainer, worldPositionStays);
                Add(newT);

                if (_normalizeScale)
                {
                    if (newT is Component c)
                    {
                        c.transform.localScale = Vector3.one;
                    }
                }

                return newT;
            }

            return null;
        }

        public virtual T AddObject()
        {
            return AddObject(Vector3.zero, Quaternion.identity, null, false);
        }

        public virtual T AddOrUpdateObject(Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            T item;
            if (_items.IsValidIndex(_currentItemIndexToUpdate))
            {
                item = _items[_currentItemIndexToUpdate];

                switch (item)
                {
                    case GameObject go:
                        if (_normalizeScale)
                        {
                            go.transform.localScale = Vector3.one;
                        }

                        go.SetActive(true);
                        break;
                    case Component c:
                        if (_normalizeScale)
                        {
                            c.transform.localScale = Vector3.one;
                        }
                        c.gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                item = AddObject(position, rotation, parent, worldPositionStays);
            }

            _currentItemIndexToUpdate++;

            return item;
        }

        public virtual T AddOrUpdateObject()
        {
            return AddOrUpdateObject(Vector3.zero, Quaternion.identity, null, false);
        }

        public void DisableOtherAfterUpdate()
        {
            for (int i = _currentItemIndexToUpdate; i < Count; i++)
            {
                switch (this[i])
                {
                    case GameObject go:
                        go.SetActive(false);
                        break;
                    case Component c:
                        c.gameObject.SetActive(false);
                        break;
                }
            }
        }

        public void ResetUpdateIndex()
        {
            _currentItemIndexToUpdate = 0;
        }

        #region List Methods
        /// <summary>
        /// Gets and sets the capacity of this list.  The capacity is the size of
        /// the internal array used to hold items.  When set, the internal 
        /// Memory of the list is reallocated to the given capacity.
        /// Note that the return value for this property may be larger than the property was set to.
        /// </summary>
        public int Capacity
        {
            get => _items.Capacity;
            set => _items.Capacity = value;
        }

        /// <summary>
        /// Read-only property describing how many elements are in the List.
        /// </summary>
        public int Count => _items.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element at the given index.
        /// </summary>
        public T this[int index]
        {
            get => _items.IsValidIndex(index) ? _items[index] : null;

            set
            {
                if (_items.IsValidIndex(index))
                {
                    _items[index] = value;
                }
            }
        }

        /// <summary>
        /// Adds the given object to the end of this list. The size of the list is
        /// increased by one. If required, the capacity of the list is doubled
        /// before adding the new element.
        /// </summary>
        public void Add(T item)
        {
            if (_allowDuplicateItems)
            {
                _items.Add(item);
            }
            else
            {
                if (!Contains(item))
                    _items.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of the given collection to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            if (_allowDuplicateItems)
            {
                _items.AddRange(collection);
            }
            else
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Add(enumerator.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the elements of the given array to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public void AddRange(T[] array)
        {
            if (_allowDuplicateItems)
            {
                _items.AddRange(array);
            }
            else
            {
                for (var i = 0; i < array.Length; i++)
                {
                    Add(array[i]);
                }
            }
        }

        /// <summary>
        /// Searches a section of the list for a given element using a binary search
        /// algorithm. 
        /// </summary>
        /// 
        /// <remarks><para>Elements of the list are compared to the search value using
        /// the given IComparer interface. If comparer is null, elements of
        /// the list are compared to the search value using the IComparable
        /// interface, which in that case must be implemented by all elements of the
        /// list and the given search value. This method assumes that the given
        /// section of the list is already sorted; if this is not the case, the
        /// result will be incorrect.</para>
        ///
        /// <para>The method returns the index of the given value in the list. If the
        /// list does not contain the given value, the method returns a negative
        /// integer. The bitwise complement operator (~) can be applied to a
        /// negative result to produce the index of the first element (if any) that
        /// is larger than the given search value. This is also the index at which
        /// the search value should be inserted into the list in order for the list
        /// to remain sorted.
        /// </para></remarks>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _items.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="IComparable{T}"/>
        /// then that is used for comparison, otherwise <see cref="Comparer{T}.Default"/> is used.
        /// </summary>
        public int BinarySearch(T item)
            => BinarySearch(0, Count, item, null);

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="IComparable{T}"/>
        /// then that is used for comparison, otherwise <see cref="Comparer{T}.Default"/> is used.
        /// </summary>
        public int BinarySearch(T item, IComparer<T> comparer)
            => BinarySearch(0, Count, item, comparer);

        /// <summary>
        /// Clears the contents of the UnityObjectList.
        /// </summary>
        public override void Clear()
        {
            switch (_provider)
            {
                case IObjectDisposer<T> tDisposer:
                    for (int i = 0; i < Count; i++)
                    {
                        tDisposer.Despawn(_items[i]);
                    }
                    break;
                case IObjectDisposer<GameObject> goDisposer:
                    for (int i = 0; i < Count; i++)
                    {
                        switch (_items[i])
                        {
                            case GameObject go:
                                goDisposer.Despawn(go);
                                break;
                            case Component c:
                                goDisposer.Despawn(c.gameObject);
                                break;
                        }
                    }
                    break;
            }

            _items.Clear();
        }

        /// <summary>
        /// Contains returns true if the specified element is in the List.
        /// It does a linear, O(n) search.  Equality is determined by calling
        /// EqualityComparer{T}.Default.Equals.
        /// </summary>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public bool Exists(Predicate<T> match)
            => _items.Exists(match);

        public List<T> FindAll(Predicate<T> match)
        {
            return _items.FindAll(match);
        }

        public int FindIndex(Predicate<T> match)
            => FindIndex(0, Count, match);

        public int FindIndex(int startIndex, Predicate<T> match)
            => FindIndex(startIndex, Count - startIndex, match);

        public int FindIndex(int startIndex, int count, Predicate<T> match)
            => _items.FindIndex(startIndex, count, match);

        public int FindLastIndex(Predicate<T> match)
            => FindLastIndex(Count - 1, Count, match);

        public int FindLastIndex(int startIndex, Predicate<T> match)
            => FindLastIndex(startIndex, startIndex + 1, match);

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
            => _items.FindLastIndex(startIndex, count, match);

        public void ForEach(Action<T> action)
            => _items.ForEach(action);

        /// <summary>
        /// Returns an enumerator for this list with the given
        /// permission for removal of elements. If modifications made to the list 
        /// while an enumeration is in progress, the MoveNext and 
        /// GetObject methods of the enumerator will throw an exception.
        /// </summary>
        public List<T>.Enumerator GetEnumerator()
            => _items.GetEnumerator();

        /// <summary>
        /// Returns the index of the first occurrence of a given value in
        /// this list. The list is searched forwards from beginning to end.
        /// </summary>
        public int IndexOf(T item)
            => IndexOf(item, 0, Count);

        /// <summary>
        /// Returns the index of the first occurrence of a given value in a range of
        /// this list. The list is searched forwards, starting at index
        /// index and ending at count number of elements. 
        /// </summary>
        public int IndexOf(T item, int index)
            => IndexOf(item, index, Count);

        /// <summary>
        /// Returns the index of the first occurrence of a given value in a range of
        /// this list. The list is searched forwards, starting at index
        /// index and upto count number of elements. 
        /// </summary>
        public int IndexOf(T item, int index, int count)
            => _items.IndexOf(item, index, count);

        /// <summary>
        /// Inserts an element into this list at a given index. The size of the list
        /// is increased by one. If required, the capacity of the list is doubled
        /// before inserting the new element.
        /// </summary>
        public void Insert(int index, T item)
        {
            if (_allowDuplicateItems)
            {
                _items.Insert(index, item);
            }
            else
            {
                if (!Contains(item))
                    _items.Insert(index, item);
            }
        }

        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (_allowDuplicateItems)
            {
                _items.InsertRange(index, collection);
            }
            else
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    int i = index;
                    while (enumerator.MoveNext())
                    {
                        T current = enumerator.Current;
                        if (!Contains(current))
                        {
                            _items.Insert(i, current);
                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, T[] array)
        {
            if (_allowDuplicateItems)
            {
                _items.InsertRange(index, array);
            }
            else
            {
                int idx = index;
                for (var i = 0; i < array.Length; i++)
                {
                    if (!Contains(array[i]))
                    {
                        _items.Insert(idx, array[i]);
                        idx++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at the end 
        /// and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item)
            => LastIndexOf(item, 0);

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item, int index)
            => LastIndexOf(item, index, Count);

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and upto count elements
        /// </summary>
        public int LastIndexOf(T item, int index, int count)
            => _items.LastIndexOf(item, index, count);

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        public bool Remove(T item)
        {
            if (!item) return false;

            RemoveUnityObject(item);

            return _items.Remove(item);
        }

        /// <summary>
        /// This method removes all items which match the predicate.
        /// The complexity is O(n).
        /// </summary>
        public int RemoveAll(Predicate<T> match)
            => _items.RemoveAll(match);

        /// <summary>
        /// Removes the element at the given index. The size of the list is
        /// decreased by one.
        /// </summary>
        public void RemoveAt(int index)
        {
            if (_items.IsValidIndex(index))
            {
                RemoveUnityObject(_items[index]);
            }

            _items.RemoveAt(index);
        }

        /// <summary>
        /// Removes a range of elements from this list.
        /// </summary>
        public void RemoveRange(int index, int count)
        {
            switch (_provider)
            {
                case IObjectDisposer<T> tDisposer:
                    for (int i = index; i < count; i++)
                    {
                        if (_items.IsValidIndex(i))
                        {
                            tDisposer.Despawn(_items[i]);
                        }
                    }
                    break;
                case IObjectDisposer<GameObject> goDisposer:
                    for (int i = index; i < count; i++)
                    {
                        if (_items.IsValidIndex(i))
                        {
                            switch (_items[i])
                            {
                                case GameObject go:
                                    goDisposer.Despawn(go);
                                    break;
                                case Component c:
                                    goDisposer.Despawn(c.gameObject);
                                    break;
                            }
                        }
                    }
                    break;
            }

            _items.RemoveRange(index, count);
        }

        /// <summary>
        /// Reverses the elements in this list.
        /// </summary>
        public void Reverse()
            => Reverse(0, Count);

        /// <summary>
        /// Reverses the elements in a range of this list. Following a call to this
        /// method, an element in the range given by index and count
        /// which was previously located at index i will now be located at
        /// index index + (index + count - i - 1).
        /// </summary>
        public void Reverse(int index, int count) => _items.Reverse(index, count);

        /// <summary>
        /// Sorts the elements in this list.  Uses the default comparer and 
        /// Array.Sort.
        /// </summary>
        public void Sort()
            => Sort(0, Count, null);

        /// <summary>
        /// Sorts the elements in this list.  Uses Array.Sort with the
        /// provided comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
            => Sort(0, Count, comparer);

        /// <summary>
        /// Sorts the elements in a section of this list. The sort compares the
        /// elements to each other using the given IComparer interface. If
        /// comparer is null, the elements are compared to each other using
        /// the IComparable interface, which in that case must be implemented by all
        /// elements of the list.
        /// 
        /// This method uses the Array.Sort method to sort the elements.
        /// </summary>
        public void Sort(int index, int count, IComparer<T> comparer)
            => _items.Sort(index, count, comparer);

        public void Sort(Comparison<T> comparison)
            => _items.Sort(comparison);

        /// <summary>
        /// ToArray returns an array containing the contents of the List.
        /// This requires copying the List, which is an O(n) operation.
        /// </summary>
        public T[] ToArray()
            => _items.ToArray();

        /// <summary>
        /// Sets the capacity of this list to the size of the list. This method can
        /// be used to minimize a list's memory overhead once it is known that no
        /// new elements will be added to the list. To completely clear a list and
        /// release all memory referenced by the list, execute the following
        /// statements:
        /// <code>
        /// list.Clear();
        /// list.TrimExcess();
        /// </code>
        /// </summary>
        public void TrimExcess() => _items.TrimExcess();

        public bool TrueForAll(Predicate<T> match)
            => _items.TrueForAll(match);

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        #endregion
    }

    public abstract class UnityObjectListSingleton<S, T> : UnityObjectListSingleton<S>, IList<T> where T : Object where S : MonoBehaviour
    {
        [BoxGroup("List")]
        [SerializeField] protected List<T> _items = new List<T>();

        protected int _currentItemIndexToUpdate = 0;

        public override void CheckItemsFromContainer()
        {
            for (int i = _itemsContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = _itemsContainer.GetChild(i);

                switch (_currentContainerRecicleOption)
                {
                    case RecicleOption.None:
                        break;

                    case RecicleOption.Add:
                        if (typeof(T) == typeof(GameObject))
                        {
                            Add(child.gameObject as T);
                            child.gameObject.SetActive(true);
                        }
                        else
                        {
                            var tMono = child.GetComponent<T>();
                            if (tMono)
                            {
                                Add(tMono);
                                child.gameObject.SetActive(true);
                            }
                            else
                            {
                                DestroyUnityObject(child);
                            }
                        }
                        break;

                    case RecicleOption.Destroy:
                        DestroyUnityObject(child);
                        break;

                    case RecicleOption.Reuse:
                        if (typeof(T) == typeof(GameObject))
                        {
                            RemoveUnityObject(child.gameObject as T);
                        }
                        else
                        {
                            RemoveUnityObject(child.GetComponent<T>());
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        protected void DestroyUnityObject(Transform t)
        {
            if (typeof(T) == typeof(GameObject))
            {
                _items.Remove(t.gameObject as T);
            }
            else
            {
                _items.Remove(t.GetComponent<T>());
            }

            DestroyImmediate(t.gameObject);
        }

        protected void RemoveUnityObject(T item)
        {
            switch (_provider)
            {
                case IObjectDisposer<T> tDisposer:
                    tDisposer.Despawn(item);
                    break;
                case IObjectDisposer<GameObject> goDisposer:
                    switch (item)
                    {
                        case GameObject go:
                            goDisposer.Despawn(go);
                            break;
                        case Component c:
                            goDisposer.Despawn(c.gameObject);
                            break;
                    }
                    break;
            }
        }

        public virtual T AddObject(Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            if (_provider is IUnityObjectProvider<GameObject> goProvider)
            {
                GameObject go = goProvider.Spawn(position, rotation, parent ?? _itemsContainer, worldPositionStays);

                if (_normalizeScale)
                {
                    go.transform.localScale = Vector3.one;
                }

                if (typeof(T) == typeof(GameObject))
                {
                    Add(go as T);

                    return go as T;
                }
                else
                {
                    T newT = go.GetComponent<T>();
                    if (newT)
                    {
                        Add(newT);

                        return newT;
                    }
                }
            }
            else if (_provider is IUnityObjectProvider<T> tProvider)
            {
                T newT = tProvider.Spawn(position, rotation, parent ?? _itemsContainer, worldPositionStays);
                Add(newT);

                if (_normalizeScale)
                {
                    if (newT is Component c)
                    {
                        c.transform.localScale = Vector3.one;
                    }
                }

                return newT;
            }

            return null;
        }

        public virtual T AddObject()
        {
            return AddObject(Vector3.zero, Quaternion.identity, null, false);
        }

        public virtual T AddOrUpdateObject(Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            T item;
            if (_items.IsValidIndex(_currentItemIndexToUpdate))
            {
                item = _items[_currentItemIndexToUpdate];

                switch (item)
                {
                    case GameObject go:
                        if (_normalizeScale)
                        {
                            go.transform.localScale = Vector3.one;
                        }

                        go.SetActive(true);
                        break;
                    case Component c:
                        if (_normalizeScale)
                        {
                            c.transform.localScale = Vector3.one;
                        }
                        c.gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                item = AddObject(position, rotation, parent, worldPositionStays);
            }

            _currentItemIndexToUpdate++;

            return item;
        }

        public virtual T AddOrUpdateObject()
        {
            return AddOrUpdateObject(Vector3.zero, Quaternion.identity, null, false);
        }

        public void DisableOtherAfterUpdate()
        {
            for (int i = _currentItemIndexToUpdate; i < Count; i++)
            {
                switch (this[i])
                {
                    case GameObject go:
                        go.SetActive(false);
                        break;
                    case Component c:
                        c.gameObject.SetActive(false);
                        break;
                }
            }
        }

        public void ResetUpdateIndex()
        {
            _currentItemIndexToUpdate = 0;
        }

        #region List Methods
        /// <summary>
        /// Gets and sets the capacity of this list.  The capacity is the size of
        /// the internal array used to hold items.  When set, the internal 
        /// Memory of the list is reallocated to the given capacity.
        /// Note that the return value for this property may be larger than the property was set to.
        /// </summary>
        public int Capacity
        {
            get => _items.Capacity;
            set => _items.Capacity = value;
        }

        /// <summary>
        /// Read-only property describing how many elements are in the List.
        /// </summary>
        public int Count => _items.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element at the given index.
        /// </summary>
        public T this[int index]
        {
            get => _items.IsValidIndex(index) ? _items[index] : null;

            set
            {
                if (_items.IsValidIndex(index))
                {
                    _items[index] = value;
                }
            }
        }

        /// <summary>
        /// Adds the given object to the end of this list. The size of the list is
        /// increased by one. If required, the capacity of the list is doubled
        /// before adding the new element.
        /// </summary>
        public void Add(T item)
        {
            if (_allowDuplicateItems)
            {
                _items.Add(item);
            }
            else
            {
                if (!Contains(item))
                    _items.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of the given collection to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            if (_allowDuplicateItems)
            {
                _items.AddRange(collection);
            }
            else
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Add(enumerator.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the elements of the given array to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public void AddRange(T[] array)
        {
            if (_allowDuplicateItems)
            {
                _items.AddRange(array);
            }
            else
            {
                for (var i = 0; i < array.Length; i++)
                {
                    Add(array[i]);
                }
            }
        }

        /// <summary>
        /// Searches a section of the list for a given element using a binary search
        /// algorithm. 
        /// </summary>
        /// 
        /// <remarks><para>Elements of the list are compared to the search value using
        /// the given IComparer interface. If comparer is null, elements of
        /// the list are compared to the search value using the IComparable
        /// interface, which in that case must be implemented by all elements of the
        /// list and the given search value. This method assumes that the given
        /// section of the list is already sorted; if this is not the case, the
        /// result will be incorrect.</para>
        ///
        /// <para>The method returns the index of the given value in the list. If the
        /// list does not contain the given value, the method returns a negative
        /// integer. The bitwise complement operator (~) can be applied to a
        /// negative result to produce the index of the first element (if any) that
        /// is larger than the given search value. This is also the index at which
        /// the search value should be inserted into the list in order for the list
        /// to remain sorted.
        /// </para></remarks>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _items.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="IComparable{T}"/>
        /// then that is used for comparison, otherwise <see cref="Comparer{T}.Default"/> is used.
        /// </summary>
        public int BinarySearch(T item)
            => BinarySearch(0, Count, item, null);

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="IComparable{T}"/>
        /// then that is used for comparison, otherwise <see cref="Comparer{T}.Default"/> is used.
        /// </summary>
        public int BinarySearch(T item, IComparer<T> comparer)
            => BinarySearch(0, Count, item, comparer);

        /// <summary>
        /// Clears the contents of the UnityObjectList.
        /// </summary>
        public override void Clear()
        {
            switch (_provider)
            {
                case IObjectDisposer<T> tDisposer:
                    for (int i = 0; i < Count; i++)
                    {
                        tDisposer.Despawn(_items[i]);
                    }
                    break;
                case IObjectDisposer<GameObject> goDisposer:
                    for (int i = 0; i < Count; i++)
                    {
                        switch (_items[i])
                        {
                            case GameObject go:
                                goDisposer.Despawn(go);
                                break;
                            case Component c:
                                goDisposer.Despawn(c.gameObject);
                                break;
                        }
                    }
                    break;
            }

            _items.Clear();
        }

        /// <summary>
        /// Contains returns true if the specified element is in the List.
        /// It does a linear, O(n) search.  Equality is determined by calling
        /// EqualityComparer{T}.Default.Equals.
        /// </summary>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public bool Exists(Predicate<T> match)
            => _items.Exists(match);

        public List<T> FindAll(Predicate<T> match)
        {
            return _items.FindAll(match);
        }

        public int FindIndex(Predicate<T> match)
            => FindIndex(0, Count, match);

        public int FindIndex(int startIndex, Predicate<T> match)
            => FindIndex(startIndex, Count - startIndex, match);

        public int FindIndex(int startIndex, int count, Predicate<T> match)
            => _items.FindIndex(startIndex, count, match);

        public int FindLastIndex(Predicate<T> match)
            => FindLastIndex(Count - 1, Count, match);

        public int FindLastIndex(int startIndex, Predicate<T> match)
            => FindLastIndex(startIndex, startIndex + 1, match);

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
            => _items.FindLastIndex(startIndex, count, match);

        public void ForEach(Action<T> action)
            => _items.ForEach(action);

            /// <summary>
        /// Returns an enumerator for this list with the given
        /// permission for removal of elements. If modifications made to the list 
        /// while an enumeration is in progress, the MoveNext and 
        /// GetObject methods of the enumerator will throw an exception.
        /// </summary>
        public List<T>.Enumerator GetEnumerator()
            => _items.GetEnumerator();

        /// <summary>
        /// Returns the index of the first occurrence of a given value in
        /// this list. The list is searched forwards from beginning to end.
        /// </summary>
        public int IndexOf(T item)
            => IndexOf(item, 0, Count);

        /// <summary>
        /// Returns the index of the first occurrence of a given value in a range of
        /// this list. The list is searched forwards, starting at index
        /// index and ending at count number of elements. 
        /// </summary>
        public int IndexOf(T item, int index)
            => IndexOf(item, index, Count);

        /// <summary>
        /// Returns the index of the first occurrence of a given value in a range of
        /// this list. The list is searched forwards, starting at index
        /// index and upto count number of elements. 
        /// </summary>
        public int IndexOf(T item, int index, int count)
            => _items.IndexOf(item, index, count);

        /// <summary>
        /// Inserts an element into this list at a given index. The size of the list
        /// is increased by one. If required, the capacity of the list is doubled
        /// before inserting the new element.
        /// </summary>
        public void Insert(int index, T item)
        {
            if (_allowDuplicateItems)
            {
                _items.Insert(index, item);
            }
            else
            {
                if (!Contains(item))
                    _items.Insert(index, item);
            }
        }

        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (_allowDuplicateItems)
            {
                _items.InsertRange(index, collection);
            }
            else
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    int i = index;
                    while (enumerator.MoveNext())
                    {
                        T current = enumerator.Current;
                        if (!Contains(current))
                        {
                            _items.Insert(i, current);
                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, T[] array)
        {
            if (_allowDuplicateItems)
            {
                _items.InsertRange(index, array);
            }
            else
            {
                int idx = index;
                for (var i = 0; i < array.Length; i++)
                {
                    if (!Contains(array[i]))
                    {
                        _items.Insert(idx, array[i]);
                        idx++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at the end 
        /// and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item)
            => LastIndexOf(item, 0);

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item, int index)
            => LastIndexOf(item, index, Count);

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and upto count elements
        /// </summary>
        public int LastIndexOf(T item, int index, int count)
            => _items.LastIndexOf(item, index, count);

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        public bool Remove(T item)
        {
            if (!item) return false;

            RemoveUnityObject(item);

            return _items.Remove(item);
        }

        /// <summary>
        /// This method removes all items which match the predicate.
        /// The complexity is O(n).
        /// </summary>
        public int RemoveAll(Predicate<T> match)
            => _items.RemoveAll(match);

        /// <summary>
        /// Removes the element at the given index. The size of the list is
        /// decreased by one.
        /// </summary>
        public void RemoveAt(int index)
        {
            if (_items.IsValidIndex(index))
            {
                RemoveUnityObject(_items[index]);
            }

            _items.RemoveAt(index);
        }

        /// <summary>
        /// Removes a range of elements from this list.
        /// </summary>
        public void RemoveRange(int index, int count)
        {
            switch (_provider)
            {
                case IObjectDisposer<T> tDisposer:
                    for (int i = index; i < count; i++)
                    {
                        if (_items.IsValidIndex(i))
                        {
                            tDisposer.Despawn(_items[i]);
                        }
                    }
                    break;
                case IObjectDisposer<GameObject> goDisposer:
                    for (int i = index; i < count; i++)
                    {
                        if (_items.IsValidIndex(i))
                        {
                            switch (_items[i])
                            {
                                case GameObject go:
                                    goDisposer.Despawn(go);
                                    break;
                                case Component c:
                                    goDisposer.Despawn(c.gameObject);
                                    break;
                            }
                        }
                    }
                    break;
            }

            _items.RemoveRange(index, count);
        }

        /// <summary>
        /// Reverses the elements in this list.
        /// </summary>
        public void Reverse()
            => Reverse(0, Count);

        /// <summary>
        /// Reverses the elements in a range of this list. Following a call to this
        /// method, an element in the range given by index and count
        /// which was previously located at index i will now be located at
        /// index index + (index + count - i - 1).
        /// </summary>
        public void Reverse(int index, int count) => _items.Reverse(index, count);

        /// <summary>
        /// Sorts the elements in this list.  Uses the default comparer and 
        /// Array.Sort.
        /// </summary>
        public void Sort()
            => Sort(0, Count, null);

        /// <summary>
        /// Sorts the elements in this list.  Uses Array.Sort with the
        /// provided comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
            => Sort(0, Count, comparer);

        /// <summary>
        /// Sorts the elements in a section of this list. The sort compares the
        /// elements to each other using the given IComparer interface. If
        /// comparer is null, the elements are compared to each other using
        /// the IComparable interface, which in that case must be implemented by all
        /// elements of the list.
        /// 
        /// This method uses the Array.Sort method to sort the elements.
        /// </summary>
        public void Sort(int index, int count, IComparer<T> comparer)
            => _items.Sort(index, count, comparer);

        public void Sort(Comparison<T> comparison)
            => _items.Sort(comparison);

        /// <summary>
        /// ToArray returns an array containing the contents of the List.
        /// This requires copying the List, which is an O(n) operation.
        /// </summary>
        public T[] ToArray()
            => _items.ToArray();

        /// <summary>
        /// Sets the capacity of this list to the size of the list. This method can
        /// be used to minimize a list's memory overhead once it is known that no
        /// new elements will be added to the list. To completely clear a list and
        /// release all memory referenced by the list, execute the following
        /// statements:
        /// <code>
        /// list.Clear();
        /// list.TrimExcess();
        /// </code>
        /// </summary>
        public void TrimExcess() => _items.TrimExcess();

        public bool TrueForAll(Predicate<T> match)
            => _items.TrueForAll(match);

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        #endregion
    }

    public abstract class UnityObjectListSingleton<S, T1, T2> : UnityObjectListSingleton<S, T1> where T1 : Object, IObjectData<T2> where S : MonoBehaviour
    {
        public override void Init()
        {
            if (!_itemsContainer)
                _itemsContainer = transform;

            CheckItemsFromContainer();

            if (_clearListWhenInit)
            {
                Clear();
            }

            if (_spawnWhenInit)
            {
                FillList();
            }
        }

        public new abstract void FillList();

        public virtual T1 AddObjectAndSetData(T2 data, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            T1 instance = AddObject(position, rotation, parent, worldPositionStays);
            instance.SetData(data);

            return instance;
        }

        public virtual T1 AddObjectAndSetData(T2 data)
        {
            T1 instance = AddObject();
            instance.SetData(data);

            return instance;
        }

        public virtual T1 AddOrUpdateObjectAndSetData(T2 data, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            T1 instance = AddOrUpdateObject(position, rotation, parent, worldPositionStays);
            instance.SetData(data);

            return instance;
        }

        public virtual T1 AddOrUpdateObjectAndSetData(T2 data)
        {
            T1 instance = AddOrUpdateObject();
            instance.SetData(data);

            return instance;
        }
    }

    public abstract class UnityObjectList<T1, T2> : UnityObjectList<T1> where T1 : Object, IObjectData<T2>
    {
        public override void Init()
        {
            if (!_itemsContainer)
                _itemsContainer = transform;

            CheckItemsFromContainer();

            if (_clearListWhenInit)
            {
                Clear();
            }

            if (_spawnWhenInit)
            {
                FillList();
            }
        }

        public new abstract void FillList();

        public virtual T1 AddObjectAndSetData(T2 data, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            T1 instance = AddObject(position, rotation, parent, worldPositionStays);
            instance.SetData(data);

            return instance;
        }

        public virtual T1 AddObjectAndSetData(T2 data)
        {
            T1 instance = AddObject();
            instance.SetData(data);

            return instance;
        }

        public virtual T1 AddOrUpdateObjectAndSetData(T2 data, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = true)
        {
            T1 instance = AddOrUpdateObject(position, rotation, parent, worldPositionStays);
            instance.SetData(data);

            return instance;
        }

        public virtual T1 AddOrUpdateObjectAndSetData(T2 data)
        {
            T1 instance = AddOrUpdateObject();
            instance.SetData(data);

            return instance;
        }
    }

    public class UnityObjectListSingleton<S> : Singleton<S> where S : MonoBehaviour
    {
        [BoxGroup("Basic")]
        [SerializeField] protected Transform _itemsContainer;
        [BoxGroup("Basic")]
        [SerializeField] protected RecicleOption _currentContainerRecicleOption = RecicleOption.None;
        [BoxGroup("Basic")]
        [SerializeField] protected bool _allowDuplicateItems = false;
        [BoxGroup("Basic")] 
        [SerializeField] protected bool _normalizeScale = true;
        [BoxGroup("Basic")]
        [RequireType(typeof(IUnityObjectProvider<>), typeof(Object))]
        [SerializeField] protected Object _provider;

        [BoxGroup("Initialization")]
        [SerializeField] protected UnityInitMethod _initMethod;
        [BoxGroup("Initialization")]
        [SerializeField] protected bool _clearListWhenInit = false;
        [BoxGroup("Initialization")]
        [SerializeField] protected bool _spawnWhenInit = true;

        protected override void Awake()
        {
            base.Awake();

            if (_initMethod == UnityInitMethod.Awake)
                Init();
        }

        protected virtual void OnEnable()
        {
            if (_initMethod == UnityInitMethod.OnEnable)
                Init();
        }

        protected virtual void Start()
        {
            if (_initMethod == UnityInitMethod.Start)
                Init();
        }

        [Button]
        public virtual void Init()
        {
            if (!_itemsContainer)
                _itemsContainer = transform;

            CheckItemsFromContainer();

            if (_clearListWhenInit)
            {
                Clear();
            }

            if (_spawnWhenInit)
            {
                FillList();
            }
        }

        public virtual void CheckItemsFromContainer(){}

        public virtual void FillList(){}

        public virtual void Clear(){}
    }

    public class UnityObjectList : MonoBehaviour
    {
        [BoxGroup("Basic")]
        [SerializeField] protected Transform _itemsContainer;
        [BoxGroup("Basic")]
        [SerializeField] protected RecicleOption _currentContainerRecicleOption = RecicleOption.None;
        [BoxGroup("Basic")]
        [SerializeField] protected bool _allowDuplicateItems = false;
        [BoxGroup("Basic")]
        [SerializeField] protected bool _normalizeScale = true;
        [BoxGroup("Basic")]
        [RequireType(typeof(IUnityObjectProvider<>), typeof(Object))]
        [SerializeField] protected Object _provider;

        [BoxGroup("Initialization")]
        [SerializeField] protected UnityInitMethod _initMethod;
        [BoxGroup("Initialization")]
        [SerializeField] protected bool _clearListWhenInit = false;
        [BoxGroup("Initialization")]
        [SerializeField] protected bool _spawnWhenInit = true;

        protected void Awake()
        {
            if (_initMethod == UnityInitMethod.Awake)
                Init();
        }

        protected virtual void OnEnable()
        {
            if (_initMethod == UnityInitMethod.OnEnable)
                Init();
        }

        protected virtual void Start()
        {
            if (_initMethod == UnityInitMethod.Start)
                Init();
        }

        [Button]
        public virtual void Init()
        {
            if (!_itemsContainer)
                _itemsContainer = transform;

            CheckItemsFromContainer();

            if (_clearListWhenInit)
            {
                Clear();
            }

            if (_spawnWhenInit)
            {
                FillList();
            }
        }

        public virtual void CheckItemsFromContainer() { }

        public virtual void FillList() { }

        public virtual void Clear() { }
    }
}

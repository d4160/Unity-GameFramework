using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using InspectInLine;

namespace Malee {
	
	public interface IReorderableObjectOnRemoved
	{
		void OnRemoved(int index);
	}

	public interface IReorderableObjectOnAdded
	{
		void OnAdded(int index);
	}

    /// <summary>
    /// Additionally allow inspecting inline the UnityObject like MonoBehaviours or ScriptableObjects
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[Serializable]
	public abstract class ReorderableArrayForUnityObject<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T> {

		[SerializeField]
		[InspectInline(canEditRemoteTarget = true, canCreateSubasset = true)]
		private List<T> array = new List<T>();

		public ReorderableArrayForUnityObject()
			: this(0) {
		}

		public ReorderableArrayForUnityObject(int length) {

			array = new List<T>(length);
		}

		public T this[int index] {

			get { return array[index]; }
			set { array[index] = value; }
		}

		public int Length {

			get { return array.Count; }
		}

		public bool IsReadOnly {

			get { return false; }
		}

		public int Count {

			get { return array.Count; }
		}

		public object Clone() {

			return new List<T>(array);
		}

		public void CopyFrom(IEnumerable<T> value) {

			array.Clear();
			array.AddRange(value);
		}

		public bool Contains(T value) {

			return array.Contains(value);
		}

		public int IndexOf(T value) {

			return array.IndexOf(value);
		}

		public void Insert(int index, T item) {

			array.Insert(index, item);
		}

		public void RemoveAt(int index) {

			// Debug.Log(index);
			array.RemoveAt(index);
		}

		public void Add(T item) {

			// Debug.Log(item);
			array.Add(item);
		}

		public void Clear() {

			array.Clear();
		}

		public void CopyTo(T[] array, int arrayIndex) {

			this.array.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {

			return array.Remove(item);
		}

		public T[] ToArray() {

			return array.ToArray();
		}

		public IEnumerator<T> GetEnumerator() {

			return array.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {

			return array.GetEnumerator();
		}
	}
}

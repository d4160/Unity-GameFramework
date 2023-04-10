using UnityEngine;

namespace d4160.UIs
{
    public abstract class UIElement<TColl, TData> : UIElement<TData> where TColl : UICollection where TData : class
    {
        public TColl Collection { get; internal set; }
    }

    public abstract class UIElement<TData> : MonoBehaviour where TData : class
    {
        protected TData _data;

        public int Index { get; internal set; }

        public virtual void SetData(TData data)
        {
            _data = data;
            SetDataInternal(_data);
        }

        protected abstract void SetDataInternal(TData data);

        public virtual void SetInteractable(bool interactable)
        {
        }

        public void Swap(UIElement<TData> other)
        {
            var tempIndex = Index;
            var tempData = _data;

            Debug.Log($"[Swap] From, Index: {tempIndex}, Data: {_data}");

            Index = other.Index;
            SetData(other._data);

            Debug.Log($"[Swap] To, Index: {Index}, Data: {_data}");

            other.Index = tempIndex;
            other.SetData(tempData);

            Debug.Log($"[Swap] Other, Index: {other.Index}, Data: {other._data}");
        }
    }
}
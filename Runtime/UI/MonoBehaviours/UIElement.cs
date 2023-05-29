using UnityEngine;

namespace d4160.UIs
{
    public abstract class UIElement<TColl, TData> : UIElement<TData> where TColl : UICollection
    {
        public TColl Collection { get; internal set; }
    }

    public abstract class UIElement<TData> : MonoBehaviour
    {
        protected TData _data;

        public int Index { get; internal set; }

        public TData Data => _data;

        public virtual void SetData(TData data, bool setInternally = true)
        {
            _data = data;
            if (setInternally)
                SetDataInternal(_data);
        }

        protected abstract void SetDataInternal(TData data);

        public virtual void SetInteractable(bool interactable)
        {
        }

        public void Swap(UIElement<TData> other)
        {
            //var tempIndex = Index;
            var tempData = _data;

            //Debug.Log($"[Swap] From, Index: {Index}, Data: {_data}");

            //Index = other.Index;
            SetData(other._data);

            //Debug.Log($"[Swap] To, Index: {other.Index}, Data: {other._data}");

            //other.Index = tempIndex;
            other.SetData(tempData);

            //Debug.Log($"[Swap] Other, Index: {other.Index}, Data: {other._data}");
            //Debug.Log($"[Swap] This, Index: {Index}, Data: {_data}");
        }
    }
}
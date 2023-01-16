using UnityEngine;

namespace d4160.Collections
{
    public abstract class Grid2DSOBase<TRs2D, T> : RuntimeSet2DSOBase<TRs2D, T> where TRs2D : Grid2D<T>, new()
    {
        [SerializeField] protected Vector2 _cellSize;
        [SerializeField] protected Vector3 _originPosition;

        protected override void SetProperties()
        {
            base.SetProperties();

            _runtimeSet.CellSize = _cellSize;
            _runtimeSet.OriginPosition = _originPosition;
        }
    }
}
using System;
using CodeMonkey.Utils;
using d4160.Grid;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Grid
{
    public abstract class GridMono<T> : GridMono
    {
        protected Grid<T> _grid;

        protected override void CreateGrid(bool drawText = false, Color textColor = default, int textFontSize = 5, Transform textParent = null)
        {
            _grid = new Grid<T>(_width, _height, _cellSize, transform.position, drawText, textColor, textFontSize, textParent);
        }

        protected override void DrawGizmos()
        {
            if (_grid == null) CreateGrid();

            _grid.CellSize = _cellSize;
            _grid.OriginPosition = transform.position;
            _grid.DrawGizmos(_gizmosSettings.gridColor, _gizmosSettings.gridDuration);
        }
    }

    public abstract class GridMono : MonoBehaviour 
    {
        [SerializeField] protected int _width = 1;
        [SerializeField] protected int _height = 10;
        [SerializeField] protected Vector2 _cellSize;

        [Space]
        [SerializeField] protected bool _toggleShowGizmosSettings;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_toggleShowGizmosSettings")]
#endif
        [SerializeField] protected GridGizmosSettings _gizmosSettings;

        protected abstract void CreateGrid(bool drawText = false, Color textColor = default, int textFontSize = 5, Transform textParent = null);

        protected virtual void InstantiateProvider(){}

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        [ContextMenu("RecreateGrid")]
        protected void RecreateGrid()
        {
            CreateGrid();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        protected void ReinstantiateProvider()
        {
            InstantiateProvider();
        }

        protected virtual void Start()
        {
            CreateGrid(_gizmosSettings.drawDebugText, _gizmosSettings.textColor, _gizmosSettings.textFontSize, _gizmosSettings.textParent);
        }

        protected virtual void OnDrawGizmos()
        {
            if (_gizmosSettings.gizmosEnabled && !_gizmosSettings.onDrawGizmosSelected)
            {
                DrawGizmos();
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (_gizmosSettings.gizmosEnabled && _gizmosSettings.onDrawGizmosSelected)
            {
                DrawGizmos();
            }
        }

        protected abstract void DrawGizmos();
    }
}
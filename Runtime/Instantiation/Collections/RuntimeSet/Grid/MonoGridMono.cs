using d4160.Instancers;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Grid
{
    public abstract class MonoGridMono<T> : GridMono where T : MonoBehaviour
    {
        [SerializeField] protected T _prefab;
        [Tooltip("The Transform parent to store the instances for this Grid")]
        [SerializeField] protected Transform _parent;
        protected MonoGrid<T> _grid;

        public IProvider<T> Provider { get => _grid?.Provider; set { if (_grid != null) _grid.Provider = value; } }
        public T Prefab => _prefab;

        protected override void CreateGrid(bool drawText = false, Color textColor = default, int textFontSize = 5, Transform textParent = null)
        {
            _grid = new MonoGrid<T>(_width, _height, _cellSize, transform.position, drawText, textColor, textFontSize, textParent);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void FillFromParent() {

            FillFromParent(true, false);
        }

        public void FillFromParent(bool fillHoles) {
            
            FillFromParent(fillHoles, false);
        }

        public void FillFromParent(bool fillHoles, bool forceReplace) {
            
            if(_grid != null) {
                if (_grid.Provider != null)
                {
                    if (_parent)
                    {
                        _grid.Fill(_parent, fillHoles, forceReplace);
                    }
                    else {
                        Debug.LogWarning("Please, select the Parent first.");
                    }
                }
                else {
                    Debug.LogWarning("Please, reinstantiate the Provider first.");
                }
            }
            else {
                Debug.LogWarning("Please, reinstantiate the Grid first.");
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void FillAll()
        {
            FillAll(false);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void FillAllForceReplace()
        {
            FillAll(true);
        }

        public void FillAll(bool forceReplace)
        {
            if(_grid != null) {
                _grid.FillAll(forceReplace);
            }
            else {
                Debug.LogWarning("Please, reinstantiate the Grid first.");
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void DestroyAll()
        {
            if(_grid != null) {
                _grid.DestroyAll();
            }
            else {
                Debug.LogWarning("Please, reinstantiate the Grid first.");
            }
        }

        protected override void DrawGizmos()
        {
            if (_grid == null) CreateGrid();

            _grid.CellSize = _cellSize;
            _grid.OriginPosition = transform.position;
            _grid.DrawGizmos(_gizmosSettings.gridColor, _gizmosSettings.gridDuration);
        }
    }
}
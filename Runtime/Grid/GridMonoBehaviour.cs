using d4160.Instancers;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Grid
{
    public abstract class GridMonoBehaviour<T> : GridBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected T _prefab;
        [Tooltip("The Transform parent to store the instances for this Grid")]
        [SerializeField] protected Transform _parent;
        protected GridMono<T> _grid;

        public IProvider<T> Provider { get => _grid?.Provider; set { if (_grid != null) _grid.Provider = value; } }
        public T Prefab => _prefab;

        protected override void InstantiateGrid(bool drawText = false, Color textColor = default, int textFontSize = 5, Transform textParent = null)
        {
            _grid = new GridMono<T>(_width, _height, _cellSize, transform.position, drawText, textColor, textFontSize, textParent);
        }

        [Button]
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

        [Button]
        public void FillAll()
        {
            FillAll(false);
        }

        [Button]
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

        [Button]
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
            if (_grid == null) InstantiateGrid();

            _grid.CellSize = _cellSize;
            _grid.OriginPosition = transform.position;
            _grid.DrawGizmos(_gizmosSettings.gridColor, _gizmosSettings.gridDuration);
        }
    }
}
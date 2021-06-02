using System;
using System.Collections;
using d4160.Coroutines;
using UnityEngine;

namespace d4160.Grid
{
    public class GridMono<T> : Grid<T> where T: MonoBehaviour
    {
        private Transform _parent;
        private GridMonoProcessValueType _processValueType = GridMonoProcessValueType.None;
        private float _movementSpeedOrDuration;

        public Transform Parent { get => _parent; set => _parent = value; }
        public GridMonoProcessValueType ProcessValueType { get => _processValueType; set => _processValueType = value; }
        /// <summary>
        /// The duration for movement
        /// </summary>
        /// <value></value>
        public float MovementSpeedOrDuration { get => _movementSpeedOrDuration; set => _movementSpeedOrDuration = value; }

        public GridMono(int width, int height, Vector2 cellSize, Vector3 originPosition = default, bool drawDebugText = true, Color? textColor = null, int textFontSize = 5, Transform textParent = null) : base(width, height, cellSize, originPosition, drawDebugText, textColor, textFontSize, textParent)
        {
        }

        protected override T ProcessValue(int x, int y, T value)
        {
            if (value)
            {
                if(_parent) value.transform.SetParent(_parent, false);

                switch(_processValueType) {
                    case GridMonoProcessValueType.None: break;
                    case GridMonoProcessValueType.FixedPosition: 
                        value.transform.position = GetWorldPositionInCenter(x, y);
                        break;
                    case GridMonoProcessValueType.MovementWithDuration:
                        MoveWithDuration(value, x, y, _movementSpeedOrDuration);
                        break;
                    case GridMonoProcessValueType.MovementWithSpeed:
                        MoveWithSpeed(value, x, y, _movementSpeedOrDuration);
                        break;
                }
                
            }
            return value;
        }

        public void Fill(Transform parent, bool fillHoles = true, bool forceReplace = false) {

            if(_provider == null) return;

            int childCount = parent.childCount;

            _iterator = 0;

            if (childCount > 0 || (childCount == 0 && fillHoles))
            {
                int counter = 0;
                for (var x = 0; x < _gridArray.GetLength(0); x++)
                {
                    for (var y = 0; y < _gridArray.GetLength(1); y++)
                    {
                        if (childCount > counter) {

                            T current = GetGridObject(x, y);
                            if (current == null || forceReplace)
                            {
                                if(current != null)
                                    _provider.Destroy(current);

                                SetGridObject(x, y, parent.GetChild(counter).GetComponent<T>());
                                counter++;
                            }
                            else {
                                Transform child = parent.GetChild(counter);
                                T comp = child.GetComponent<T>();
                                if (comp)
                                {
                                    _provider.Destroy(comp);
                                }
                                else
                                {
                                    if(Application.isPlaying) {
                                        GameObject.Destroy(child);
                                    }
                                    else {
                                        GameObject.DestroyImmediate(child);
                                    }
                                }
                            }
                        }
                        else {
                            if (fillHoles)
                            {
                                T current = GetGridObject(x, y);
                                if (current == null || forceReplace)
                                {
                                    if(current != null)
                                        _provider.Destroy(current);

                                    T instance = _provider.Instantiate();
                                    SetGridObject(x, y, instance);

                                    counter++;
                                }
                            }
                        }

                        _iterator++;
                    }
                }

                if (parent.childCount > counter)
                {
                    while (parent.childCount > counter)
                    {
                        Transform child = parent.GetChild(counter);
                        if (Application.isPlaying)
                        {
                            GameObject.Destroy(child.gameObject);
                        }
                        else
                        {
                            GameObject.DestroyImmediate(child.gameObject);
                        }
                    }
                }
            }
        }

        public void MoveWithDuration (int sourceX, int sourceY, int targetX, int targetY, float duration)
        {
            T source = GetGridObject(sourceX, sourceY);

            MoveWithDuration(source, targetX, targetY, duration);
        }

        public void MoveWithDuration (Vector3 worldPosition, int targetX, int targetY, float duration)
        {
            GetXY(worldPosition, out int x, out int y);

            MoveWithDuration(x, y, targetX, targetY, duration);
        }

        public void MoveWithDuration (T source, int targetX, int targetY, float duration)
        {
            if (source)
            {
                CoroutineStarter.Instance.StartCoroutine(SmoothLerp(source, targetX, targetY, duration));
            }
        }

        public void MoveWithSpeed (int sourceX, int sourceY, int targetX, int targetY, float speed)
        {
            T source = GetGridObject(sourceX, sourceY);

            MoveWithSpeed(source, targetX, targetY, speed);
        }

        public void MoveWithSpeed (Vector3 worldPosition, int targetX, int targetY, float speed)
        {
            GetXY(worldPosition, out int x, out int y);

            MoveWithSpeed(x, y, targetX, targetY, speed);
        }

        public void MoveWithSpeed (T source, int targetX, int targetY, float speed)
        {
            if (source)
            {
                CoroutineStarter.Instance.StartCoroutine(SmoothMoveTowards(source, targetX, targetY, speed));
            }
        }

        protected IEnumerator SmoothLerp (T source, int targetX, int targetY, float duration)
        {
            if(!source)
                yield break;

            Transform sourceT = source.transform;

            //Debug.Log($"{sourceX} {sourceY} {targetX} {targetY}");

            Vector3 targetPos = GetWorldPositionInCenter(targetX, targetY);

            Vector3 startingPos  = sourceT.position;
            Vector3 finalPos = targetPos;
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                if (sourceT)
                {
                    sourceT.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / duration));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                else {
                    yield break;
                }
                
            }

            sourceT.position = finalPos;
        }

        protected IEnumerator SmoothMoveTowards (T source, int targetX, int targetY, float speed)
        {
            if(!source)
                yield break;

            Transform sourceT = source.transform;

            //Debug.Log($"{sourceX} {sourceY} {targetX} {targetY}");

            Vector3 targetPos = GetWorldPositionInCenter(targetX, targetY);

            while (Vector3.Distance(sourceT.position, targetPos) >= 0.0025f)
            {
                if (sourceT)
                {
                    sourceT.position = Vector3.MoveTowards(sourceT.position, targetPos, speed * Time.deltaTime);
                    yield return null;
                }
                else {
                    yield break;
                }
            }

            sourceT.position = targetPos;
        }
    }

    public enum GridMonoProcessValueType {
        None,
        /// <summary>
        /// Just set transform position as grid world position
        /// </summary>
        FixedPosition,
        /// <summary>
        /// Use duration to move, from cell to cell, move time is the same
        /// </summary>
        MovementWithDuration,
        /// <summary>
        /// Use speed to move from cell to cell, with constant speed
        /// </summary>
        MovementWithSpeed
    }
}
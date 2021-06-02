using System.Collections;
using System.Collections.Generic;
using d4160.Coroutines;
using UnityEngine;

namespace d4160.Tick
{
    public class Tick
    {
        private int _timesBySecond;
        private float _fixedDeltaTime;
        private WaitForSeconds _waitForTick;
        private Coroutine _tickRoutine;

        private List<ITickObject> _tickObjects = new List<ITickObject>();

        public float FixedDeltaTime { 
            get => _fixedDeltaTime; 
            set {
                if(_fixedDeltaTime != value) {
                    _fixedDeltaTime = value;
                    _timesBySecond = (int)(1 / _fixedDeltaTime);
                    _waitForTick = new WaitForSeconds(_fixedDeltaTime);
                }
            } 
        }

        public Tick(int timesBySecond = 1) {
            _timesBySecond = timesBySecond;
            _fixedDeltaTime = 1f / _timesBySecond;

            _waitForTick = new WaitForSeconds(_fixedDeltaTime);
        }

        public void StartTick() {
            if (_tickRoutine == null)
                _tickRoutine = CoroutineStarter.Instance.StartCoroutine(TickRoutine());
        }

        public void StopTick() {
            if (_tickRoutine != null)
            {
                CoroutineStarter.Instance.StopCoroutine(_tickRoutine);
                _tickRoutine = null;
            }
        }

        private IEnumerator TickRoutine() {
            while (true) {
                InvokeOnTick();
                yield return _waitForTick;
            }
        }

        private void InvokeOnTick() {
            for (var i = 0; i < _tickObjects.Count; i++)
            {
                _tickObjects[i].OnTick(_fixedDeltaTime);
            }
        }

        public void AddTickObject(ITickObject tickObj) {
            if(tickObj != null && !_tickObjects.Contains(tickObj)) {
                _tickObjects.Add(tickObj);

                if(_tickObjects.Count == 1) {
                    StartTick();
                }
            }
        }

        public void RemoveTickObject(ITickObject tickObj) {
            if(tickObj != null && _tickObjects.Contains(tickObj)) {
                _tickObjects.Remove(tickObj);

                if(_tickObjects.Count == 0) {
                    StopTick();
                }
            }
        }
    }
}
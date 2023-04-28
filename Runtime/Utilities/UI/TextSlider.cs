using d4160.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace d4160.Utilities.UIs
{
    public class TextSlider : SliderBase<string, Button>
    {
        protected override void RegisterEvents()
        {
            _prevBtn.onClick.AddListener(GoPrev);
            _nextBtn.onClick.AddListener(GoNext);
        }
    }

    public abstract class SliderBase<T, TSelectable> : MonoBehaviour
    {
        [SerializeField] protected bool _clamp = true;

        [Header("UI")]
        [SerializeField] protected TextMeshProUGUI _labelTxt;
        [SerializeField] protected TSelectable _prevBtn;
        [SerializeField] protected TSelectable _nextBtn;

        [Header("Events")]
        [SerializeField] protected Event _onValueChanged;

        public Event OnValueChanged => _onValueChanged;

        [System.Serializable]
        public class Event : UnityEvent<int, T> { }

        protected List<T> _values;
        protected int _selected;

        protected virtual void Awake()
        {
            RegisterEvents();
        }

        protected virtual void OnDestroy()
        {
            UnregisterEvents();
        }

        protected abstract void RegisterEvents();
        protected virtual void UnregisterEvents() { }

        public void SetValues(List<T> values, int selected = 0) 
        {
            SetValues(values, selected, true);
        }

        public void SetValuesWithoutNotify(List<T> values, int selected = 0)
        {
            SetValues(values, selected, false);
        }

        private void SetValues(List<T> values, int selected, bool notify)
        {
            _values = values;

            if (_values.IsValidIndex(selected))
            {
                _selected = selected;
                _labelTxt.text = _values[selected].ToString();

                if (notify)
                {
                    _onValueChanged?.Invoke(selected, _values[selected]);
                }
            }
        }

        public void GoPrev()
        {
            GoPrevOrNext(false, true);
        }

        public void GoNext()
        {
            GoPrevOrNext(true, true);
        }

        public void GoPrevWithoutNotify()
        {
            GoPrevOrNext(false, false);
        }

        public void GoNextWithoutNotify()
        {
            GoPrevOrNext(true, false);
        }

        private void GoPrevOrNext(bool next, bool notify)
        {
            _selected += next ? 1 : -1;
            
            if (_clamp)
            {
                _selected = Mathf.Clamp(_selected, 0, _values.LastIndex());
            }
            else
            {
                if (_selected < 0) _selected = _values.LastIndex();
                else if (_selected > _values.LastIndex()) _selected = 0;
            }

            if (notify)
            {
                _onValueChanged?.Invoke(_selected, _values[_selected]);
            }
        }
    }
}
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace d4160.Core.Editors.Utilities
{
    public static class UIElementsUtility
    {
        public static ObjectField ObjectField<T>(T value, string label = null, Action < T> onValueChangedCallback = null) where T : Object
        {
            ObjectField field = new ObjectField
            {
                label = label,
                objectType = typeof(T),
                value = value
            };

            field.RegisterValueChangedCallback((v) =>
            {
                onValueChangedCallback?.Invoke(v.newValue as T);
            });

            return field;
        }
        
        public static EnumField EnumField(string label, Enum defaultValue, Action<Enum> onValueChangedCallback = null)
        {
            EnumField field = new EnumField(label, defaultValue);

            field.RegisterValueChangedCallback((v) =>
            {
                onValueChangedCallback?.Invoke(v.newValue);
            });

            return field;
        }
        
        public static EnumFlagsField EnumFlagsField(string label, Enum defaultValue, Action<Enum> onValueChangedCallback = null)
        {
            EnumFlagsField field = new EnumFlagsField(label, defaultValue);

            field.RegisterValueChangedCallback((v) =>
            {
                onValueChangedCallback?.Invoke(v.newValue);
            });

            return field;
        }

        public static Toggle Toggle(bool value, string label = null, Action<bool> onValueChangedCallback = null)
        {
            Toggle field = new Toggle()
            {
                label = label,
                value = value
            };

            field.RegisterValueChangedCallback((v) =>
            {
                onValueChangedCallback?.Invoke(v.newValue);
            });

            return field;
        }

        public static VisualElement MinMaxIntField(string labelText, ref Vector2Int currentValue, int min, int max, Action<Vector2Int> onSliderChanged, Action<int> onMinValueChanged, Action<int> onMaxValueChanged, int maxLength = 3, float sliderWidth = 136f, float textWidth = 30f)
        {
            currentValue.x = Mathf.Clamp(currentValue.x, min, max);
            currentValue.y = Mathf.Clamp(currentValue.y, min, max);

            var minMaxSlider = new MinMaxSlider("", min, max, min, max)
            {
                value = currentValue
            };

            minMaxSlider.style.width = sliderWidth;

            var ve = new VisualElement();
            ve.style.flexDirection = FlexDirection.Row;

            var label = new Label(labelText);
            var t1 = new IntegerField(maxLength) { value = currentValue.x };
            t1.style.width = textWidth;
            var t2 = new IntegerField(maxLength) { value = currentValue.y };
            t2.style.width = textWidth;

            minMaxSlider.RegisterValueChangedCallback((ev) =>
            {
                onSliderChanged?.Invoke(new Vector2Int((int)ev.newValue.x, (int)ev.newValue.y));

                minMaxSlider.SetValueWithoutNotify(new Vector2Int((int)ev.newValue.x, (int)ev.newValue.y));
                t1.SetValueWithoutNotify((int)ev.newValue.x);
                t2.SetValueWithoutNotify((int)ev.newValue.y);
            });

            t1.RegisterValueChangedCallback((ev) =>
            {
                if (ev.newValue < min)
                    t1.SetValueWithoutNotify(min);
                if (ev.newValue > t2.value)
                    t1.SetValueWithoutNotify(t2.value);

                onMinValueChanged?.Invoke(t1.value);
                minMaxSlider.SetValueWithoutNotify(new Vector2(t1.value, minMaxSlider.value.y));
            });

            t2.RegisterValueChangedCallback((ev) =>
            {
                if (ev.newValue < t1.value)
                    t2.SetValueWithoutNotify(t1.value);
                if (ev.newValue > max)
                    t2.SetValueWithoutNotify(max);

                onMaxValueChanged?.Invoke(t2.value);
                minMaxSlider.SetValueWithoutNotify(new Vector2(minMaxSlider.value.x, t2.value));
            });

            ve.Add(label);
            ve.Add(t1);
            ve.Add(minMaxSlider);
            ve.Add(t2);

            return ve;
        }

        public static VisualElement MinMaxFloatField(string labelText, ref Vector2 currentValue, float min, float max, Action<Vector2> onSliderChanged, Action<float> onMinValueChanged, Action<float> onMaxValueChanged, int maxLength = 3, float sliderWidth = 136f, float textWidth = 30f)
        {
            currentValue.x = Mathf.Clamp(currentValue.x, min, max);
            currentValue.y = Mathf.Clamp(currentValue.y, min, max);

            var minMaxSlider = new MinMaxSlider("", min, max, min, max)
            {
                value = currentValue
            };

            minMaxSlider.style.width = sliderWidth;

            var ve = new VisualElement();
            ve.style.flexDirection = FlexDirection.Row;

            var label = new Label(labelText);
            var t1 = new FloatField(maxLength) { value = currentValue.x };
            t1.style.width = textWidth;
            var t2 = new FloatField(maxLength) { value = currentValue.y };
            t2.style.width = textWidth;

            minMaxSlider.RegisterValueChangedCallback((ev) =>
            {
                onSliderChanged?.Invoke(ev.newValue);

                t1.SetValueWithoutNotify(ev.newValue.x);
                t2.SetValueWithoutNotify(ev.newValue.y);
            });

            t1.RegisterValueChangedCallback((ev) =>
            {
                if (ev.newValue < min)
                    t1.SetValueWithoutNotify(min);
                if (ev.newValue > t2.value)
                    t1.SetValueWithoutNotify(t2.value);

                onMinValueChanged?.Invoke(t1.value);
                minMaxSlider.SetValueWithoutNotify(new Vector2(t1.value, minMaxSlider.value.y));
            });

            t2.RegisterValueChangedCallback((ev) =>
            {
                if (ev.newValue < t1.value)
                    t2.SetValueWithoutNotify(t1.value);
                if (ev.newValue > max)
                    t2.SetValueWithoutNotify(max);

                onMaxValueChanged?.Invoke(t2.value);
                minMaxSlider.SetValueWithoutNotify(new Vector2(minMaxSlider.value.x, t2.value));
            });

            ve.Add(label);
            ve.Add(t1);
            ve.Add(minMaxSlider);
            ve.Add(t2);

            return ve;
        }

        public static VisualElement RangeField(string labelText, ref float currentValue, float start, float end, Action<float> onSliderChanged, Action<float> onValueChanged, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0, int maxLength = 5, float sliderWidth = 136f, float textWidth = 50f)
        {
            return new RangeField(labelText, ref currentValue, start, end, onSliderChanged, onValueChanged, direction, pageSize, maxLength, sliderWidth, textWidth);
        }

        public static Button Button(string text, Action onClick)
        {
            var button = new Button(() =>
                {
                    onClick?.Invoke();
                })
                { text = text };

            return button;
        }
    }

    public class RangeField : VisualElement
    {
        protected Slider _slider;
        protected Label _label;
        protected FloatField _floatField;

        public Slider Slider => _slider;
        public Label Label => _label;
        public FloatField FloatField => _floatField;

        public RangeField(string labelText, ref float currentValue, float start, float end, Action<float> onSliderChanged, Action<float> onValueChanged, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0, int maxLength = 5, float sliderWidth = 136f, float textWidth = 50f)
        {
            currentValue = Mathf.Clamp(currentValue, start, end);

            _slider = new Slider("", start, end, direction, pageSize)
            {
                value = currentValue
            };

            _slider.style.width = sliderWidth;

            this.style.flexDirection = FlexDirection.Row;

            _label = new Label(labelText);
            _floatField = new FloatField(maxLength) { value = currentValue };
            _floatField.style.width = textWidth;

            _slider.RegisterValueChangedCallback((ev) =>
            {
                onSliderChanged?.Invoke(ev.newValue);

                _floatField.SetValueWithoutNotify(ev.newValue);
            });

            _floatField.RegisterValueChangedCallback((ev) =>
            {
                if (ev.newValue < start)
                    _floatField.SetValueWithoutNotify(start);
                if (ev.newValue > end)
                    _floatField.SetValueWithoutNotify(end);

                onValueChanged?.Invoke(_floatField.value);
                _slider.SetValueWithoutNotify(_floatField.value);
            });

            Add(_label);
            Add(_floatField);
            Add(_slider);
        }

        public void UpdateValue(float value)
        {
            _slider.value = value;
            _floatField.value = value;
        }
    }
}
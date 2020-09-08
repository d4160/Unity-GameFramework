using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace d4160.Core.Editors.Utilities
{
    using UnityEngine;

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
    }
}
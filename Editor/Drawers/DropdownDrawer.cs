namespace d4160.Core.Editors
{
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;
    using System.Collections.Generic;
    using d4160.Core.Attributes;
    using d4160.Core.Editors.Utilities;

    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            position.x -= (EditorGUI.indentLevel * 15);
            position.width += (EditorGUI.indentLevel * 15);

            var target = property.serializedObject.targetObject;
            var attrib = attribute as DropdownAttribute;

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var type = target.GetType();
            var propInfo = type.GetProperty(attrib.ValuesProperty, flags);

            if (propInfo != null && propInfo.PropertyType == typeof(string[]))
            {
                var names = propInfo.GetValue(target) as string[];

                if (attrib.IncludeNone)
                {
                    names = EditorUtilities.GetNoneSelectableFrom(names);
                }

                if (attrib.ExceptionNames != null || attrib.ExceptionNames.Length != 0)
                {
                    names = GetNamesWithExceptions(names, attrib.ExceptionNames);
                }

                var value = property.intValue;
                value = attrib.IncludeNone ? value + 1 : value;

                property.intValue = EditorGUI.Popup(position, value, names);

                if (attrib.IncludeNone)
                    property.intValue--;
            }
            else
            {
                EditorGUI.LabelField(position, $"Cannot get archetypes names in {attrib.ValuesProperty} property of object {target.name}");
            }
        }

        private string[] GetNamesWithExceptions(string[] names, string[] exceptions)
        {
            var list = new List<string>(names);
            for (int i = names.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < exceptions.Length; j++)
                {
                    if (names[i] == exceptions[j])
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
            return list.ToArray();
        }
    }
}
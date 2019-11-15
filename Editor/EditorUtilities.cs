namespace d4160.Core.Editors.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public static class EditorUtilities
    {
        public static string[] GetNoneSelectableFrom(string[] names, string noneLabel = "None")
        {
            var list = new List<string>(names);
            list.Insert(0, noneLabel);
            return list.ToArray();

            /*
            *var newStrings = new string[strings.Length + 1];

            for (int i = 0; i < newStrings.Length; i++)
            {
                if (i == 0)
                    newStrings[i] = noneLabel;
                else
                    newStrings[i] = strings[i - 1];
            }

            return newStrings;
             */
        }

        public static string AssetsRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            else {
                throw new System.ArgumentException("Full path does not contain the current project's Assets folder", "absolutePath");
            }
        }

        public static string FixScriptName(string scriptFileName)
        {
            string fileName = scriptFileName.Replace(" ", "_");
            fileName = fileName.Replace("-", "_");

            return fileName;
        }

        public static string GetRelativePathOfFile(string fileName)
        {
            var dataPathDir = new DirectoryInfo(Application.dataPath);
            var dataPathUri = new System.Uri(Application.dataPath);
            foreach (var file in dataPathDir.GetFiles(fileName, SearchOption.AllDirectories))
            {
                var relativeUri = dataPathUri.MakeRelativeUri(new System.Uri(file.FullName));
                var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

                return relativePath;
            }

            return string.Empty;
        }

        public static void ObjectField<T>(SerializedProperty property, GUIContent label, GUIContent nullLabel, T[] objectList) where T : UnityEngine.Object
        {
            if (property == null)
            {
                return;
            }

            List<GUIContent> objectNames = new List<GUIContent>();

            T selectedObject = property.objectReferenceValue as T;

            int selectedIndex = -1; // Invalid index

            // First option in list is <None>
            objectNames.Add(nullLabel);
            if (selectedObject == null)
            {
                selectedIndex = 0;
            }

            for (int i = 0; i < objectList.Length; ++i)
            {
                if (objectList[i] == null) continue;
                objectNames.Add(new GUIContent(objectList[i].name));

                if (selectedObject == objectList[i])
                {
                    selectedIndex = i + 1;
                }
            }

            T result;

            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, objectNames.ToArray());

            if (selectedIndex == -1)
            {
                // Currently selected object is not in list, but nothing else was selected so no change.
                return;
            }
            else if (selectedIndex == 0)
            {
                result = null; // Null option
            }
            else
            {
                result = objectList[selectedIndex - 1];
            }

            property.objectReferenceValue = result;
        }

        public static void ObjectFieldAsIndex<T>(Rect position, SerializedProperty property, GUIContent label, GUIContent nullLabel, T[] objectList) where T : UnityEngine.Object
        {
            if (property == null)
            {
                return;
            }

            var objectNames = new List<GUIContent>();

            var selectedIndex = nullLabel != GUIContent.none ? property.intValue + 1 : property.intValue;

            // First option in list as 'nullLabel'
            if (nullLabel != GUIContent.none)
                objectNames.Add(nullLabel);

            for (int i = 0; i < objectList.Length; ++i)
            {
                if (objectList[i] == null)
                    objectNames.Add(nullLabel);

                objectNames.Add(new GUIContent(objectList[i].name));
            }

            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, objectNames.ToArray());

            property.intValue = nullLabel != GUIContent.none ? selectedIndex - 1 : selectedIndex;
        }

        /// <summary>
        /// Need to do more tests...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
        {
            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (obj == null) { return null; }

            T actualObject = null;
            if (obj.GetType().IsArray)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                actualObject = ((T[])obj)[index];
            }
            else
            {
                actualObject = obj as T;
            }
            return actualObject;
        }
    }
}
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace d4160.SceneManagement.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element.
            var container = new VisualElement();

            // Create property fields.
            var sceneAsset = new PropertyField(property.FindPropertyRelative("sceneAsset"));

            // Add fields to the container.
            container.Add(sceneAsset);

            return container;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty scenePathProp = property.FindPropertyRelative("scenePath");
            SerializedProperty sceneAsset = property.FindPropertyRelative("_sceneAsset");

            Rect sceneRect = new Rect(position.x, position.y, position.width - 64f, position.height);
            Rect button1Rect = new Rect(position.x + position.width - 64f, position.y, 32f, position.height);
            Rect button2Rect = new Rect(position.x + position.width - 32f, position.y, 32f, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.ObjectField(sceneRect, sceneAsset, GUIContent.none);
            var iconContent = EditorGUIUtility.IconContent("FolderOpened On Icon");
            iconContent.tooltip = "Open Single";
            if (GUI.Button(button1Rect, iconContent))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    if (sceneAsset.objectReferenceValue)
                        EditorSceneManager.OpenScene($"{scenePathProp.stringValue}", OpenSceneMode.Single);
                }
            }

            iconContent = EditorGUIUtility.IconContent("FolderOpened Icon");
            iconContent.tooltip = "Open Additive";
            if (GUI.Button(button2Rect, iconContent))
            {
                if (sceneAsset.objectReferenceValue)
                    EditorSceneManager.OpenScene($"{scenePathProp.stringValue}", OpenSceneMode.Additive);
            }

            EditorGUI.EndProperty();
        }
    }
}
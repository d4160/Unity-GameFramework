using BetterDefines.Editor.Entity;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BetterDefines.Editor
{
    public class BetterDefinesWindow : EditorWindow
    {
        private string addDefineText = "NEW_DEFINE_SCRIPTING_SYMBOL";
        private bool drawMainDefines = true;
        private ReorderableList list;

        private Vector2 scrollPos;
        private SerializedObject settingsSerializedObject;

        [MenuItem("Window/Better Defines")]
        private static void Init()
        {
            var window = (BetterDefinesWindow) GetWindow(typeof (BetterDefinesWindow));

            window.titleContent = new GUIContent("Defines");

            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            settingsSerializedObject = new SerializedObject(BetterDefinesSettings.Instance);
            list = DefinesReorderableList.Create(settingsSerializedObject);
        }

        private void OnGUI()
        {
            //EditorGUI.BeginChangeCheck();
            settingsSerializedObject.Update();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawTopSettingsTabs();
            if (drawMainDefines)
            {
                DrawAddDefine();
                list.DoLayoutList();
            }
            else
            {
                DrawPreferences();
            }
            EditorGUILayout.EndScrollView();
            settingsSerializedObject.ApplyModifiedProperties();
            /*if (EditorGUI.EndChangeCheck())
            {
                AssetDatabase.SaveAssets();
            }*/
        }

        private void DrawAddDefine()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            addDefineText = EditorGUILayout.TextField(addDefineText);

            var isEmpty = string.IsNullOrEmpty(addDefineText);
            var isNameValid = addDefineText.IsValidDefineName();
            var isAlreadyAdded = BetterDefinesSettings.Instance.IsDefinePresent(addDefineText);

            GUI.enabled = !isEmpty && isNameValid && !isAlreadyAdded;
            if (GUILayout.Button("ADD"))
            {
                AddElement(addDefineText);
                addDefineText = string.Empty;
            }
            GUI.enabled = true;

            if (GUILayout.Button(new GUIContent("Update", "Add and updates the defines in the selected BuildTargetGroup (PlayerSettings)")))
            { 
                UpdateDefines();
            }

            if (GUILayout.Button(new GUIContent("Select", "Select the BetterDefinesSettings asset")))
            {
                Selection.SetActiveObjectWithContext(BetterDefinesSettings.Instance, null);
            }

            if (GUILayout.Button(new GUIContent("Open", "Open the PlayerSettings window")))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            }

            EditorGUILayout.EndHorizontal();
            if (!isEmpty && !isNameValid) { EditorGUILayout.HelpBox("Invalid symbol name", MessageType.Error); }
            if (!isEmpty && isAlreadyAdded) { EditorGUILayout.HelpBox("Symbol already added", MessageType.Error); }
            EditorGUILayout.EndVertical();
        }

        private void UpdateDefines()
        {
            var defines = BetterDefinesUtils.GetSelectedTargetGroupDefines();
            for (int i = 0; i < defines.Length; i++)
            {
                var define = defines[i];
                var isAlreadyAdded = BetterDefinesSettings.Instance.IsDefinePresent(define);
                var selectedPlatformId = PlatformUtils.GetIdByBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                var definesSettings = BetterDefinesSettings.Instance;

                if (isAlreadyAdded)
                {
                    if (!definesSettings.GetDefineState(define, selectedPlatformId))
                    {
                        definesSettings.SetDefineState(define, selectedPlatformId, true);
                    }
                }
                else
                {
                    AddElement(define);

                    UpdateDefines();
                }
            }  
        }

        private void AddElement(string validDefine)
        {
            settingsSerializedObject.Update();
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            var customDefine = list.serializedProperty.GetArrayElementAtIndex(index);
            customDefine.FindPropertyRelative("Define").stringValue = validDefine;

            // disable all by default
            var defineSettings = customDefine.FindPropertyRelative("StatesForPlatforms");
            for (var i = 0; i < defineSettings.arraySize; i++)
            {
                defineSettings.GetArrayElementAtIndex(i).FindPropertyRelative("IsEnabled").boolValue = false;
            }

            settingsSerializedObject.ApplyModifiedProperties();
        }

        private void DrawTopSettingsTabs()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(drawMainDefines, "Custom Defines", "toolbarbutton"))
            {
                drawMainDefines = true;
            }
            if (GUILayout.Toggle(!drawMainDefines, "Preferences", "toolbarbutton"))
            {
                drawMainDefines = false;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawPreferences()
        {
            EditorGUILayout.HelpBox("Please disable platforms that you are not using in your project. " +
                                    "For disabled platforms toggles will not be displayed in defines tab", MessageType.Info);
            foreach (var platform in PlatformUtils.AllAvailableBuildPlatforms)
            {
                var setting = BetterDefinesSettings.Instance.GetGlobalPlatformState(platform.Id);

                if (setting.PlatformId == PlatformUtils.STANDALONE_PLATFORM_ID) { GUI.enabled = false; }
                setting.IsEnabled = GUILayout.Toggle(setting.IsEnabled, new GUIContent($" {platform.Name}", platform.Icon));
                GUI.enabled = true;
            }
            EditorUtility.SetDirty(BetterDefinesSettings.Instance);
        }
    }
}
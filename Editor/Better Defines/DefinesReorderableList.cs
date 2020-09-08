using System.Linq;
using System.Text.RegularExpressions;
using BetterDefines.Editor.Entity;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BetterDefines.Editor
{
    public static class DefinesReorderableList
    {
        private static readonly string ARROW_DOWN = Regex.Unescape("\u25BC");

        public static ReorderableList Create(SerializedObject settingsSerializedObject)
        {
            var listSerializedProperty = settingsSerializedObject.FindProperty("Defines");
            var list = new ReorderableList(settingsSerializedObject, listSerializedProperty,
                true,
                true, false, true);
            list.drawHeaderCallback += rect => { EditorGUI.LabelField(rect, "Custom Scripting Define Symbols"); };
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.y += 2;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                var defineProp = element.FindPropertyRelative("Define");

                EditorGUI.SelectableLabel(new Rect(rect.x, rect.y, rect.width*0.25f, EditorGUIUtility.singleLineHeight),
                    defineProp.stringValue);

                DrawPlatformToggles(rect, defineProp.stringValue);
                DrawActionButton(rect, defineProp.stringValue);
            };
            return list;
        }

        private static void DrawPlatformToggles(Rect rect, string define)
        {
            var settings = BetterDefinesSettings.Instance;
            var platformsWidth = rect.width*0.7f;
            var filteredPlatforms = PlatformUtils.AllAvailableBuildPlatforms.Where(x => settings.GetGlobalPlatformState(x.Id).IsEnabled).ToList();
            var singleToggleWidth = platformsWidth/filteredPlatforms.Count;

            for (var i = 0; i < filteredPlatforms.Count; i++)
            {
                var isEnabled = settings.GetDefineState(define, filteredPlatforms[i].Id);
                var result = GUI.Toggle(GetPlatformToggleRect(rect, singleToggleWidth, i), isEnabled,
                    filteredPlatforms[i].ToGUIContent(), EditorStyles.toolbarButton);
                settings.SetDefineState(define, filteredPlatforms[i].Id, result);
            }
        }

        private static Rect GetPlatformToggleRect(Rect rect, float singleToggleWidth, int index)
        {
            var platformsXStartPos = rect.x + rect.width*0.23f;
            return new Rect(platformsXStartPos + index*singleToggleWidth, rect.y, singleToggleWidth, EditorGUIUtility.singleLineHeight);
        }

        #region action_context_menu
        private const string APPLY_CONFIG_ACTION_ID = "APPLY_CONFIG";
        private const string REMOVE_FROM_ALL_ACTION_ID = "REMOVE_FROM_ALL";
        private const string ADD_TO_ALL_ACTION_ID = "ADD_TO_ALL";


        private class ActionParams
        {
            public string Define;
            public string Id;
        }

        private static void DrawActionButton(Rect rect, string define)
        {
            var xPos = rect.x + rect.width*0.95f;
            var width = rect.width*0.05f;
            GUI.color = Color.green;
            if (GUI.Button(new Rect(xPos, rect.y, width, EditorGUIUtility.singleLineHeight), ARROW_DOWN))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Apply Configuration"), false, ActionClickHandler,
                    new ActionParams {Id = APPLY_CONFIG_ACTION_ID, Define = define});
                menu.AddItem(new GUIContent("Add to All Platforms"), false, ActionClickHandler,
                    new ActionParams {Id = ADD_TO_ALL_ACTION_ID, Define = define});
                menu.AddItem(new GUIContent("Remove From All Platforms"), false, ActionClickHandler,
                    new ActionParams {Id = REMOVE_FROM_ALL_ACTION_ID, Define = define});
                menu.ShowAsContext();
            }
            GUI.color = Color.white;
        }

        private static void ActionClickHandler(object target)
        {
            var data = (ActionParams) target;
            switch (data.Id)
            {
                case APPLY_CONFIG_ACTION_ID:
                    ApplySelectedConfig(data.Define);
                    break;
                case ADD_TO_ALL_ACTION_ID:
                    BetterDefinesUtils.AddDefineToAll(data.Define);
                    break;
                case REMOVE_FROM_ALL_ACTION_ID:
                    BetterDefinesUtils.RemoveDefineFromAll(data.Define);
                    break;
                default:
                    Debug.LogError("Id not present");
                    break;
            }
        }

        private static void ApplySelectedConfig(string define)
        {
            var settings = BetterDefinesSettings.Instance;
            foreach (var platformId in settings.GetGlobalUserEnabledPlatformIds())
            {
                var isEnabled = settings.GetDefineState(define, platformId);
                var buildTargetGroup = PlatformUtils.GetBuildTargetGroupById(platformId);
                if(buildTargetGroup == BuildTargetGroup.Unknown)
                {
                    continue;
                }
                BetterDefinesUtils.ToggleDefine(define, isEnabled, buildTargetGroup);
            }
        }
        #endregion
    }
}
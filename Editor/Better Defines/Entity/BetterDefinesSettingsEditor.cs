using UnityEditor;

namespace BetterDefines.Editor.Entity
{
    [CustomEditor(typeof(BetterDefinesSettings))]
    public class BetterDefinesSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Label");
            base.OnInspectorGUI();
        }
    }
}   
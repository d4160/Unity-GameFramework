using UnityEditor;
using UnityEngine;

namespace BetterDefines.Editor
{
    public class CustomDefineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.DrawRect(position, Color.black);
            //base.OnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 50f;
        }
    }
}
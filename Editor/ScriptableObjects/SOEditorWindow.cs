using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InspectInLine;
using UnityEditor.Callbacks;
using d4160.ScriptableObjects.Editors.Utilities;

namespace d4160.ScriptableObjects.Editors
{
    public abstract class SOEditorWindow<TEw, TSoContainer, TSoElement> : EditorWindow, ISoEditorWindow where TSoContainer : ScriptableObject where TSoElement : ScriptableObject where TEw : EditorWindow, ISoEditorWindow
    {
        protected SerializedObject _serializedObject;
        protected SerializedProperty _currentProperty;

        private readonly Dictionary<Object, SerializedObject>
            _serializedObjectMap = new Dictionary<Object, SerializedObject>();
        private readonly Dictionary<Object, bool>
            _expandedMap = new Dictionary<Object, bool>();

        protected string _selectedPropertyPath;
        protected SerializedProperty _selectedProperty;
        protected ScriptableObject _scriptableObject;

        protected int selectedIndex = -1;
        protected int moveSrcIndex = 0;
        protected int moveDstIndex = 0;

        public virtual string WindowTitle => typeof(TEw).Name;
        /// <summary>
        /// The icon for this window.
        /// More info: https://docs.unity3d.com/ScriptReference/EditorGUIUtility.IconContent.html
        /// And: https://github.com/halak/unity-editor-icons
        /// </summary>
        public virtual string WindowIcon => "BuildSettings.Editor.Small";
        protected virtual bool CustomObjectPropertiesDraw => false;

        public ScriptableObject ScriptableObject { get => _scriptableObject; set => _scriptableObject = value; }
        public SerializedObject SerializedObject { get => _serializedObject; set => _serializedObject = value; }

        public static void Open(TSoContainer data)
        {
            TEw window = GetWindow<TEw>();
            GUIContent guiContent = EditorGUIUtility.IconContent(window.WindowIcon);
            guiContent.text = window.WindowTitle;
            window.titleContent = guiContent;

            window.ScriptableObject = data;
            window.SerializedObject = new SerializedObject(data);
        }

        // Implement this in child classes, since can't use with generic
        //[OnOpenAsset()]
        //public static bool OpenEditor(int instanceId, int line)
        //{
        //    TSoContainer data = EditorUtility.InstanceIDToObject(instanceId) as TSoContainer;
        //    if (data)
        //    {
        //        Open(data);
        //        return true;
        //    }

        //    return false;
        //}

        // To hide base inspector (also hides Naughty and Odin special attributes like buttons...)
        // [CustomEditor(typeof(TSoContainer))]
        //public class ...CustomEditor : Editor
        //{
        //    public override void OnInspectorGUI()
        //    {
        //        // base.OnInspectorGUI();
        //        if (GUILayout.Button("Open Editor"))
        //        {
        //            TEw.Open(target as TSoContainer);
        //        }
        //    }
        //}

        private SerializedObject GetSerializedObject(Object target)
        {
            Debug.Assert(target != null);
            var serializedObject = default(SerializedObject);
            if (_serializedObjectMap.TryGetValue(target, out serializedObject))
                return serializedObject;

            serializedObject = new SerializedObject(target);
            _serializedObjectMap.Add(target, serializedObject);
            return serializedObject;
        }

        private bool GetExpandedState(Object target)
        {
            Debug.Assert(target != null);
            bool expanded = false;
            if (_expandedMap.TryGetValue(target, out expanded))
                return expanded;

            _expandedMap.Add(target, false);
            return false;
        }

        protected virtual void OnGUI()
        {
            if (_serializedObject == null)
            {
                _serializedObject = new SerializedObject(_scriptableObject);
            }

            // Debug.Log("Start");
            _currentProperty = _serializedObject.FindProperty("_array");
            // DrawProperties(_currentProperty, true);
            // Debug.Log($"{_currentProperty.arraySize}");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(170), GUILayout.ExpandHeight(true));

            DrawSidebar(_currentProperty);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

            if (_selectedProperty != null)
            {
                // Debug.Log($"{_selectedProperty.name} {_selectedProperty.propertyType}");
                DrawProperties(_selectedProperty, true);
                // DrawSelectedPropertyPanel();
            }
            else
            {
                EditorGUILayout.LabelField("Select an item from the list");
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            // Debug.Log("End");

            Apply();
        }

        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            // EditorGUILayout.PropertyField(prop, drawChildren); // Draw default property with attributes
            string lastPropPath = string.Empty;

            if (prop == null)
            {
                _selectedPropertyPath = null;
                _selectedProperty = null;
                // Debug.Log("Prop is null");
                return;
            }

            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                DrawObject(prop, drawChildren);
            }
            else
            {
                // This also prints the array elements, the first element is the 'size'
                //Debug.Log(prop.name);
                foreach (SerializedProperty p in prop)
                {
                    if (p.name == "array" || p.name == "size") continue;
                    //Debug.Log(p.name);
                    // When is array is generic
                    if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                    {
                        EditorGUILayout.BeginHorizontal();
                        p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                        EditorGUILayout.EndHorizontal();

                        if (p.isExpanded)
                        {
                            // Draw all 
                            EditorGUI.indentLevel++;
                            DrawProperties(p, drawChildren);
                            EditorGUI.indentLevel--;
                        }
                    }
                    else if (p.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        // if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) continue;
                        // lastPropPath = p.propertyPath;
                        DrawObject(p, drawChildren);
                    }
                    else
                    {
                        // if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) continue;
                        // lastPropPath = p.propertyPath;
                        EditorGUILayout.PropertyField(p, drawChildren);
                    }
                }
            }
        }

        protected void DrawObject(SerializedProperty p, bool drawChildren)
        {
            if (p.objectReferenceValue is ScriptableObject)
            {
                EditorGUILayout.BeginHorizontal();
                bool expanded = GetExpandedState(p.objectReferenceValue);
                expanded = EditorGUILayout.Foldout(expanded, p.displayName);
                _expandedMap[p.objectReferenceValue] = expanded;
                EditorGUILayout.PropertyField(p, GUIContent.none, drawChildren);
                EditorGUILayout.EndHorizontal();

                if (expanded)
                {
                    SerializedObject sObj = GetSerializedObject(p.objectReferenceValue);

                    EditorGUI.indentLevel++;
                    if (!CustomObjectPropertiesDraw)
                    {
                        var properties = sObj.EnumerateChildProperties();
                        
                        foreach (var property in properties)
                        {
                            EditorGUILayout.PropertyField(property, drawChildren);
                        }
                    }
                    else
                    {
                        Dictionary<string, SerializedProperty> dic = sObj.GetChildProperties();
                        DrawObjectProperties(dic);
                    }
                    EditorGUI.indentLevel--;
                    sObj.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }

        protected virtual void DrawObjectProperties(Dictionary<string, SerializedProperty> dic)
        {

        }

        protected void DrawSidebar(SerializedProperty prop)
        {
            string arrayPath = null;
            int i = 0;
            int count = 0;

            if (prop == null)
            {
                // Debug.Log("prop is null");
                return;
            }

            foreach (SerializedProperty p in prop)
            {
                if (p == null)
                {
                    continue;
                }

                if (p.name == "array")
                {
                    // Cant do array = p, since p reference is changing in foreach
                    arrayPath = p.propertyPath;
                    count = p.arraySize;
                    continue;
                }

                if (p.name == "size")
                {
                    continue;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    string displayName = p.propertyType == SerializedPropertyType.ObjectReference ? p.objectReferenceValue?.name : p.displayName;
                    if (GUILayout.Button($"{i}: {displayName}", GUILayout.MaxWidth(120)))
                    {
                        _selectedPropertyPath = p.propertyPath;
                        selectedIndex = i;
                        _selectedProperty = _serializedObject.FindProperty(_selectedPropertyPath);
                    }

                    if (GUILayout.Button("D", GUILayout.MaxWidth(25)))
                    {
                        DuplicateItem(p, _serializedObject.FindProperty(arrayPath), i);
                    }

                    if (GUILayout.Button("X", GUILayout.MaxWidth(25)))
                    {
                        DeleteItem(p, _serializedObject.FindProperty(arrayPath), i);
                    }
                }

                i++;
            }

            if (GUILayout.Button("New"))
            {
                AddNewItem(_serializedObject.FindProperty(arrayPath));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                moveSrcIndex = EditorGUILayout.IntField(moveSrcIndex, GUILayout.MaxWidth(45));
                moveDstIndex = EditorGUILayout.IntField(moveDstIndex, GUILayout.MaxWidth(45));
                if (GUILayout.Button("Move", GUILayout.MaxWidth(80)))
                {
                    MoveArrayElement(_serializedObject.FindProperty(arrayPath), moveSrcIndex, moveDstIndex);
                }
            }

            // Debug.Log($"{count} {iSelected}");

            if (selectedIndex >= count)
            {
                _selectedProperty = null;
            }

            //if (!string.IsNullOrEmpty(_selectedPropertyPath))
            //{
            //    _selectedProperty = _serializedObject.FindProperty(_selectedPropertyPath);
            //}
        }

        protected void SaveSubAsset(ScriptableObject subasset)
        {
            AssetDatabase.AddObjectToAsset(subasset, _scriptableObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_scriptableObject));
        }

        protected virtual void AddNewItem(SerializedProperty array)
        {
            var subasset = CreateInstance<TSoElement>();

            // Debug.Log($"{array.name} - {array.propertyType} - {array.arraySize}");
            //foreach (SerializedProperty p in array)
            //{
            //    Debug.Log($"{p.name}");
            //}

            subasset.name = $"{array.arraySize}";

            SaveSubAsset(subasset);

            array.arraySize++;
            var sp = array.GetArrayElementAtIndex(array.arraySize - 1);
            sp.objectReferenceValue = subasset;

            Apply();
        }

        protected virtual void DuplicateItem(SerializedProperty p, SerializedProperty array, int index)
        {
            if (_scriptableObject is IScriptableCopy<TSoElement> so)
            {
                if (p.objectReferenceValue is TSoElement key)
                {
                    TSoElement copyKey = _scriptableObject.CreateCopy(key);
                    copyKey.name = $"{key.name} (copy)";

                    SaveSubAsset(copyKey);

                    array.InsertArrayElementAtIndex(index + 1);
                    var sp = array.GetArrayElementAtIndex(index + 1);
                    sp.objectReferenceValue = copyKey;

                    Apply();
                }
            }
        }

        protected virtual void DeleteItem(SerializedProperty p, SerializedProperty array, int index)
        {
            SerializedProperty sp = array.GetArrayElementAtIndex(index);
            if (sp.objectReferenceValue)
            {
                _scriptableObject.RemoveSubAsset(sp.objectReferenceValue);
            }
            array.DeleteArrayElementAtIndex(index);
        }

        protected void MoveArrayElement(SerializedProperty array, int srcIndex, int dstIndex)
        {
            array.MoveArrayElement(srcIndex, dstIndex);
        }

        protected void DrawSelectedPropertyPanel()
        {
            _currentProperty = _selectedProperty;

            EditorGUILayout.BeginHorizontal("box");

            // DrawField("name", true);
            // ...

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");

            if (GUILayout.Button("Character", EditorStyles.toolbarButton))
            {
                //_character = true;
                //_inventory = false;
            }

            if (GUILayout.Button("Inventory", EditorStyles.toolbarButton))
            {
                //_character = false;
                //_inventory = true;
            }

            EditorGUILayout.EndHorizontal();

            if (true) // _character
            {
                EditorGUILayout.BeginVertical("box");
                //...
                EditorGUILayout.EndVertical();
            }

            if (true) // _inventory
            {
                EditorGUILayout.BeginVertical("box");
                //...
                EditorGUILayout.EndVertical();
            }
        }

        // 8:54
        protected void DrawField(string propName, bool relative)
        {
            if (relative && _currentProperty != null)
            {
                EditorGUILayout.PropertyField(_currentProperty.FindPropertyRelative(propName), true);
            }
            else if (_serializedObject != null)
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(propName), true);
            }
        }

        protected void Apply()
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }

    public interface ISoEditorWindow
    {
        string WindowTitle { get; }
        string WindowIcon { get; }
        ScriptableObject ScriptableObject { get;set; }
        SerializedObject SerializedObject { get; set; }
    }
}

// UnityEditor.Handles.DrawAAPolyLine... Handles.DrawBezier(... (y2 - y1) * 0.5f  -y2 || +y1...
// ExecuteAlways, OnDrawGizmos...Selected
// Shader.Find("Default.Diffuse");... new Material(shader){ hideFlags = HideFlags.... }
// UnityEngine.Rendering.MaterialPropertyBlock... new ()... Mpb.SetColor... ren.SetPropertyBlock(Mpb)
// Handles.DrawWireDisc(point, transforn.up, radius
// Handles.color... Gizmos.color
// : Editor... CustomEditor(typeof....) OnInspectorGUI... GUILayout... EditorLayoutGUI... Options: GUILayout.width()
// using(new GUILayout.HorizontalScope(EditorStyles.helpBox) { ... }
// EditorStyles.... GUI.skin.button... GUILayout.Space(px)
// Editor... target... if (newValue != target.value) Undo.RecordObject(target, "string"); ... target.value = newValue
//[CanEditMultipleObjects] targets
// Editor -> OnEnable
// so.Update();
// new SerializedObject(target)... SerializedProperty propName = so.FindProperty("propName"); // this way hold Undo and Multiobject
// bool somethingChange = so.ApplyModifiedProperties(); // if (bool)... after do something (apply material color)
// 2:50:00
// MenuItem("Tools/...") in static
// ContextMenu() in normal
// selection.gameObjects
// Vector3 extention -> Mathf.Round(v.x...)
// Undo.Record(... go.transform, const...) // the object cant be a go, in case when modify the transform position
// 3:04:19
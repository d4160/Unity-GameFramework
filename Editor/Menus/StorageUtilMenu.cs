using UnityEngine;
using UnityEditor;

public class StorageUtilMenu : ScriptableObject
{
    [MenuItem("Tools/Storage/Application.persistentDataPath")]
    static void ShowInExplorerPersistentDataPath()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Tools/Storage/Application.dataPath")]
    static void ShowInExplorerDataPath()
    {
        EditorUtility.RevealInFinder(Application.dataPath);
    }
}
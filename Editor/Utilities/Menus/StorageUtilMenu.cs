using UnityEngine;
using UnityEditor;

public class StorageUtilMenu : ScriptableObject
{
    [MenuItem("Tools/d4160/Storage/Application.persistentDataPath")]
    static void ShowInExplorerPersistentDataPath()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Tools/d4160/Storage/Application.dataPath")]
    static void ShowInExplorerDataPath()
    {
        EditorUtility.RevealInFinder(Application.dataPath);
    }
}
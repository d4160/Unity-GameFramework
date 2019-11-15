namespace d4160.Utilities
{
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    public static class IOUtilities
    {
        /// <summary>
        /// Returns the RelativePath of the created folder
        /// </summary>
        /// <param name="absPath"></param>
        /// <param name="folderType"></param>
        /// <returns></returns>
        public static string CreateAssetFolderIfNoExists(string absPath, FolderType folderType)
        {
            var folderPathToUse = PathCombine(absPath, folderType);

            if (!Directory.Exists(folderPathToUse))
            {
                var relPath = AssetsRelativePath(absPath);
                folderPathToUse = CreateAssetFolder(relPath, folderType);
            }
            else
                folderPathToUse = AssetsRelativePath(folderPathToUse);

            return folderPathToUse;
        }

        public static void CreateCSharpScript(string assetsRelPath, string fileName, string content)
        {
            var completeFileName = Path.Combine(assetsRelPath, fileName + ".cs");

            if (File.Exists(completeFileName))
                Debug.Log("File exits");

            using (StreamWriter writer = File.CreateText(completeFileName))
            {
                writer.Write(content);
            }
        }

        public static string AssetsRelativePath(string absPath)
        {
            if (absPath.StartsWith(Application.dataPath))
                return "Assets" + absPath.Substring(Application.dataPath.Length);

            return Application.dataPath;
        }

        public static string PathCombine(string basePath, FolderType appendFolderType)
        {
            switch (appendFolderType)
            {
                case FolderType.Models:
                    return Path.Combine(basePath, "Models");
                case FolderType.Controllers:
                    return Path.Combine(basePath, "Controllers");
                case FolderType.Components:
                    return Path.Combine(basePath, "Components");
                case FolderType.DataControllers:
                    return Path.Combine(basePath, "DataControllers");
                case FolderType.Managers:
                    return Path.Combine(basePath, "Managers");
                case FolderType.Views:
                    return Path.Combine(basePath, "Views");
                case FolderType.ViewModels:
                    return Path.Combine(basePath, "ViewModels");
                case FolderType.ViewEntities:
                    return Path.Combine(basePath, "ViewEntities");
                case FolderType.Attributes:
                    return Path.Combine(basePath, "Attributes");
                case FolderType.Inputs:
                    return Path.Combine(basePath, "Inputs");
                case FolderType.Animators:
                    return Path.Combine(basePath, "Animators");
                case FolderType.Audio:
                    return Path.Combine(basePath, "Audio");
                case FolderType.AI:
                    return Path.Combine(basePath, "AI");
                case FolderType.Network:
                    return Path.Combine(basePath, "Network");
                case FolderType.Editor:
                    return Path.Combine(basePath, "Editor");
                case FolderType.Editor_Drawers:
                    return Path.Combine(Path.Combine(basePath, "Editor"), "Drawers");
                default:
                    return string.Empty;
            }
        }

        public static string CreateAssetFolder(string relPath, FolderType folderType)
        {
            string guid = "";

#if UNITY_EDITOR
            switch (folderType)
            {
                case FolderType.Models:
                    guid = AssetDatabase.CreateFolder(relPath, "Models");
                    break;
                case FolderType.Controllers:
                    guid = AssetDatabase.CreateFolder(relPath, "Controllers");
                    break;
                case FolderType.Components:
                    guid = AssetDatabase.CreateFolder(relPath, "Components");
                    break;
                case FolderType.DataControllers:
                    guid = AssetDatabase.CreateFolder(relPath, "DataControllers");
                    break;
                case FolderType.Managers:
                    guid = AssetDatabase.CreateFolder(relPath, "Managers");
                    break;
                case FolderType.Views:
                    guid = AssetDatabase.CreateFolder(relPath, "Views");
                    break;
                case FolderType.ViewModels:
                    guid = AssetDatabase.CreateFolder(relPath, "ViewModels");
                    break;
                case FolderType.ViewEntities:
                    guid = AssetDatabase.CreateFolder(relPath, "ViewEntities");
                    break;
                case FolderType.Attributes:
                    guid = AssetDatabase.CreateFolder(relPath, "Attributes");
                    break;
                case FolderType.Inputs:
                    guid = AssetDatabase.CreateFolder(relPath, "Inputs");
                    break;
                case FolderType.Animators:
                    guid = AssetDatabase.CreateFolder(relPath, "Animators");
                    break;
                case FolderType.Audio:
                    guid = AssetDatabase.CreateFolder(relPath, "Audio");
                    break;
                case FolderType.AI:
                    guid = AssetDatabase.CreateFolder(relPath, "AI");
                    break;
                case FolderType.Network:
                    guid = AssetDatabase.CreateFolder(relPath, FolderType.Network.ToString());
                    break;
                case FolderType.Editor:
                    guid = AssetDatabase.CreateFolder(relPath, "Editor");
                    break;
                case FolderType.Editor_Drawers:
                    // Call after an Editor folder creation
                    guid = AssetDatabase.CreateFolder(relPath, "Drawers");
                    break;
                default:
                    break;
            }

            return AssetDatabase.GUIDToAssetPath(guid);
#else
            return guid;
#endif
        }
    }

    public enum FolderType
    {
        Models,
        Controllers,
        Components,
        DataControllers,
        Managers,
        Views,
        ViewModels,
        ViewEntities,
        Attributes,
        Inputs,
        Animators,
        Audio,
        AI,
        Network,
        Editor,
        Editor_Drawers
    }
}

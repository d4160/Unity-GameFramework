using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BetterDefines.Editor
{
    public static class BetterDefinesUtils
    {
        /// <summary>
        ///     Creates .asset file of the specified <see cref="UnityEngine.ScriptableObject" />
        /// </summary>
        public static void CreateAsset<T>(string baseName, string forcedPath = "") where T : ScriptableObject
        {
            if (baseName.Contains("/"))
                throw new ArgumentException("Base name should not contain slashes");

            var asset = ScriptableObject.CreateInstance<T>();

            string path;
            if (!string.IsNullOrEmpty(forcedPath))
            {
                path = forcedPath;
                Directory.CreateDirectory(forcedPath);
            }
            else
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (string.IsNullOrEmpty(path))
                {
                    path = "Assets";
                }
                else if (Path.GetExtension(path) != string.Empty)
                {
                    path = path.Replace(Path.GetFileName(path), string.Empty);
                }
            }

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + baseName + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        #region defines
        public static void ToggleDefine(string define, bool enable, params BuildTargetGroup[] supportedPlatforms)
        {
            foreach (var targetPlatform in supportedPlatforms)
            {
                ToggleDefine(define, enable, targetPlatform);
            }
        }

        public static void ToggleDefine(string define, bool enable, BuildTargetGroup targetPlatform)
        {
            var scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetPlatform);
            var flags = new List<string>(scriptDefines.Split(';'));

            if (flags.Contains(define))
            {
                if (!enable)
                {
                    flags.Remove(define);
                }
            }
            else
            {
                if (enable)
                {
                    flags.Add(define);
                }
            }

            var result = string.Join(";", flags.ToArray());

            if (scriptDefines != result)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, result);
            }
        }

        public static void RemoveDefineFromAll(string define)
        {
            ToggleDefine(define, false, GetAllAvailablePlatforms());
        }

        public static void AddDefineToAll(string define)
        {
            ToggleDefine(define, true, GetAllAvailablePlatforms());
        }

        private static BuildTargetGroup[] GetAllAvailablePlatforms()
        {
            var allPlatforms = Enum.GetValues(typeof (BuildTargetGroup)).Cast<BuildTargetGroup>().ToList();
            allPlatforms.Remove(BuildTargetGroup.Unknown);
            return allPlatforms.ToArray();
        }

        public static string[] GetDefines(BuildTargetGroup targetGroup)
        {
            var scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            return scriptDefines.Split(';');
        }

        public static string[] GetSelectedTargetGroupDefines()
        {
            return GetDefines(EditorUserBuildSettings.selectedBuildTargetGroup);
        }
        #endregion
    }
}
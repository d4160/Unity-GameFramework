#if UNITY_4_4 || UNITY_4_5 || UNITY_4_5 || UNITY_4_7
#define UNITY_4
#endif

#if UNITY_4 || (UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3_0 || UNITY_5_3_1)
#define UNITY_PRE_5_3_2
#endif

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_PRE_5_3_0
#endif

#if !UNITY_PRE_5_3_0
#define UNITY_5_3_0_AND_LATER
#endif

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BetterDefines.Editor.Entity;
using UnityEditor;
using UnityEngine;

namespace BetterDefines.Editor
{
    [InitializeOnLoad]
    public static class PlatformUtils
    {
        public const string WEB_PLAYER_PLATFORM_ID = "Web";
        public const string STANDALONE_PLATFORM_ID = "Standalone";
        public const string ANDROID_PLATFORM_ID = "Android";
        public const string IOS_PLATFORM_ID = "iPhone";
        public const string BLACKBERRY_PLATFORM_ID = "BlackBerry";
        public const string TIZEN_PLATFORM_ID = "Tizen";
        public const string XBOX360_PLATFORM_ID = "XBox360";
        public const string XBOX_ONE_PLATFORM_ID = "XboxOne";
        public const string PS3_PLATFORM_ID = "PS3";
        public const string PS_VITA_PLATFORM_ID = "PSP2";
        public const string PS4_PLATFORM_ID = "PS4";
        public const string WINDOWS_STORE_PLATFORM_ID = "Metro";
        public const string WP8_PLATFORM_ID = "WP8";
        public const string WEB_GL_PLATFORM_ID = "WebGL";
        public const string SAMSUNG_TV_PLATFORM_ID = "SamsungTV";
        public const string NINTENDO_3DS_PLATFORM_ID = "N3DS";
        public const string TV_OS_PLATFORM_ID = "tvOS";
        public const string WIIU_PLATFORM_ID = "WiiU";
        public const string SWITCH_PLATFORM_ID = "Switch";

        private static readonly List<BuildPlatform> AvailableBuildPlatforms;
        private static readonly Dictionary<string, BuildTargetGroup> BuildTargetGroups;

        static PlatformUtils()
        {
            AvailableBuildPlatforms = InitAvailableBuildPlatforms();
            BuildTargetGroups = InitBuildTargetGroupsDic();
        }

        private static List<BuildPlatform> InitAvailableBuildPlatforms()
        {
            return new List<BuildPlatform>
            {
                new BuildPlatform("PC, Mac & Linux Standalone", STANDALONE_PLATFORM_ID, true, LoadPlatformIcon(STANDALONE_PLATFORM_ID)),
                new BuildPlatform("Android", ANDROID_PLATFORM_ID, true, LoadPlatformIcon(ANDROID_PLATFORM_ID)),
                new BuildPlatform("iOS", IOS_PLATFORM_ID, true, LoadPlatformIcon(IOS_PLATFORM_ID)),
#if UNITY_5_3_0_AND_LATER
                new BuildPlatform("tvOS", TV_OS_PLATFORM_ID, true, LoadPlatformIcon(TV_OS_PLATFORM_ID)),
#endif
                new BuildPlatform("Xbox One", XBOX_ONE_PLATFORM_ID, true, LoadPlatformIcon(XBOX_ONE_PLATFORM_ID)),
                new BuildPlatform("PS4", PS4_PLATFORM_ID, true, LoadPlatformIcon(PS4_PLATFORM_ID)),
#if UNITY_5_3_0_AND_LATER
                new BuildPlatform("Switch", SWITCH_PLATFORM_ID, true, LoadPlatformIcon(SWITCH_PLATFORM_ID)),
#endif

                new BuildPlatform("Windows Store", WINDOWS_STORE_PLATFORM_ID, true, LoadPlatformIcon(WINDOWS_STORE_PLATFORM_ID)),
                new BuildPlatform("WebGL", WEB_GL_PLATFORM_ID, true, LoadPlatformIcon(WEB_GL_PLATFORM_ID)),
            };
        }

        private static Dictionary<string, BuildTargetGroup> InitBuildTargetGroupsDic()
        {
            return new Dictionary<string, BuildTargetGroup>
            {
                {STANDALONE_PLATFORM_ID, BuildTargetGroup.Standalone},
                {ANDROID_PLATFORM_ID, BuildTargetGroup.Android},
#if !UNITY_PRE_5_3_2
                {TV_OS_PLATFORM_ID, BuildTargetGroup.tvOS},
                {SWITCH_PLATFORM_ID, BuildTargetGroup.Switch},
#endif

                {IOS_PLATFORM_ID, BuildTargetGroup.iOS},

                {XBOX_ONE_PLATFORM_ID, BuildTargetGroup.XboxOne},
                {PS4_PLATFORM_ID, BuildTargetGroup.PS4},
                {WINDOWS_STORE_PLATFORM_ID, BuildTargetGroup.WSA},
                {WEB_GL_PLATFORM_ID, BuildTargetGroup.WebGL},
            };
        }

        public static ReadOnlyCollection<BuildPlatform> AllAvailableBuildPlatforms
        {
            get { return AvailableBuildPlatforms.AsReadOnly(); }
        }

        private static Texture2D LoadPlatformIcon(string iconId)
        {
            return EditorGUIUtility.IconContent(string.Format("BuildSettings.{0}.Small", iconId)).image as Texture2D;
        }

        public static BuildTargetGroup GetBuildTargetGroupById(string platformId)
        {
            if (!platformId.IsValidBuildPlatformId())
            {
                throw new ArgumentException("Invalid platform id");
            }
            return BuildTargetGroups.ContainsKey(platformId) ? BuildTargetGroups[platformId] : BuildTargetGroup.Unknown;
        }

        public static string GetIdByBuildTargetGroup(BuildTargetGroup targetGroup)
        {
            if (targetGroup == BuildTargetGroup.Unknown) return null;
            
            return BuildTargetGroups.FirstOrDefault((x) => x.Value == targetGroup).Key;
        }
    }
}
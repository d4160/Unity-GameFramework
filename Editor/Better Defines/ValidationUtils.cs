using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace BetterDefines.Editor
{
    public static class ValidationUtils
    {
        private static readonly CodeDomProvider Provider = CodeDomProvider.CreateProvider("C#");
        private static readonly List<string> AllPossiblePlatforms = new List<string>
        {
            PlatformUtils.WEB_PLAYER_PLATFORM_ID,
            PlatformUtils.STANDALONE_PLATFORM_ID,
            PlatformUtils.ANDROID_PLATFORM_ID,
            PlatformUtils.IOS_PLATFORM_ID,
            PlatformUtils.BLACKBERRY_PLATFORM_ID,
            PlatformUtils.TIZEN_PLATFORM_ID,
            PlatformUtils.XBOX360_PLATFORM_ID,
            PlatformUtils.XBOX_ONE_PLATFORM_ID,
            PlatformUtils.PS3_PLATFORM_ID,
            PlatformUtils.PS_VITA_PLATFORM_ID,
            PlatformUtils.PS4_PLATFORM_ID,
            PlatformUtils.WINDOWS_STORE_PLATFORM_ID,
            PlatformUtils.WP8_PLATFORM_ID,
            PlatformUtils.WEB_GL_PLATFORM_ID,
            PlatformUtils.SAMSUNG_TV_PLATFORM_ID,
            PlatformUtils.NINTENDO_3DS_PLATFORM_ID,
            PlatformUtils.TV_OS_PLATFORM_ID,
            PlatformUtils.WIIU_PLATFORM_ID,
            PlatformUtils.SWITCH_PLATFORM_ID
        };

        public static bool IsValidDefineName(this string defineName)
        {
            return Provider.IsValidIdentifier(defineName);
        }

        public static bool IsValidBuildPlatformId(this string platformId)
        {
            return !String.IsNullOrEmpty(platformId) && AllPossiblePlatforms.Contains(platformId);
        }
    }
}
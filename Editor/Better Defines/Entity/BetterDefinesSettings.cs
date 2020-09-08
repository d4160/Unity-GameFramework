using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BetterDefines.Editor.Entity
{
    public class BetterDefinesSettings : ScriptableObject
    {
        public const string SETTINGS_RESOURCE_NAME = "BetterDefinesSettings";

        private static BetterDefinesSettings _instance;

        public List<CustomDefine> Defines;
        public List<PlatformEnabledState> EnabledPlatformsGlobal;

        public static BetterDefinesSettings Instance
        {
            get { return _instance ?? (_instance = Resources.Load<BetterDefinesSettings>(SETTINGS_RESOURCE_NAME)); }
        }

        public bool IsDefinePresent(string define)
        {
            return Defines.Any(x => x.Define == define);
        }

        #region global_platform_enables
        public PlatformEnabledState GetGlobalPlatformState(string platformId)
        {
            if (!platformId.IsValidBuildPlatformId())
            {
                throw new InvalidOperationException("Incorrect platform platformId: " + platformId);
            }

            if (EnabledPlatformsGlobal.SingleOrDefault(x => x.PlatformId == platformId) != null)
            {
                return EnabledPlatformsGlobal.Single(x => x.PlatformId == platformId);
            }
            var toAdd = new PlatformEnabledState(platformId, false);
            EnabledPlatformsGlobal.Add(toAdd);
            return EnabledPlatformsGlobal.Single(x => x.PlatformId == platformId);
        }

        public List<string> GetGlobalUserEnabledPlatformIds()
        {
            return EnabledPlatformsGlobal.Where(x => x.IsEnabled).ToList().ConvertAll(x => x.PlatformId);
        } 
        #endregion

        #region defines
        public void SetDefineState(string define, string platformId, bool state)
        {
            if (!platformId.IsValidBuildPlatformId())
            {
                throw new InvalidOperationException("Incorrect platform platformId: " + platformId);
            }

            var customDefine = Defines.SingleOrDefault(x => x.Define == define);
            if(customDefine == null)
            {
                customDefine = new CustomDefine(define);
                Defines.Add(customDefine);
            }

            customDefine.EnableForPlatform(platformId, state);
            EditorUtility.SetDirty(this);
        }

        public bool GetDefineState(string define, string platformId)
        {
            if (!platformId.IsValidBuildPlatformId())
            {
                throw new InvalidOperationException("Incorrect platform platformId: " + platformId);
            }

            var defineSymbol = Defines.SingleOrDefault(x => x.Define == define);
            return defineSymbol != null && defineSymbol.IsPlatformEnabled(platformId);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterDefines.Editor.Entity
{
    [System.Serializable]
    public class CustomDefine
    {
        public string Define;
        public List<PlatformEnabledState> StatesForPlatforms;

        public CustomDefine(string id)
        {
            StatesForPlatforms = new List<PlatformEnabledState>();
            foreach (var platform in PlatformUtils.AllAvailableBuildPlatforms)
            {
                StatesForPlatforms.Add(new PlatformEnabledState(platform.Id, false));
            }
        }

        public bool IsPlatformEnabled(string platformId)
        {
            if (string.IsNullOrEmpty(platformId))
            {
                throw new ArgumentNullException("Platform Id");
            }

            if (StatesForPlatforms.All(x => x.PlatformId != platformId))
            {
                //Debug.LogWarning("Platform id " + platformId + " not available. Adding it to " + Define);
                StatesForPlatforms.Add(new PlatformEnabledState(platformId, false));
            }

            return StatesForPlatforms.Single(x => x.PlatformId == platformId).IsEnabled;
        }

        public void EnableForPlatform(string platformId, bool isEnabled)
        {
            StatesForPlatforms.Single(x => x.PlatformId == platformId).IsEnabled = isEnabled;
        }
    }
}

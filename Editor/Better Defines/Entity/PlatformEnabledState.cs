using System;

namespace BetterDefines.Editor.Entity
{
    [Serializable]
    public class PlatformEnabledState
    {
        public string PlatformId;
        public bool IsEnabled;

        public PlatformEnabledState(string platformId, bool isEnabled)
        {
            PlatformId = platformId;
            IsEnabled = isEnabled;
        }
    }
}
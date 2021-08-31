using UnityEngine.Promise;

namespace d4160.Profile
{
    public interface IProfileService
    {
        /// <summary>
        /// Contract for profile service
        /// </summary>
        public interface IProfileService
        {
            string DisplayName { get; }

            void GetPlayerProfile(Completer completer);
        }
    }
}
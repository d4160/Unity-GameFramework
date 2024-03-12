using d4160.Collections;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Data/Multiplay")]
    public class MultiplayDataSO : ScriptableObject
    {
        [SerializeField] private string[] _fleetsId;

        /// <summary>
        /// The first FleetId of the list.
        /// </summary>
        /// <returns>Empty if there is any FleetId</returns>
        public string FleetId => GetFleetId(0);

        public string GetFleetId(int index)
        {
            if (_fleetsId.IsValidIndex(index))
            {
                return _fleetsId[index];
            }

            return string.Empty;
        }
    }
}
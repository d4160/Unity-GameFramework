using d4160.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/LobbyData")]
    public class LobbyDataSO : ScriptableObject
    {
        [ContextMenuItem("ClearIndex", "ClearIndex")]
        public LobbyData[] lobbyData;

        public void SetValue(int index, string value)
        {
            if (lobbyData.IsValidIndex(index))
            {
                lobbyData[index].value.StringValue = value;
            }
        }

#if UNITY_EDITOR
        private void ClearIndex()
        {
            for (int i = 0; i < lobbyData.Length; i++)
            {
                lobbyData[i].index = default;
            }
        }
#endif
    }
}

using d4160.Variables;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/LobbyParams")]
    public class LobbyParamsSO : ScriptableObject
    {
        public StringReference _name;
        [Range(1, 100)]
        public int maxPlayers = 10;
        public bool isPrivate = false;
    }
}
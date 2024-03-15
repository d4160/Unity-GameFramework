using System;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Data/ServerQueryOptions")]
    public class ServerQueryOptionsSO : ScriptableObject
    {
        [SerializeField] private ushort _maxPlayers;
        [SerializeField] private string _serverName;
        [SerializeField] private string _gameType;
        [SerializeField] private string _buildId;
        [SerializeField] private string _map;

        public ushort MaxPlayers => _maxPlayers;
        public string ServerName => _serverName;
        public string GameType => _gameType;
        public string BuildId => _buildId;
        public string Map => _map;
    }
}
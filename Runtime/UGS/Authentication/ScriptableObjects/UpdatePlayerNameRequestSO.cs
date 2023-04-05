using UnityEngine;
using d4160.Variables;
using Unity.Services.Authentication;
using System.Threading.Tasks;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.Authentication
{
    [CreateAssetMenu(menuName = "d4160/UGS/Authentication/UpdatePlayerName")]
    public class UpdatePlayerNameRequestSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _playerName;

        public StringVariableSO PlayerName => _playerName;

        public async Task<string> UpdatePlayerNameAsync()
        {
            return await AuthenticationService.Instance.UpdatePlayerNameAsync(_playerName.Value);
        }
    }
}
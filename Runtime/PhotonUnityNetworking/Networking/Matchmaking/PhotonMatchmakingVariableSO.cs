using d4160.Variables;
using UnityEngine;

namespace d4160.Networking.Photon
{
    [CreateAssetMenu(menuName = "d4160/Photon/Matchmaking Variable")]
    public class PhotonMatchmakingVariableSO : VariableSOBase<PhotonMatchmakingSO>
    {
        public string CurrentRoomName => _value?.CurrentRoomName;

        public void JoinRoom() => _value?.JoinRoom();
    }
}
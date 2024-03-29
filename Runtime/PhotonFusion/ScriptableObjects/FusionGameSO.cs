using Fusion;
using Fusion.Photon.Realtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Game")]
    public class FusionGameSO : ScriptableObject
    {
        public GameMode gameMode;
        public string sessionName;
        public int playerCount;
        public bool isOpen = true;
        public bool isVisible = true;
        public MatchmakingMode matchmakingMode = MatchmakingMode.FillRoom;
        public NetworkRunnerEventSO onInializedEventSO;
        public FusionPlayerSO playerSO;

        private static readonly FusionService _service = FusionService.Instance;

        [Button]
        public void StartGame()
        {
            // Start or join (depends on gamemode) a session with a specific name
            StartGame(gameMode);
        }

        public async void StartGame(GameMode mode)
        {
            // Start or join (depends on gamemode) a session with a specific name
            await _service.Runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = sessionName,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = _service.SceneManager,
                PlayerCount = playerCount,
                IsOpen = isOpen,
                IsVisible = isVisible,
                MatchmakingMode = matchmakingMode,
                Initialized = (runner) =>
                {
                    _service.LoggerSO.LogInfo($"OnInitialized; Tick={runner.Tick.Raw} SessionName={sessionName}");
                    if (onInializedEventSO) onInializedEventSO.Invoke(runner);
                },
                AuthValues = playerSO ? playerSO.AuthValues : null
            });
        }

        //[Button]
        //public void DisconnectLocal()
        //{
        //    PhotonFusionService.Runner.Disconnect(PhotonFusionService.Runner.LocalPlayer);
        //}

        [Button]
        public void Shutdown()
        {
            _service.Runner.Shutdown(false);
        }
    }
}
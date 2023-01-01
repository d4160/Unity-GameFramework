using Fusion;
using Fusion.Photon.Realtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/Game")]
public class PhotonFusionGameSO : ScriptableObject
{
    public GameMode gameMode;
    public string sessionName;
    public int playerCount;
    public bool isOpen = true;
    public bool isVisible = true;
    public MatchmakingMode matchmakingMode = MatchmakingMode.FillRoom;
    public NetworkRunnerEventSO onInializedEventSO;
    public PhotonFusionPlayerSO playerSO;

    public PhotonFusionService PhotonFusionService => PhotonFusionService.Instance;

    [Button]
    public void StartGame()
    {
        // Start or join (depends on gamemode) a session with a specific name
        StartGame(gameMode);
    }

    public async void StartGame(GameMode mode)
    {
        // Start or join (depends on gamemode) a session with a specific name
        await PhotonFusionService.Runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = PhotonFusionService.SceneManager,
            PlayerCount = playerCount,
            IsOpen = isOpen,
            IsVisible = isVisible,
            MatchmakingMode = matchmakingMode,
            Initialized = (runner) => {
                PhotonFusionService.LoggerSO.LogInfo($"OnInitialized; Tick={runner.Tick.Raw} SessionName={sessionName}");
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
        PhotonFusionService.Runner.Shutdown(false);
    }
}

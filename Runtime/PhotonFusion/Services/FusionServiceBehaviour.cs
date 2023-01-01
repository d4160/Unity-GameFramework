using d4160.Logging;
using d4160.MonoBehaviours;
using Fusion;
using UnityEngine;

namespace d4160.Fusion
{
    [RequireComponent(typeof(NetworkRunner))]
    [RequireComponent(typeof(NetworkSceneManagerDefault))]
    public class FusionServiceBehaviour : MonoBehaviourUnityData<FusionServiceSO>
    {
        private NetworkRunner _runner;
        private NetworkSceneManagerDefault _sceneManager;

        private void Awake()
        {
            _runner = GetComponent<NetworkRunner>();
            _sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }

        private void Start()
        {
            if (_data)
            {
                _data.SetLogger();
                _data.Runner = _runner;
                _data.SceneManager = _sceneManager;
            }
        }

        private void OnEnable()
        {
            if (_data)
                _data.RegisterEvents();
        }

        private void OnDisable()
        {
            if (_data)
                _data.UnregisterEvents();
        }
    }
}

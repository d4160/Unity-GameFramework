using UnityEngine;
using Dissonance;
using d4160.Variables;
using d4160.Events;

namespace d4160.Dissonance
{
    [RequireComponent(typeof(VoiceBroadcastTrigger))]
    public class DissonanceBroadcastMuter : MonoBehaviour
    {
        [SerializeField] private BoolVariableSO _isMutedVar;

        private VoiceBroadcastTrigger _broadcast;
        private BoolEventSO.EventListener _onIsMutedChanged;

        private void Awake()
        {
            _broadcast = GetComponent<VoiceBroadcastTrigger>();

            _onIsMutedChanged = new((val) =>
            {
                _broadcast.IsMuted = val;
            });
        }

        private void Start()
        {
            _broadcast.IsMuted = _isMutedVar;
        }

        private void OnEnable()
        {
            _isMutedVar.OnValueChange.AddListener(_onIsMutedChanged);
        }

        private void OnDisable()
        {
            _isMutedVar.OnValueChange.RemoveListener(_onIsMutedChanged);
        }
    }
}
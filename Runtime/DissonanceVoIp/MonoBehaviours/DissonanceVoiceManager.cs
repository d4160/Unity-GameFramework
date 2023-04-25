using d4160.Collections;
using d4160.Events;
using d4160.Singleton;
using d4160.Variables;
using UnityEngine;

namespace d4160.Dissonance
{
    public class DissonanceVoiceManager : Singleton<DissonanceVoiceManager>
    {
        [Header("References")]
        [SerializeField] private BroadcastReceiptPair[] _voicePairs;

        [Header("Data")]
        [SerializeField] private BoolVariableSO _isMuteVar;

        [Header("Start Configs")]
        [SerializeField, Range(-1, 10)] private int _enabledIndex = -1;
        [SerializeField] private bool _othersReceiptEnabled = false;
    
        private BoolEventSO.EventListener _onIsMuteChanged;

        protected override void Awake()
        {
            base.Awake();
            _onIsMuteChanged = new((val) => {
                if (_enabledIndex == -1) return;
                _voicePairs[_enabledIndex].SetIsMuteBroadcast(val);
            });
        }

        protected virtual void Start()
        {
            if (_enabledIndex != -1)
            {
                EnableVoicePair(_enabledIndex);
            }
        }

        private void OnEnable()
        {
            _isMuteVar.OnValueChange.AddListener(_onIsMuteChanged);
        }

        private void OnDisable()
        {
            _isMuteVar.OnValueChange.RemoveListener(_onIsMuteChanged);
        }

        public void EnableVoicePair(int index)
        {
            EnableVoicePair(index, true);
        }

        public void EnableVoicePair(int index, bool disableAll)
        {
            EnableVoicePair(index, disableAll, _othersReceiptEnabled);
        }

        public void EnableVoicePair(int index, bool disableAll, bool othersReceiptEnabled)
        {
            if (!_voicePairs.IsValidIndex(index)) return;

            _enabledIndex = index;
            if (disableAll) DisableAllVoicePairs(othersReceiptEnabled);
            _voicePairs[index].SetEnableAndIsMute(true, _isMuteVar.Value);
        }

        private void DisableAllVoicePairs(bool othersReceiptEnabled = false)
        {
            for (int i = 0; i < _voicePairs.Length; i++)
            {
                _voicePairs[i].SetEnableAndIsMute(othersReceiptEnabled, true);
            }
        }

        public void SetEnableVoiceReceipt(int index, bool enabled)
        {
            if (!_voicePairs.IsValidIndex(index)) return;

            _voicePairs[index].SetEnableReceipt(enabled);
        }
    }
}
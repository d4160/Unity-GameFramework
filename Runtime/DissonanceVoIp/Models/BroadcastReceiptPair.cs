using Dissonance;
using UnityEngine;

namespace d4160.Dissonance
{
    [System.Serializable]
    public class BroadcastReceiptPair
    {
        [SerializeField] private VoiceBroadcastTrigger _broadcast;
        [SerializeField] private VoiceReceiptTrigger _receipt;

        public void SetIsMuteBroadcast(bool isMute)
        {
            _broadcast.IsMuted = isMute;
        }

        public void SetEnableReceipt(bool enabled)
        {
            _receipt.enabled = enabled;
        }

        public void SetEnableAndIsMute(bool enabled, bool isMute)
        {
            SetEnableReceipt(enabled);
            SetIsMuteBroadcast(isMute);
        }
    }
}
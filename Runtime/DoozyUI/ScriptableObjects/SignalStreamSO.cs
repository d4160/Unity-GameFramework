using Doozy.Runtime.Signals;
using UnityEngine;

namespace d4160.DoozyUI
{
    [CreateAssetMenu(menuName = "d4160/Doozy UI/SignalStream")]
    public class SignalStreamSO : ScriptableObject
    {
        public string category;
        public string _name;
        public SignalEventSO onSignal;

        private SignalReceiver _signalReceiver; // Signal receiver
        private SignalStream _signalStream;

        private SignalStream SignalStream
        {
            get
            {
                _signalStream ??= SignalStream.Get(category, _name);
                return _signalStream;
            }
        }

        // Call in Awake
        public void Setup()
        {
            _signalReceiver = new SignalReceiver().SetOnSignalCallback((s) => {
                if (onSignal) onSignal.Invoke(s);
            });
        }

        public void RegisterEvents()
        {
            //add the receiver to react to signals sent through the stream
            SignalStream.ConnectReceiver(_signalReceiver);
        }

        public void UnregisterEvents()
        {
            SignalStream.DisconnectReceiver(_signalReceiver);
        }

        public void SendSignal(string message = "")
        {
            SignalStream.SendSignal(message);
        }

        public void SendSignal<T>(T val, string message = "")
        {
            SignalStream.SendSignal(val, message);
        }
    }
}
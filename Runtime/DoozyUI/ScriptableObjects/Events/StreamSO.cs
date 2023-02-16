using Doozy.Runtime.Signals;
using UnityEngine;

namespace d4160.DoozyUI
{
    [CreateAssetMenu(menuName = "d4160/Doozy UI/Stream")]
    public class StreamSO : ScriptableObject
    {
        public string category;
        public string _name;
        public SignalEventSO onSignal;

        private SignalReceiver _signalReceiver; // Signal receiver
        private SignalStream _signalStream;

        public void Setup()
        {
            //get a reference to the target stream
            _signalStream = SignalStream.Get(category, _name);
            //initialize the receiver and set its callback
            _signalReceiver = new SignalReceiver().SetOnSignalCallback((s) => {
                if (onSignal) onSignal.Invoke(s);
            });
        }

        public void RegisterEvents()
        {
            //add the receiver to react to signals sent through the stream
            _signalStream.ConnectReceiver(_signalReceiver);
        }

        public void UnregisterEvents()
        {
            _signalStream.DisconnectReceiver(_signalReceiver);
        }
    }
}
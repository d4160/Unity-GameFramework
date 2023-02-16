using Doozy.Runtime.Signals;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class GenericUISignalListener : MonoBehaviour
{
    public string StreamCategory;            // Target stream category
    public string StreamName;                // Target stream name
    private SignalReceiver m_SignalReceiver; // Signal receiver
    private SignalStream m_SignalStream;     // Target stream

    public UltEvent onReceive;

    private void Awake()
    {
        //get a reference to the target stream
        m_SignalStream = SignalStream.Get(StreamCategory, StreamName);
        //initialize the receiver and set its callback
        m_SignalReceiver = new SignalReceiver().SetOnSignalCallback(OnSignal);
    }
    private void OnEnable()
    {
        //add the receiver to react to signals sent through the stream
        m_SignalStream.ConnectReceiver(m_SignalReceiver);
    }
    private void OnDisable()
    {
        //remove the receiver from reacting to signals sent through the stream
        m_SignalStream.DisconnectReceiver(m_SignalReceiver);
    }
    private void OnSignal(Signal signal)
    {

        //do stuff
        //here you can also extract info from the signal
        onReceive.InvokeX();
    }
}

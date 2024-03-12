using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BasicMicrophoneCapture : MonoBehaviour
{
    public float delayToStartMic = 1f;
    public float waitToRestart = 10f;
    public AudioSource echoASource;

    private AudioSource _aSource;
    private AudioClip _microphoneClip;
    private WaitForSeconds _delay, _waitForRestart;

    private WaitForSeconds Delay => _delay ??= new WaitForSeconds(delayToStartMic);
    private WaitForSeconds WaitForRestart => _waitForRestart ??= new WaitForSeconds(waitToRestart);

    public bool IsRecording
    {
        get { return _microphoneClip != null; }
    }

    private void Awake()
    {
        _aSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //StartMicrophone(true);

        //StartCoroutine(RestartMicCo(true));
    }

    [Button]
    public void StartMicrophone()
    {
        StartMicrophone(false);
    }

    public void StartMicrophone(bool delay)
    {
        StartCoroutine(StartMicrophoneCo(delay));
    }

    [Button]
    public void StopMicrophone()
    {
        //UNITY_WEBGL
        //Microphone.End(Microphone.devices[0]);

        //if (_microphoneClip != null)
        //{
        //    Destroy(_microphoneClip);
        //    _microphoneClip = null;
        //}

        _aSource.Stop();
    }

    private IEnumerator StartMicrophoneCo(bool delay = false)
    {
        if (delay)
            yield return Delay;

        // UNITY_WEBGL

        // string microphoneName = Microphone.devices[0];
        // _microphoneClip = Microphone.Start(microphoneName, true, 10, AudioSettings.outputSampleRate);

        // _aSource.clip = _microphoneClip;
        // _aSource.loop = true;

        // while (!(Microphone.GetPosition(microphoneName) > 0))
        // {
        //     yield return null;
        // }

        // _aSource.Play();
    }

    private IEnumerator RestartMicCo(bool delay = false)
    {
        if (delay)
            yield return Delay;

        while (true)
        {
            yield return WaitForRestart;

            //Debug.Log($"Samples: {_microphoneClip.samples}");
            StopMicrophone();
            StartMicrophone();
        }
    }

    public float[] GetClipData(bool echo, out int frequency, out int channels)
    {
        var data = GetTrimmedData();
        if (echo)
        {
            var echoClip = AudioClip.Create("echo", data.Length,
                _microphoneClip.channels, _microphoneClip.frequency, false);
            echoClip.SetData(data, 0);
            echoASource.clip = echoClip;
            echoASource.Play();
        }

        frequency = _microphoneClip.frequency;
        channels = _microphoneClip.channels;

        return data;
    }

    private float[] GetTrimmedData()
    {
        // UNITY_WEBGL
        return default;

        // get microphone samples and current position
        // var pos = Microphone.GetPosition(null);
        // var origData = new float[_microphoneClip.samples * _microphoneClip.channels];
        // _microphoneClip.GetData(origData, 0);

        // // check if mic just reached audio buffer end
        // if (pos == 0)
        //     return origData;

        // // looks like we need to trim it by pos
        // var trimData = new float[pos];
        // Array.Copy(origData, trimData, pos);
        // return trimData;
    }
}

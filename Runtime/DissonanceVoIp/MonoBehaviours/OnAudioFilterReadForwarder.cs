using d4160.Events;
using d4160.Variables;
using UnityEngine.Serialization;
using UnityEngine;
using NaughtyAttributes;

public class OnAudioFilterReadForwarder : MonoBehaviour, IEventListener<bool>
{
    public enum MuteState
    {
        None,
        Before,
        After
    }
    [SerializeField, FormerlySerializedAs("_muteBehaviour")] private MuteState _muteState;
    [Range(0f, 7f)]
    [SerializeField, FormerlySerializedAs("amplitudeMultiplier")] private float _amplitudeMultiplier = 1f;
    [SerializeField] private AudioFilterReadEventSO _audioFilterReadEvent;

    [Header("Variables")]
    [SerializeField, Expandable] private BoolVariableSO _isMutedVar;

    private Dissonance.Audio.Capture.BasicMicrophoneCapture _micCapture;
    private float _originalAmplitude;

    private void Awake()
    {
        _originalAmplitude = _amplitudeMultiplier;

        _micCapture = GetComponent<Dissonance.Audio.Capture.BasicMicrophoneCapture>();
    }

    private void Start()
    {
        if (_isMutedVar != null)
        {
            _muteState = _isMutedVar.Value ? MuteState.Before : MuteState.After;
        }

        if (_micCapture)
        {
            _micCapture.OnStartCapture += (s) =>
            {
                //Debug.Log($"[onStartCapture] Microphone: {s}");
                if (!string.IsNullOrEmpty(s) && s.Contains("Stereo Mix"))
                {
                    _amplitudeMultiplier = 1f;
                }
                else
                {
                    _amplitudeMultiplier = _originalAmplitude;
                }
            };
        }
    }

    private void OnEnable()
    {
        if (_isMutedVar != null && _isMutedVar.OnValueChange)
        {
            _isMutedVar.OnValueChange.AddListener(this);
        }
    }

    private void OnDisable()
    {
        if (_isMutedVar != null && _isMutedVar.OnValueChange)
        {
            _isMutedVar.OnValueChange.RemoveListener(this);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //Debug.Log($"[OnAudioFilterRead] DataLength: {data.Length}; Channels: {channels}");
        if (_muteState == MuteState.Before)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Mathf.Clamp(data[i] * _amplitudeMultiplier, -1f, 1f);
            }
        }
        if (_audioFilterReadEvent) _audioFilterReadEvent.Invoke(data, channels);
        if (_muteState == MuteState.After)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }
    }

    public void OnInvoked(bool isMuted)
    {
        _muteState = isMuted ? MuteState.Before : MuteState.After;
    }
}
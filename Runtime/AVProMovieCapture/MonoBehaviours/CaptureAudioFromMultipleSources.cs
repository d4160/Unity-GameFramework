    using RenderHeads.Media.AVProMovieCapture;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using d4160.Events;

    public class CaptureAudioFromMultipleSources :
        UnityAudioCapture, IEventListener<float[], int>
    {
        [SerializeField] bool _debugLogging = false;
        [SerializeField] bool _muteAudio = false;
        public AudioFilterReadEventSO _onAudioFilterReadEvent;

        private const int BufferSize = 16;
        private float[] _buffer = new float[0];
        private float[] _readBuffer = new float[0];
        private int _bufferIndex;
        private GCHandle _bufferHandle;
        private int _numChannels;

        private int _overflowCount;
        private object _lockObject = new object();

        public float[] Buffer { get { return _readBuffer; } }
        public int BufferLength { get { return _bufferIndex; } }
        public System.IntPtr BufferPtr { get { return _bufferHandle.AddrOfPinnedObject(); } }

        public override int OverflowCount { get { return _overflowCount; } }
        public override int ChannelCount { get { return _numChannels; } }

        public override int SampleRate => AudioSettings.outputSampleRate;

        private void OnEnable()
        {
            _onAudioFilterReadEvent.AddListener(this);
            //foreach (var onAudioFilterReadForwarder in _OnAudioFilterReadForwarders)
            //{
            //    onAudioFilterReadForwarder.Callback += OnAudioFilterReadCombiner;
            //}
        }

    private void OnDisable()
    {
        _onAudioFilterReadEvent.RemoveListener(this);
    }

    public override void PrepareCapture()
        {
            int bufferLength = 0;
            int numBuffers = 0;
            AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
            _numChannels = GetUnityAudioChannelCount();
            if (_debugLogging)
            {
                Debug.Log(string.Format("[UnityAudioCapture] SampleRate: {0}hz SpeakerMode: {1} BestDriverMode: {2} (DSP using {3} buffers of {4} bytes using {5} channels)", AudioSettings.outputSampleRate, AudioSettings.speakerMode.ToString(), AudioSettings.driverCapabilities.ToString(), numBuffers, bufferLength, _numChannels));
            }

            _buffer = new float[bufferLength * _numChannels * numBuffers * BufferSize];
            _readBuffer = new float[bufferLength * _numChannels * numBuffers * BufferSize];
            _bufferIndex = 0;
            _bufferHandle = GCHandle.Alloc(_readBuffer, GCHandleType.Pinned);
            _overflowCount = 0;
        }

    public override void StartCapture()
    {
        FlushBuffer();
    }

    public override void StopCapture()
    {
        lock (_lockObject)
        {
            _bufferIndex = 0;
            if (_bufferHandle.IsAllocated)
                _bufferHandle.Free();
            _readBuffer = _buffer = null;
        }
        _numChannels = 0;
    }

    private bool _paused = false;

    public override void PauseCapture()
    {
        if (!_paused)
        {
            _paused = true;
        }
    }

    public override void ResumeCapture()
    {
        if (_paused)
        {
            _paused = false;
            lock (_lockObject)
            {
                FlushBuffer();
            }
        }
    }

    public override System.IntPtr ReadData(out int length)
    {
        lock (_lockObject)
        {
            System.Array.Copy(_buffer, 0, _readBuffer, 0, _bufferIndex);
            length = _bufferIndex;
            _bufferIndex = 0;
        }
        return _bufferHandle.AddrOfPinnedObject();
    }

    public override void FlushBuffer()
    {
        lock (_lockObject)
        {
            _bufferIndex = 0;
            _overflowCount = 0;
        }
    }
    int _count = 0;
    int _lastLength;
    private void OnAudioFilterReadCombiner(float[] data, int channels)
    {
        if (_paused)
            return;

        if (_buffer != null && _buffer.Length > 0)
        {
            _count++;
            // TODO: use double buffering
            lock (_lockObject)
            {
                int length = Mathf.Min(data.Length, _buffer.Length - _bufferIndex);

                if (!_muteAudio)
                {
                    if (_count == 1)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            _buffer[i + _bufferIndex] = data[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++)
                        {
                            _buffer[i + _bufferIndex] += data[i];
                        }
                        _count = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        _buffer[i + _bufferIndex] = data[i];
                        data[i] = 0f;
                    }
                }
                if (_count == 0)
                {
                    _bufferIndex += length;
                    _lastLength = length;
                }

                if (length < data.Length)
                {
                    _overflowCount++;
                    Debug.LogWarning("[AVProMovieCapture] Audio buffer overflow, may cause sync issues.  Disable this component if not recording Unity audio.");
                }
            }

        }
    }

    public void OnInvoked(float[] data, int channels)
    {
        OnAudioFilterReadCombiner(data, channels);
    }
}
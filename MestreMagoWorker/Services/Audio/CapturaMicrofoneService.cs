using NAudio.Wave;

namespace MestreMagoWorker.Services.Audio
{
    public class CapturaMicrofoneService
    {
        private WaveInEvent? _waveInEvent;
        private BufferedWaveProvider? _bufferedWaveProvider;

        public event Action<byte[]>? OnData;
        public WaveFormat WaveFormat { get; private set; }

        public CapturaMicrofoneService()
        {
            WaveFormat = new WaveFormat(16000, 1);
        }

        public void Start()
        {
            if (_waveInEvent != null)
                return;

            _waveInEvent = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = WaveFormat
            };

            _bufferedWaveProvider = new BufferedWaveProvider(WaveFormat)
            {
                DiscardOnBufferOverflow = true
            };

            _waveInEvent.DataAvailable += (s, e) =>
            {
                var buffer = new byte[e.BytesRecorded];
                Array.Copy(e.Buffer, buffer, e.BytesRecorded);

                _bufferedWaveProvider?.AddSamples(buffer, 0, e.BytesRecorded);
                OnData?.Invoke(buffer);
            };

            _waveInEvent.RecordingStopped += (s, e) =>
            {
                _waveInEvent?.Dispose();
                _waveInEvent = null;
            };

            _waveInEvent.StartRecording();
        }

        public void Stop()
        {
            if (_waveInEvent != null)
            {
                _waveInEvent.StopRecording();
                _waveInEvent.Dispose();
                _waveInEvent = null;
            }
        }

        public static int GetDeviceCount() => WaveInEvent.DeviceCount;

        public static string? GetDeviceName(int deviceIndex)
        {
            if (deviceIndex < 0 || deviceIndex >= WaveInEvent.DeviceCount)
                return null;

            var capabilities = WaveInEvent.GetCapabilities(deviceIndex);
            return capabilities.ProductName;
        }
    }
}

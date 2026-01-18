using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace MestreMagoWorker.Services.Audio
{
    public class CapturaAudioService
    {
        private WasapiLoopbackCapture _capture;
        public event Action<byte[]> OnData;
        public void Start()
        {
            _capture = new WasapiLoopbackCapture();
            _capture.DataAvailable += (s, a) => 
            {

                var buffer = new byte[a.BytesRecorded];
                Array.Copy(a.Buffer, buffer, a.BytesRecorded);

                OnData?.Invoke(buffer);
            };
            _capture.RecordingStopped += (s, e) => { _capture.Dispose(); };
            _capture.StartRecording();
        }


        public void Stop() => _capture?.StopRecording();
    }
}

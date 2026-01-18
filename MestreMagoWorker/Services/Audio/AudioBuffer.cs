using System;
using System.Collections.Generic;
using System.Text;

namespace MestreMagoWorker.Services.Audio
{
    public class AudioBuffer
    {
        private readonly MemoryStream _buffer = new();
        private readonly int _bytesPerSecond;
        private readonly int _windowSeconds;

        public AudioBuffer(int bytesPerSecond, int windowSeconds = 3)
        {
            _bytesPerSecond = bytesPerSecond;
            _windowSeconds = windowSeconds;
        }

        public bool Adicionar(byte[] chunk)
        {
            _buffer.Write(chunk, 0, chunk.Length);
            return _buffer.Length >= _bytesPerSecond * _windowSeconds;
        }

        public byte[] Consumo()
        {
            var data = _buffer.ToArray();
            _buffer.SetLength(0);
            return data;
        }
    }
}

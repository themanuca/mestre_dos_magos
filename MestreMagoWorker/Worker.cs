using MestreMagoWorker.Services.Audio;
using Microsoft.Toolkit.Uwp.Notifications;
using NAudio.Wave;

namespace MestreMagoWorker
{
    public class Worker(ILogger<Worker> logger, CapturaAudioService capturaAudioService, AudioBuffer audioBuffer) : BackgroundService
    {
        private readonly CapturaAudioService capturaAudioService = capturaAudioService;
        private readonly AudioBuffer audioBuffer = audioBuffer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          
            capturaAudioService.OnData += chuck =>
            {
                if (audioBuffer.Adicionar(chuck))
                {
                           
                    var consumo = audioBuffer.Cosnumo();

                    var format = new WaveFormat(16000, 1);
                    var wav = ConvertToWav(consumo, format);
                }
                Console.WriteLine($"Audio recebido:{chuck.Length} bytes");
            };
            capturaAudioService.Start();
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                
            await Task.Delay(1000, stoppingToken);
            
        }
        public byte[] ConvertToWav(byte[] pcmData, WaveFormat inputFormat)
        {
            using var sourceStream = new RawSourceWaveStream(
                new MemoryStream(pcmData), inputFormat);

            var targetFormat = new WaveFormat(16000, 1);

            using var resampler = new MediaFoundationResampler(sourceStream, targetFormat);
            using var output = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(output, resampler);

            return output.ToArray();
        }

    }
}

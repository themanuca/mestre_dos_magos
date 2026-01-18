using MestreMagoWorker.Services.Audio;
using Microsoft.Toolkit.Uwp.Notifications;
using NAudio.Wave;

namespace MestreMagoWorker
{
    public class Worker(
        ILogger<Worker> logger,
        CapturaAudioService capturaAudioService,
        CapturaMicrofoneService capturaMicrofoneService,
        AudioBuffer audioBuffer) : BackgroundService
    {
        private readonly CapturaAudioService _capturaAudioService = capturaAudioService;
        private readonly CapturaMicrofoneService _capturaMicrofoneService = capturaMicrofoneService;
        private readonly AudioBuffer _audioBuffer = audioBuffer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Iniciando captura de áudio do sistema e microfone...");

                _capturaAudioService.OnData += ProcessarAudioSistema;

                _capturaMicrofoneService.OnData += ProcessarAudioMicrofone;

                _capturaAudioService.Start();
                logger.LogInformation("Captura de áudio do sistema iniciada");

                _capturaMicrofoneService.Start();
                logger.LogInformation("Captura de áudio do microfone iniciada (Dispositivo 0: {device})",
                    CapturaMicrofoneService.GetDeviceName(0) ?? "Desconhecido");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Serviço de captura de áudio cancelado");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro na captura de áudio: {message}", ex.Message);
                throw;
            }
            finally
            {
                _capturaAudioService.Stop();
                _capturaMicrofoneService.Stop();
                logger.LogInformation("Captura de áudio finalizada");
            }
        }

        private void ProcessarAudioSistema(byte[] chunk)
        {
            if (_audioBuffer.Adicionar(chunk))
            {
                var consumo = _audioBuffer.Consumo();
                var format = new WaveFormat(16000, 1);
                var wav = ConvertToWav(consumo, format);

                logger.LogDebug("Áudio do sistema processado: {bytes} bytes → {wavBytes} bytes WAV",
                    consumo.Length, wav.Length);
            }
        }

        private void ProcessarAudioMicrofone(byte[] chunk)
        {
            logger.LogDebug("Áudio do microfone recebido: {bytes} bytes", chunk.Length);
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

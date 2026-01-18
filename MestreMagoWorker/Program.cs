using MestreMagoWorker;
using MestreMagoWorker.Services;
using MestreMagoWorker.Services.Audio;

var host = Host.CreateDefaultBuilder(args)
.ConfigureServices((ctx, services) => {
    services.AddHostedService<Worker>();
    services.AddSingleton<CapturaAudioService>();
    services.AddSingleton(new AudioBuffer(bytesPerSecond: 16000, windowSeconds: 3));
})
.Build();
await host.RunAsync();

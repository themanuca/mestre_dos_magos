using MestreMagoWorker;
using MestreMagoWorker.Services;
using MestreMagoWorker.Services.Audio;
using MestreMagoWorker.Services.Prompts;
using MestreMagoWorker.Services.LLM;
using MestreMagoWorker.Services.Models;
using MestreMagoWorker.Services.Orchestration;
using MestreMagoWorker.Services.Session;

var host = Host.CreateDefaultBuilder(args)
.ConfigureServices((ctx, services) => {
    services.AddHostedService<Worker>();
    services.AddSingleton<CapturaAudioService>();
    services.AddSingleton<CapturaMicrofoneService>();
    services.AddSingleton(new AudioBuffer(bytesPerSecond: 16000, windowSeconds: 3));

    // Core SaaS services
    services.AddSingleton<PromptRepository>();
    services.AddSingleton<ModelSelector>();
    services.AddSingleton<SessionManager>();
    services.AddSingleton<ILLMClient, OpenAIClient>(); // trocar pelo cliente real
    services.AddSingleton<ModelOrchestrator>();
})
.Build();
await host.RunAsync();

using System.Threading;
using System.Threading.Tasks;
using MestreMagoWorker.Services.Models;

namespace MestreMagoWorker.Services.LLM
{
    public interface ILLMClient
    {
        Task<string> GenerateAsync(string prompt, ModelInfo model, int maxTokens, CancellationToken cancellationToken = default);
    }
}

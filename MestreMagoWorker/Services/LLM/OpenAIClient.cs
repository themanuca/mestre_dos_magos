using System;
using System.Threading;
using System.Threading.Tasks;
using MestreMagoWorker.Services.Models;

namespace MestreMagoWorker.Services.LLM
{
    // Implementar adaptador real aqui (OpenAI/Azure/etc).
    public class OpenAIClient : ILLMClient
    {
        public OpenAIClient()
        {
            // injete configurações/HTTP client via DI quando implementar
        }

        public Task<string> GenerateAsync(string prompt, ModelInfo model, int maxTokens, CancellationToken cancellationToken = default)
        {
            // Exemplo: chamar API externa e retornar texto.
            // Aqui apenas stub para compilar; substitua pela integração real.
            return Task.FromResult("[LLM RESPONSE STUB] " + (prompt.Length > 200 ? prompt[..200] : prompt));
        }
    }
}

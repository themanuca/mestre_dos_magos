using System;
using System.Threading;
using System.Threading.Tasks;
using MestreMagoWorker.Models;
using MestreMagoWorker.Services.LLM;
using MestreMagoWorker.Services.Models;
using MestreMagoWorker.Services.Prompts;
using MestreMagoWorker.Services.Session;

namespace MestreMagoWorker.Services.Orchestration
{
    public class ModelOrchestrator
    {
        private readonly ILLMClient _llm;
        private readonly PromptRepository _prompts;
        private readonly ModelSelector _selector;
        private readonly SessionManager _sessions;

        public ModelOrchestrator(ILLMClient llm, PromptRepository prompts, ModelSelector selector, SessionManager sessions)
        {
            _llm = llm;
            _prompts = prompts;
            _selector = selector;
            _sessions = sessions;
        }

        public async Task<string> GenerateSuggestionAsync(string sessionId, string userInput, ModelPreference pref, CancellationToken ct = default)
        {
            var session = _sessions.GetSession(sessionId) ?? throw new InvalidOperationException("Sessão não encontrada");
            // build prompt: prompt-base + contexto + histórico + user input
            var basePromptMeta = _prompts.Get("assistant.default.v1");
            var basePrompt = basePromptMeta?.Content ?? "Você é um assistente útil.";

            var assembled = new System.Text.StringBuilder();
            assembled.AppendLine(basePrompt);
            assembled.AppendLine();
            assembled.AppendLine(session.BuildPrompt(userInput));

            var model = _selector.Select(pref);
            var response = await _llm.GenerateAsync(assembled.ToString(), model, session.MaxTokens, ct);

            session.AppendMessage("assistant", response);
            return response;
        }
    }
}

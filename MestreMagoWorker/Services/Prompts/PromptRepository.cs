using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MestreMagoWorker.Services.Prompts
{
    public record PromptMeta(string Key, string Content, string Author, string Version, DateTime CreatedUtc);

    public class PromptRepository
    {
        private readonly ConcurrentDictionary<string, PromptMeta> _store = new();

        public void AddOrUpdate(PromptMeta meta)
        {
            _store[meta.Key] = meta;
        }

        public PromptMeta? Get(string key) =>
            _store.TryGetValue(key, out var m) ? m : null;

        public IEnumerable<PromptMeta> List() => _store.Values;

        // Seed example
        public void SeedDefault()
        {
            var key = "assistant.default.v1";
            var content = "Você é um assistente conciso e útil. Responda alinhado ao contexto fornecido.";
            AddOrUpdate(new PromptMeta(key, content, "system", "1.0.0", DateTime.UtcNow));
        }
    }
}

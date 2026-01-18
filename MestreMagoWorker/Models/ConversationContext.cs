using System;
using System.Collections.Generic;
using System.Text;

namespace MestreMagoWorker.Models
{
    public class ConversationContext
    {
        public string SessionId { get; init; } = Guid.NewGuid().ToString();
        public string? ManualContext { get; set; }
        public List<(string role, string text, DateTime at)> Messages { get; } = new();
        public string Tone { get; set; } = "neutral";
        public string Objective { get; set; } = "assist";
        public int MaxTokens { get; set; } = 512;
        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

        public void AppendMessage(string role, string text)
        {
            Messages.Add((role, text, DateTime.UtcNow));
            LastUpdated = DateTime.UtcNow;
        }

        public void ClearHistory()
        {
            Messages.Clear();
            LastUpdated = DateTime.UtcNow;
        }

        public string BuildPrompt(string userInput)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ManualContext))
            {
                sb.AppendLine($"Contexto do usuário: {ManualContext}");
                sb.AppendLine();
            }

            sb.AppendLine($"Tom: {Tone} | Objetivo: {Objective}");
            sb.AppendLine();

            if (Messages.Count > 0)
            {
                sb.AppendLine("Histórico:");
                foreach (var (role, text, at) in Messages)
                {
                    sb.AppendLine($"[{role}] {text}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("Entrada:");
            sb.AppendLine(userInput);

            return sb.ToString();
        }
    }
}

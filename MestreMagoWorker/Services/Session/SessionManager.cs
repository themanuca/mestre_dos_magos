using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MestreMagoWorker.Models;

namespace MestreMagoWorker.Services.Session
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<string, ConversationContext> _sessions = new();

        public ConversationContext CreateSession(string? manualContext = null)
        {
            var ctx = new ConversationContext { ManualContext = manualContext };
            _sessions[ctx.SessionId] = ctx;
            return ctx;
        }

        public ConversationContext? GetSession(string sessionId) =>
            _sessions.TryGetValue(sessionId, out var ctx) ? ctx : null;

        public IEnumerable<ConversationContext> ListSessions() => _sessions.Values;

        public bool RemoveSession(string sessionId) => _sessions.TryRemove(sessionId, out _);

        // Simple retention purge
        public Task PurgeInactiveAsync(TimeSpan inactivity, CancellationToken ct = default)
        {
            foreach (var kv in _sessions)
            {
                if (ct.IsCancellationRequested) break;
                if (DateTime.UtcNow - kv.Value.LastUpdated > inactivity)
                {
                    _sessions.TryRemove(kv.Key, out _);
                }
            }
            return Task.CompletedTask;
        }
    }
}

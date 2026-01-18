using System;
using System.Collections.Generic;
using System.Linq;

namespace MestreMagoWorker.Services.Models
{
    public enum ModelPreference { Cost, Speed, Quality }

    public record ModelInfo(string Id, decimal CostPerToken, int EstimatedLatencyMs, int QualityScore);

    public class ModelSelector
    {
        private readonly List<ModelInfo> _models = new()
        {
            new ModelInfo("fast-small", 0.0005m, 100, 60),
            new ModelInfo("balanced-medium", 0.0010m, 200, 80),
            new ModelInfo("high-quality", 0.0025m, 500, 95)
        };

        public ModelInfo Select(ModelPreference preference)
        {
            return preference switch
            {
                ModelPreference.Cost => _models.OrderBy(m => m.CostPerToken).First(),
                ModelPreference.Speed => _models.OrderBy(m => m.EstimatedLatencyMs).First(),
                ModelPreference.Quality => _models.OrderByDescending(m => m.QualityScore).First(),
                _ => _models[1]
            };
        }
    }
}

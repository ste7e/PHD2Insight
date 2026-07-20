using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Analysis;

public static class GuideCorrectionAnalysis {
    public static GuideCorrectionResult Calculate(
        GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var raPulses = session.Frames
            .Where(f => f.RaPulseMilliseconds.HasValue)
            .Select(f => f.RaPulseMilliseconds!.Value)
            .ToList();

        var decPulses = session.Frames
            .Where(f => f.DecPulseMilliseconds.HasValue)
            .Select(f => f.DecPulseMilliseconds!.Value)
            .ToList();

        return new GuideCorrectionResult {
            RaCorrectionCount = raPulses.Count,

            AverageRaPulseMilliseconds = raPulses.Count == 0 ? 0 : raPulses.Average(),
            MaximumRaPulseMilliseconds = raPulses.Count == 0 ? 0 : raPulses.Max(),
            TotalRaCorrectionTime = TimeSpan.FromMilliseconds(raPulses.Sum()),

            DecCorrectionCount = decPulses.Count,

            AverageDecPulseMilliseconds = decPulses.Count == 0 ? 0 : decPulses.Average(),
            MaximumDecPulseMilliseconds = decPulses.Count == 0 ? 0 : decPulses.Max(),
            TotalDecCorrectionTime = TimeSpan.FromMilliseconds(decPulses.Sum()),

        };
    }
}
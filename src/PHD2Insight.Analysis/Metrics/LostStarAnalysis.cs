using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class LostStarAnalysis {
    public static LostStarResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var lostStars = session.Frames
            .Where(f => f.ErrorCode != 0)
            .ToList();

        if (lostStars.Count == 0) {
            return new LostStarResult();
        }

        var messages = lostStars
            .GroupBy(f => f.ErrorDescription ?? "Unknown")
            .ToDictionary(
                g => g.Key,
                g => g.Count());

        return new LostStarResult {
            LostStarCount = lostStars.Count,

            LostStarPercentage =
                100.0 * lostStars.Count / session.Frames.Count,

            FirstOccurrence =
                lostStars.First().ElapsedTime,

            LastOccurrence =
                lostStars.Last().ElapsedTime,

            ErrorMessages = messages
        };
    }
}
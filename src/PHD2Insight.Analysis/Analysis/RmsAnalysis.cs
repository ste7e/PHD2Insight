using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;
using PHD2Insight.Analysis.Statistics;

namespace PHD2Insight.Analysis.Analysis;

public static class RmsAnalysis {
    public static RmsResult Calculate(
        GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        if (session.Frames.Count == 0) {
            return new RmsResult();
        }

        double raPixels =
            StatisticalFunctions.RootMeanSquare(
                session.Frames.Select(f => f.RaErrorPixels));

        double decPixels =
            StatisticalFunctions.RootMeanSquare(
                session.Frames.Select(f => f.DecErrorPixels));

        double raArcSeconds =
            StatisticalFunctions.RootMeanSquare(
                session.Frames.Select(f => f.RaErrorArcSeconds));

        double decArcSeconds =
            StatisticalFunctions.RootMeanSquare(
                session.Frames.Select(f => f.DecErrorArcSeconds));

        return new RmsResult {
            RaPixels = raPixels,
            DecPixels = decPixels,
            TotalPixels = Math.Sqrt(
                raPixels * raPixels +
                decPixels * decPixels),

            RaArcSeconds = raArcSeconds,
            DecArcSeconds = decArcSeconds,
            TotalArcSeconds = Math.Sqrt(
                raArcSeconds * raArcSeconds +
                decArcSeconds * decArcSeconds)
        };
    }
}
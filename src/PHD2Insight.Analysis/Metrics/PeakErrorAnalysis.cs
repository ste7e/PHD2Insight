using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class PeakErrorAnalysis {
    public static PeakErrorResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        if (frames.Count == 0) {
            return new PeakErrorResult();
        }

        return new PeakErrorResult {
            MaximumRaErrorPixels =
                frames.Max(f => Math.Abs(f.RaErrorPixels)),

            MaximumDecErrorPixels =
                frames.Max(f => Math.Abs(f.DecErrorPixels)),

            MaximumTotalErrorPixels =
                frames.Max(f =>
                    Math.Sqrt(
                        f.RaErrorPixels * f.RaErrorPixels +
                        f.DecErrorPixels * f.DecErrorPixels)),

            MaximumRaErrorArcSeconds =
                frames.Max(f => Math.Abs(f.RaErrorArcSeconds)),

            MaximumDecErrorArcSeconds =
                frames.Max(f => Math.Abs(f.DecErrorArcSeconds)),

            MaximumTotalErrorArcSeconds =
                frames.Max(f =>
                    Math.Sqrt(
                        f.RaErrorArcSeconds * f.RaErrorArcSeconds +
                        f.DecErrorArcSeconds * f.DecErrorArcSeconds))
        };
    }
}
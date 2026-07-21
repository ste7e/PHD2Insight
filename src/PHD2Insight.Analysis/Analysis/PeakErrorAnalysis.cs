using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Analysis;

public static class PeakErrorAnalysis {
    public static PeakErrorResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        if (session.Frames.Count == 0) {
            return new PeakErrorResult();
        }

        return new PeakErrorResult {
            MaximumRaErrorPixels =
                session.Frames.Max(f => Math.Abs(f.RaErrorPixels)),

            MaximumDecErrorPixels =
                session.Frames.Max(f => Math.Abs(f.DecErrorPixels)),

            MaximumTotalErrorPixels =
                session.Frames.Max(f =>
                    Math.Sqrt(
                        f.RaErrorPixels * f.RaErrorPixels +
                        f.DecErrorPixels * f.DecErrorPixels)),

            MaximumRaErrorArcSeconds =
                session.Frames.Max(f => Math.Abs(f.RaErrorArcSeconds)),

            MaximumDecErrorArcSeconds =
                session.Frames.Max(f => Math.Abs(f.DecErrorArcSeconds)),

            MaximumTotalErrorArcSeconds =
                session.Frames.Max(f =>
                    Math.Sqrt(
                        f.RaErrorArcSeconds * f.RaErrorArcSeconds +
                        f.DecErrorArcSeconds * f.DecErrorArcSeconds))
        };
    }
}
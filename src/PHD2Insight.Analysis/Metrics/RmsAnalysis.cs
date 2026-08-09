using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Statistics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class RmsAnalysis {
    public static RmsResult Calculate(
        GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        if (frames.Count == 0) {
            return new RmsResult();
        }

        double raPixels = StatisticalFunctions.GuideRms(frames.Select(f => f.RaErrorPixels));

        double decPixels = StatisticalFunctions.GuideRms(frames.Select(f => f.DecErrorPixels));

        double raArcSeconds = StatisticalFunctions.GuideRms(frames.Select(f =>f.RaErrorArcSeconds));

        double decArcSeconds = StatisticalFunctions.GuideRms(frames.Select(f =>f.DecErrorArcSeconds));

        double meanRaPixels = StatisticalFunctions.Mean( frames.Select(f => f.RaErrorPixels));

        double meanDecPixels = StatisticalFunctions.Mean( frames.Select(f => f.DecErrorPixels));  
        
        double meanRaArcSeconds = StatisticalFunctions.Mean( frames.Select(f => f.RaErrorArcSeconds)); 

        double meanDecArcSeconds = StatisticalFunctions.Mean(frames.Select(f => f.DecErrorArcSeconds));
        
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
                decArcSeconds * decArcSeconds),
            MeanRaPixels = meanRaPixels,
            MeanDecPixels = meanDecPixels,
            MeanRaArcSeconds = meanRaArcSeconds,
            MeanDecArcSeconds = meanDecArcSeconds,
        };
    }

}
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Statistics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Analysis;

public static class AnalysisRunner {
    public static AnalysisResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var result = new AnalysisResult {
            SessionStatistics = SessionStatistics.Calculate(session),
            Rms = RmsAnalysis.Calculate(session),
            GuideCorrections = GuideCorrectionAnalysis.Calculate(session),
            PeakErrors = PeakErrorAnalysis.Calculate(session),
            LostStars = LostStarAnalysis.Calculate(session),
        };

        return result;
    }
}
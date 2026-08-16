using PHD2Insight.Analysis;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Parser;
using PHD2Insight.Parser.Parsers;
using PHD2Insight.Analysis.Quality;

namespace PHD2Insight.UI.Services;

public sealed class PHD2LogAnalysisService {

    private readonly GuideLogParser parser;
    private readonly DiagnosticEngine diagnosticEngine;
    private readonly GuidingQualityClassifier qualityClassifier;

    public PHD2LogAnalysisService() {
        parser = new GuideLogParser();
        qualityClassifier = new GuidingQualityClassifier();

        diagnosticEngine = new DiagnosticEngine(
        [
            new RaOscillationDiagnosisRule(),
            new DecOscillationDiagnosisRule(),
            new PoorTransparencyDiagnosisRule(),
            new GuideReversalDiagnosisRule(),
            new LargeGuideCorrectionsDiagnosisRule(),
            new AggressiveGuidingDiagnosisRule(),
        ]);
    }

    public LogAnalysisResult Analyse(string filePath) {

        using var stream = File.OpenRead(filePath);

        var parseResult = parser.Parse(stream);

        if (!parseResult.Success || parseResult.Value is null) {
            var errors = string.Join(
                Environment.NewLine,
                parseResult.Errors);

            throw new InvalidOperationException(
                $"Unable to parse PHD2 log:{Environment.NewLine}{errors}");
        }

        var guideLog = parseResult.Value;

        var sessions = guideLog.Sessions
            .Select((session, index) => {

                var analysis =
                    MetricsCalculator.Calculate(session);

                var diagnoses =
                    diagnosticEngine
                        .Diagnose(analysis)
                        .OrderByDescending(d => d.Score)
                        .ThenByDescending(d => d.Confidence)
                        .ToList();

                var quality =
                    qualityClassifier.Classify(
                        analysis.Rms?.TotalArcSeconds);

                return new SessionAnalysisResult(
                    index,
                    session,
                    analysis,
                    diagnoses,
                    quality);
            })
            .ToList();

        return new LogAnalysisResult {
            FileName = Path.GetFileName(filePath),
            Sessions = sessions
        };
    }
}





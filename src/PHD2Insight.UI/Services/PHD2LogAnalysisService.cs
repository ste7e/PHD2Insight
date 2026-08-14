using PHD2Insight.Analysis;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Parser;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.UI.Services;

public sealed class PHD2LogAnalysisService {

    private readonly GuideLogParser parser;
    private readonly DiagnosticEngine diagnosticEngine;

    public PHD2LogAnalysisService() {
        parser = new GuideLogParser();

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
                var analysis = MetricsCalculator.Calculate(session);

                var diagnoses = diagnosticEngine
                    .Diagnose(analysis)
                    .OrderByDescending(d => d.Score)
                    .ThenByDescending(d => d.Confidence)
                    .ToList();

                return new SessionAnalysisResult(
                    index,
                    analysis,
                    diagnoses);
            })
            .ToList();

        return new LogAnalysisResult(
            Path.GetFileName(filePath),
            sessions);
    }
}


public sealed record LogAnalysisResult(
    string FileName,
    IReadOnlyList<SessionAnalysisResult> Sessions);


public sealed record SessionAnalysisResult(
    int SessionNumber,
    AnalysisResult Analysis,
    IReadOnlyList<Diagnosis> Diagnoses);
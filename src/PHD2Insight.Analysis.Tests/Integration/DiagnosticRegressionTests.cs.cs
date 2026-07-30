using PHD2Insight.Analysis;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Abstractions;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Analysis.Tests.Integration;

public sealed class DiagnosticRegressionTests {
    private static readonly string SampleFolder =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "..",
                "samples"));

    [Fact]
    public void DumpValues() {
        DumpValuesFromSample("PHD2_GuideLog_001.txt");
        DumpValuesFromSample("PHD2_GuideLog_002.txt");
    }
    private void DumpValuesFromSample(string sampleName) {
        // Arrange
        var parser = new GuideLogParser();

        var diagnosticEngine = new DiagnosticEngine(
        [
            new RaOscillationDiagnosisRule()
        ]);

        var path = Path.Combine(
            SampleFolder,
            sampleName);

        Console.WriteLine("---------------------------------");
        Console.WriteLine("---------------------------------");

        Console.WriteLine($"Dumping values for {sampleName}\n\n");

        using var stream = File.OpenRead(path);

        // Act
        var guideLog = AssertParsed(parser.Parse(stream));

        ICollection<AnalysisResult> analyses = new List<AnalysisResult>();

        reportSessions(diagnosticEngine, guideLog, analyses);

        for (int i=0;i< guideLog.Sessions.Count; i++) {
            reportSessionMetrics(diagnosticEngine, guideLog, i, analyses.ElementAt(i));
        }
    }
    private static void reportSessionMetrics(DiagnosticEngine diagnosticEngine, GuideLog guideLog, int sessionIndex, AnalysisResult analysis) {
        var oscillation = analysis.OscillationMetrics;

        Console.WriteLine("---------------------------------");
        Console.WriteLine($"Session {sessionIndex} metrics:");

        Console.WriteLine($"RA Oscillation Events/min : {oscillation.RaOscillationEventsPerMinute}");
        Console.WriteLine($"DEC Oscillation Events/min : {oscillation.DecOscillationEventsPerMinute}");
        Console.WriteLine($"Mean RA amplitude : {oscillation.MeanRaOscillationAmplitudeArcSeconds:F2}");
        Console.WriteLine($"Mean DEC amplitude: {oscillation.MeanDecOscillationAmplitudeArcSeconds:F2}");

        Console.WriteLine($"Frames               : {analysis.SessionStatistics.FrameCount:F0}");
        Console.WriteLine($"RA RMS (pixels)      : {analysis.Rms.RaPixels:F2}");
        Console.WriteLine($"DEC RMS (pixels)     : {analysis.Rms.DecPixels:F2}");
        Console.WriteLine($"RA RMS (arcsec)      : {analysis.Rms.RaArcSeconds:F2}");
        Console.WriteLine($"DEC RMS (arcsec)     : {analysis.Rms.DecArcSeconds:F2}");
        Console.WriteLine($"RA/DEC Ratio         : {analysis.Rms.RaToDecRatio:F2}");
        Console.WriteLine($"RA Direction Changes : {analysis.OscillationMetrics.RaDirectionReversals}");
        Console.WriteLine($"Average RA Pulse (ms): {analysis.GuideCorrections.AverageRaPulseMilliseconds:F0}");

        Console.WriteLine("=========================================");

        var diagnoses = diagnosticEngine.Diagnose(analysis);
        if (diagnoses.Count > 0) {
            var diagnosis = Assert.Single(diagnoses);

            Console.WriteLine($"Diagnosis: {diagnosis.Code} ({diagnosis.Confidence})");

            foreach (var e in diagnosis.Evidence) {
                Console.WriteLine($"{e.Code,-35} +{e.Weight}");
            }
        }

    }

    private static void reportSessions(DiagnosticEngine diagnosticEngine, GuideLog guideLog, ICollection<AnalysisResult> analyses) {
        for (var i = 0; i < guideLog.Sessions.Count; i++) {
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Calculating metrics for Session {i}:");
            analyses.Add(MetricsCalculator.Calculate(guideLog.Sessions[i]));

            Console.WriteLine($"Diagnosing metrics for Session {i}");
            var diagnoses = diagnosticEngine
                .Diagnose(analyses.ElementAt(i))
                .ToList();

            Console.WriteLine(
                $"Session {i}: {string.Join(", ", diagnoses.Select(d => $"{d.Code} ({d.Confidence})"))}");
        }
    }

    [Fact]
    public void RaOscillation_Log_Is_Diagnosed_As_Medium_Confidence() {
        // Arrange
        var parser = new GuideLogParser();

        var diagnosticEngine = new DiagnosticEngine(
        [
            new RaOscillationDiagnosisRule()
        ]);

        var path = Path.Combine(
            SampleFolder,
            "PHD2_GuideLog_001.txt");

        using var stream = File.OpenRead(path);

        // Act
        var guideLog = AssertParsed(parser.Parse(stream));

        Assert.Equal(4, guideLog.Sessions.Count);

        var analysis = MetricsCalculator.Calculate(guideLog.Sessions[2]);
        var diagnoses = diagnosticEngine.Diagnose(analysis);

        var diagnosis = Assert.Single(diagnoses);
        Assert.Equal(
            DiagnosisCodes.RaOscillation,
            diagnosis.Code);

        Assert.Equal(
            DiagnosisConfidence.Medium,
            diagnosis.Confidence);

    }

    [Fact]
    public void Check_RMS_values() {
        // Arrange
        var parser = new GuideLogParser();

        var diagnosticEngine = new DiagnosticEngine(
        [
            new RaOscillationDiagnosisRule()
        ]);

        var path = Path.Combine(
            SampleFolder,
            "PHD2_GuideLog_002.txt");

        using var stream = File.OpenRead(path);

        // Act
        var guideLog = AssertParsed(parser.Parse(stream));

        Assert.Equal(3, guideLog.Sessions.Count);

        var result = MetricsCalculator.Calculate(guideLog.Sessions[0]);

        Assert.Equal(0.84, result.Rms.RaArcSeconds, 2);
        Assert.Equal(0.43, result.Rms.DecArcSeconds, 2);
        Assert.Equal(0.95, result.Rms.TotalArcSeconds, 2);

        Assert.Equal(-0.03, result.Rms.MeanRaArcSeconds, 2);
        Assert.Equal(0.25, result.Rms.MeanDecArcSeconds, 2);
    }


    private static GuideLog AssertParsed(ParseResult<GuideLog> result) {
        Assert.True(
            result.Success,
            $"Parsing failed:{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors)}");

        Assert.NotNull(result.Value);

        return result.Value!;
    }
}
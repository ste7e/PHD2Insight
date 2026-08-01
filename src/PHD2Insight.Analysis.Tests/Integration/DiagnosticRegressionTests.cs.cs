using PHD2Insight.Analysis;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Abstractions;
using PHD2Insight.Parser.Parsers;
using System.Security.Policy;
using System.Text;

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

    public static IEnumerable<string> GetFilenames(string folderPath, string pattern = "*", bool recursive = false) { 
        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return Directory.EnumerateFiles(folderPath, pattern, option); 
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

    [Fact]
    public void DumpDiagnoses() {
        var csv = new StringBuilder();
        var sum = new Summary {
            CountByScore = new Dictionary<int, int>(),
            CountByConfidence = new Dictionary<DiagnosisConfidence, int>(),
            CountByDiagnosisCode = new Dictionary<string, int>(),
            CountByEvidenceCombination = new Dictionary<string, int>()

        };

        csv.AppendLine(
            "File,Session,Confidence,EvidenceScore,EvidenceCount,RA RMS,DEC RMS,RA/DEC Ratio,RA Osc/min,DEC Osc/min,RA Osc Amp,DEC Osc Amp,Mean RA Pulse,Mean DEC Pulse,Diagnosis,Evidence");


        foreach (var sample in GetFilenames(SampleFolder+"\\Good guiding", pattern: "PHD2_GuideLog*.txt", recursive: true)) {
            DumpDiagnosesFromSample(Path.GetFullPath(sample), csv, sum);
        }

        File.WriteAllText(Path.Combine(SampleFolder, "RegressionResults.csv"), csv.ToString());

        Console.WriteLine("Summary of diagnoses:");

        Console.WriteLine("Confidence, Count");
        foreach (var kvp in sum.CountByConfidence) {
            Console.WriteLine($"{kvp.Key}, {kvp.Value}");
        }
        Console.WriteLine("Evidence combination, Count");
        foreach (var kvp in sum.CountByEvidenceCombination) {
            Console.WriteLine($"{kvp.Key}, {kvp.Value}");
        }
        Console.WriteLine("Diagnosis code, Count");
        foreach (var kvp in sum.CountByDiagnosisCode) {
            Console.WriteLine($"{kvp.Key}, {kvp.Value}");
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

    private static GuideLog AssertParsed(ParseResult<GuideLog> result) {
        Assert.True(
            result.Success,
            $"Parsing failed:{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors)}");

        Assert.NotNull(result.Value);

        return result.Value!;
    }

    private static void reportDiagnoses(DiagnosticEngine diagnosticEngine, GuideLog guideLog, int sessionIndex, AnalysisResult analysis) {
        var diagnoses = diagnosticEngine.Diagnose(analysis);
        Console.WriteLine("---------------------------------");
        Console.WriteLine($"Session {sessionIndex} diagnoses:");
        if (diagnoses.Count > 0) {
            foreach (var diagnosis in diagnoses) {
                Console.WriteLine($"Diagnosis: {diagnosis.Code} ({diagnosis.Confidence}) {diagnosis.Description}");
                foreach (var e in diagnosis.Evidence) {
                    Console.WriteLine($"{e.Code,-35} +{e.Weight} {e.Explanation}");
                }
            }
        } else {
            Console.WriteLine("No diagnoses found.");
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

    private class Summary {
        public DiagnosisConfidence Confidence { get; set; }
        public required IDictionary<int, int> CountByScore { get; set; }
        public required IDictionary<DiagnosisConfidence, int> CountByConfidence { get; set; }
        public required IDictionary<string, int> CountByDiagnosisCode { get; set; } = new Dictionary<string, int>();
        public required IDictionary<string, int> CountByEvidenceCombination { get; set; } = new Dictionary<string, int>();
    }

    private void DumpDiagnosesFromSample(string samplePath, StringBuilder? csv = null, Summary? sum = null) {
        // Arrange
        var parser = new GuideLogParser();

        var diagnosticEngine = new DiagnosticEngine(
        [
            new RaOscillationDiagnosisRule()
        ]);

        string sampleName;
        string fullSamplePath = Path.Combine(SampleFolder, samplePath);
        if (samplePath.Contains("\\")) {
            fullSamplePath = samplePath;
            sampleName = Path.GetFileName(fullSamplePath);
        } else {
            fullSamplePath = Path.Combine(SampleFolder, samplePath);
            sampleName = samplePath;
        }
        if (!File.Exists(samplePath)) {
            throw new FileNotFoundException($"Sample file not found: {samplePath}");
        }

        Console.WriteLine("---------------------------------");
        Console.WriteLine("---------------------------------");

        Console.WriteLine($"Dumping diagnoses for {sampleName}\n\n");

        using var stream = File.OpenRead(fullSamplePath);

        // Act
        var guideLog = AssertParsed(parser.Parse(stream));

        for (int i = 0; i < guideLog.Sessions.Count; i++) {
            var analysis = MetricsCalculator.Calculate(guideLog.Sessions[i]);

            reportSessionMetrics(diagnosticEngine, guideLog, i, analysis);

            var diagnosis = diagnosticEngine.Diagnose(analysis)
                .OrderByDescending(d => d.Confidence)
                .FirstOrDefault();

            var evidenceCombination = diagnosis is null
                        ? ""
                        : string.Join("|",
                            diagnosis.Evidence.Select(e => e.Code));
            csv?.AppendLine(
                string.Join(",",
                    Path.GetFileName(sampleName),
                    i,
                    diagnosis?.Confidence.ToString() ?? "",
                    diagnosis?.Score.ToString() ?? "",
                    diagnosis?.Evidence.Count.ToString() ?? "",
                    analysis.Rms.RaArcSeconds.ToString("F2"),
                    analysis.Rms.DecArcSeconds.ToString("F2"),
                    analysis.Rms.RaToDecRatio.ToString("F2"),
                    analysis.OscillationMetrics.RaOscillationEventsPerMinute.ToString("F2"),
                    analysis.OscillationMetrics.DecOscillationEventsPerMinute.ToString("F2"),
                    analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds.ToString("F2"),
                    analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds.ToString("F2"),
                    analysis.GuideCorrections.AverageRaPulseMilliseconds.ToString("F0"),
                    analysis.GuideCorrections.AverageDecPulseMilliseconds.ToString("F0"),
                    diagnosis?.Title ?? "",
                    evidenceCombination));

            if (sum!=null) {
                if (diagnosis != null) {
                    sum.Confidence = diagnosis.Confidence;
                    if (!sum.CountByConfidence.ContainsKey(diagnosis.Confidence)) {
                        sum.CountByConfidence[diagnosis.Confidence] = 0;
                    }
                    sum.CountByConfidence[diagnosis.Confidence]++;

                    if (!sum.CountByScore.ContainsKey(diagnosis.Score)) {
                        sum.CountByScore[diagnosis.Score] = 0;
                    }
                    sum.CountByScore[diagnosis.Score]++;

                    if (!sum.CountByEvidenceCombination.ContainsKey(evidenceCombination)) {
                        sum.CountByEvidenceCombination[evidenceCombination] = 0;
                    }
                    sum.CountByEvidenceCombination[evidenceCombination]++;

                    if (!sum.CountByDiagnosisCode.ContainsKey(diagnosis.Code)) {
                        sum.CountByDiagnosisCode[diagnosis.Code] = 0;
                    }
                    sum.CountByDiagnosisCode[diagnosis.Code]++;
                }
            }
        }

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

        for (int i = 0; i < guideLog.Sessions.Count; i++) {
            reportSessionMetrics(diagnosticEngine, guideLog, i, analyses.ElementAt(i));
        }
    }
}
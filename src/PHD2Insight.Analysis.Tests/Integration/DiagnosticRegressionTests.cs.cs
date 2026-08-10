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

    private static double Percentile(
    IReadOnlyList<double> values,
    double percentile) {
        if (values.Count == 0)
            return double.NaN;

        var sorted = values.OrderBy(v => v).ToArray();

        if (sorted.Length == 1)
            return sorted[0];

        var position = percentile * (sorted.Length - 1);
        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);

        if (lower == upper)
            return sorted[lower];

        var fraction = position - lower;

        return sorted[lower] +
               (sorted[upper] - sorted[lower]) * fraction;
    }
    private static double Percentile(
    IEnumerable<double> values,
    double percentile) {
        var sorted = values
            .Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .OrderBy(v => v)
            .ToArray();

        if (sorted.Length == 0)
            return double.NaN;

        if (sorted.Length == 1)
            return sorted[0];

        var position = percentile * (sorted.Length - 1);

        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);

        if (lower == upper)
            return sorted[lower];

        var fraction = position - lower;

        return sorted[lower] +
               (sorted[upper] - sorted[lower]) * fraction;
    }
    private static void ReportDistribution(
    string name,
    IEnumerable<double> values) {
        var data = values
            .Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .ToArray();

        if (data.Length == 0)
            return;

        Console.WriteLine();
        Console.WriteLine(name);
        Console.WriteLine($"  Count : {data.Length}");
        Console.WriteLine($"  Min   : {Percentile(data, 0.00):F2}");
        Console.WriteLine($"  P25   : {Percentile(data, 0.25):F2}");
        Console.WriteLine($"  Median: {Percentile(data, 0.50):F2}");
        Console.WriteLine($"  P75   : {Percentile(data, 0.75):F2}");
        Console.WriteLine($"  P90   : {Percentile(data, 0.90):F2}");
        Console.WriteLine($"  P95   : {Percentile(data, 0.95):F2}");
        Console.WriteLine($"  P99   : {Percentile(data, 0.99):F2}");
        Console.WriteLine($"  Max   : {Percentile(data, 1.00):F2}");
    }
    private static void ReportCorpusMetricDistributions(Summary summary) {
        Console.WriteLine();
        Console.WriteLine("=========================================");
        Console.WriteLine("Corpus metric distributions");
        Console.WriteLine("=========================================");

        ReportDistribution(
            "RA Oscillation Events/min",
            summary.RaOscillationEventsPerMinute.Values);

        ReportDistribution(
            "DEC Oscillation Events/min",
            summary.DecOscillationEventsPerMinute.Values);

        ReportDistribution(
            "Mean RA amplitude (arcsec)",
            summary.MeanRaOscillationAmplitudeArcSeconds.Values);


        ReportDistribution(
            "Mean DEC amplitude (arcsec)",
            summary.MeanDecOscillationAmplitudeArcSeconds.Values);

        ReportDistribution(
            "RA RMS (arcsec)",
            summary.RaRmsArcSeconds.Values);

        ReportDistribution(
            "DEC RMS (arcsec)",
            summary.DecRmsArcSeconds.Values);

        ReportDistribution(
            "RA/DEC Ratio",
            summary.RaToDecRatio.Values);

        ReportDistribution(
            "RA Direction Changes",
            summary.RaDirectionReversals.Values);

        ReportDistribution(
            "DEC Direction Changes",
            summary.DecDirectionReversals.Values);

        ReportDistribution(
            "Average RA Pulse (ms)",
            summary.AverageRaPulseMilliseconds.Values);

        ReportDistribution(
            "Average DEC Pulse (ms)",
            summary.AverageDecPulseMilliseconds.Values);

        ReportDistribution(
            "Lost Stars (%)",
            summary.LostStarPercentage.Values);

        ReportDistribution(
            "RA Reversal Rate (per minute)",
            summary.RaReversalRatePerMinute.Values);

        ReportDistribution(
            "DEC Reversal Rate (per minute)",
            summary.DecReversalRatePerMinute.Values);

        Console.WriteLine("=========================================");
    }
    [Fact]
    public void RunRegressionTestOnAllLogsInFolder() {
        var csv = new StringBuilder();
        var sum = new Summary();

        csv.AppendLine(
            "File,Session,RA RMS,DEC RMS,RA/DEC Ratio,RA Osc/min,DEC Osc/min,RA Osc Amp,DEC Osc Amp,Mean RA Pulse,Mean DEC Pulse,Diagnosis,Supporting Observations");

        foreach (var sample in GetFilenames(SampleFolder /*+ "\\Bad guiding"*/, pattern: "PHD2_GuideLog*.txt", recursive: true)) {
            RunRegressionTestOnSample(Path.GetFullPath(sample), csv, sum);
        }

        File.WriteAllText(Path.Combine(SampleFolder, "RegressionResults.csv"), csv.ToString());

        Console.WriteLine("Summary of diagnoses:");
        Console.WriteLine("=================");

        ReportCorpusMetricDistributions(sum);

/*        Console.WriteLine("Session, RA osc events/min");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.RaOscillationEventsPerMinute) {
            Console.WriteLine($"{kvp.Key}, {kvp.Value}");
        }
*/
        Console.WriteLine("=================");
        Console.WriteLine("Confidence, Count");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.CountByConfidence) {
            Console.WriteLine($"{kvp.Key,-20} {kvp.Value}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Score, Count");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.CountByScore) {
            Console.WriteLine($"{kvp.Key,-20} {kvp.Value}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Observation, Count");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.CountByObservation) {
            Console.WriteLine($"{kvp.Key,-35} {kvp.Value}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Diagnosis code, Count");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.CountByDiagnosisCode) {
            Console.WriteLine($"{kvp.Key,-35} {kvp.Value}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Diagnosis combination, Count");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.CountCoOccurrence) {
            Console.WriteLine($"{kvp.Key,-80} {kvp.Value}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Diagnosis co-occurrence matrix");
        Console.WriteLine("-----------------");
        foreach (var kvp in sum.CoOccurrenceMatrix) {
            Console.WriteLine($"{kvp.Key.Item1,-20} + {kvp.Key.Item2,-20} {kvp.Value}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Observation frequency by diagnosis");
        Console.WriteLine("-----------------");
        Console.WriteLine($"{"Observation",-35} {"Count",-10} {"Diagnosis"}");
        foreach (var kvp in sum.ObservationFreqByDiagnosis.OrderByDescending(r => r.Value)) {
            Console.WriteLine($"{kvp.Key.Item2,-35} {kvp.Value,-10} {kvp.Key.Item1}");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Observation correlations");
        Console.WriteLine("-----------------");
        string lastKey1 = "";
        foreach (var kvp in sum.ObservationCorrelations.OrderByDescending(r => r.Value * 100 / sum.CountByObservation[r.Key.Item1]).OrderBy(r => r.Key.Item1).Where(r => r.Value * 100 / sum.CountByObservation[r.Key.Item1] > 15)) {
            if (kvp.Key.Item1 != lastKey1) {
                Console.WriteLine("--");
                Console.WriteLine($"{kvp.Key.Item1} ({sum.CountByObservation[kvp.Key.Item1]})");
                lastKey1 = kvp.Key.Item1;
            }
            Console.WriteLine($"   → {kvp.Key.Item2,-35} {kvp.Value * 100 / sum.CountByObservation[kvp.Key.Item1]}% ({kvp.Value})");
        }
        Console.WriteLine("=================");
        Console.WriteLine("Guide reversal analysis");
        Console.WriteLine("-----------------");
        Console.WriteLine($"{"Sessions with RA reversals:",-50} {sum.ReversalStats.RaReversalCount}");
        Console.WriteLine($"{"Sessions with DEC reversals:",-50} {sum.ReversalStats.DecReversalCount}");
        Console.WriteLine($"{"Sessions with both RA and DEC reversals:",-50} {sum.ReversalStats.RaAndDecReversal}");
        Console.WriteLine($"{"Sessions with only RA reversals:",-50} {sum.ReversalStats.RaOnlyReversal}");
        Console.WriteLine($"{"Sessions with only DEC reversals:",-50} {sum.ReversalStats.DecOnlyReversal}");
        Console.WriteLine($"{"Sessions with no reversals:",-50} {sum.ReversalStats.NoReversal}");

    }
    [Fact]
    public void RaOscillation_Log_Is_Diagnosed_As_Low_Confidence() {
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
            DiagnosisConfidence.Low,
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
                foreach (var obs in diagnosis.SupportingObservations) {
                    Console.WriteLine($"{obs.Code,-40} +{obs.Weight} {obs.Explanation}");
                }
            }
        } else {
            Console.WriteLine("No diagnoses found.");
        }
    }

    private static void reportSessionMetrics(
        DiagnosticEngine diagnosticEngine,
        GuideLog guideLog,
        int sessionIndex,
        AnalysisResult analysis) {
        var oscillation = analysis.OscillationMetrics;


        Console.WriteLine("---------------------------------");
        Console.WriteLine($"Session {sessionIndex} metrics:");

        Console.WriteLine($"RA Oscillation Events/min : {oscillation.RaOscillationEventsPerMinute}");
        Console.WriteLine($"DEC Oscillation Events/min : {oscillation.DecOscillationEventsPerMinute}");
        Console.WriteLine($"Mean RA amplitude : {oscillation.MeanRaOscillationAmplitudeArcSeconds:F2}");
        Console.WriteLine($"Mean DEC amplitude: {oscillation.MeanDecOscillationAmplitudeArcSeconds:F2}");

        Console.WriteLine($"Frames               : {analysis.SessionStatistics?.FrameCount:F0}");
        Console.WriteLine($"RA RMS (pixels)      : {analysis.Rms?.RaPixels:F2}");
        Console.WriteLine($"DEC RMS (pixels)     : {analysis.Rms?.DecPixels:F2}");
        Console.WriteLine($"RA RMS (arcsec)      : {analysis.Rms?.RaArcSeconds:F2}");
        Console.WriteLine($"DEC RMS (arcsec)     : {analysis.Rms?.DecArcSeconds:F2}");
        Console.WriteLine($"RA/DEC Ratio         : {analysis.Rms?.RaToDecRatio:F2}");
        Console.WriteLine($"RA Direction Changes : {analysis.OscillationMetrics?.RaDirectionReversals}");
        Console.WriteLine($"Average RA Pulse (ms): {analysis.GuideCorrections?.AverageRaPulseMilliseconds:F0}");

        Console.WriteLine($"DEC Direction Changes: {analysis.OscillationMetrics?.DecDirectionReversals}");
        Console.WriteLine($"Average DEC Pulse(ms): {analysis.GuideCorrections?.AverageDecPulseMilliseconds:F0}");
        Console.WriteLine($"Lost Stars           : {analysis.LostStars?.LostStarCount}");
        Console.WriteLine($"Lost Stars (%)       : {analysis.LostStars?.LostStarPercentage:F2}%");
        Console.WriteLine($"Settling Events      : {analysis.Settling?.SettlingAttemptCount}");
        Console.WriteLine($"Longest settling Time (s)    : {analysis.Settling?.LongestSettlingTime.TotalSeconds:F2}");

        Console.WriteLine("=========================================");

        var diagnosisCombinations = new Dictionary<string, int>();

        var diagnoses = diagnosticEngine.Diagnose(analysis)
            .OrderByDescending(d => d.Score)
            .ThenByDescending(d => d.Confidence)
            .ToList();

        var combination = diagnoses.Count == 0
    ? "(None)"
    : string.Join(
        " + ",
        diagnoses
            .Select(d => d.Code + "(" + d.Score + ")")
            .OrderBy(c => c));

        diagnosisCombinations.TryGetValue(combination, out var count);
        diagnosisCombinations[combination] = count + 1;


        if (diagnoses.Count > 0) {
            foreach (var diagnosis in diagnoses) {
                Console.WriteLine($"Diagnosis: {diagnosis.Code} ({diagnosis.Confidence})");

                foreach (var obs in diagnosis.SupportingObservations) {
                    Console.WriteLine($"{obs.Code,-40} +{obs.Weight}");
                }
            }
        }

        Console.WriteLine("---------------------------------");
        Console.WriteLine("Diagnosis combinations");
        Console.WriteLine("---------------------------------");

        foreach (var pair in diagnosisCombinations
            .OrderByDescending(p => p.Value)
            .ThenBy(p => p.Key)) {
            Console.WriteLine($"{pair.Key,-60} {pair.Value,5}");
        }
        Console.WriteLine("---------------------------------");

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
        public IDictionary<int, int> CountByScore { get; set; } = new Dictionary<int, int>();
        public IDictionary<DiagnosisConfidence, int> CountByConfidence { get; set; } = new Dictionary<DiagnosisConfidence, int>();
        public IDictionary<string, int> CountByDiagnosisCode { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> CountByObservation { get; set; } = new Dictionary<string, int>();

        public IDictionary<string, double> RaOscillationEventsPerMinute { get; set; } = new Dictionary<string, double>();

        public IDictionary<string, int> CountCoOccurrence { get; set; } = new Dictionary<string, int>();
        public IDictionary<(string, string), int> CoOccurrenceMatrix { get; set; } = new Dictionary<(string, string), int>();

        public IDictionary<(string, string), int> ObservationFreqByDiagnosis { get; set; } = new Dictionary<(string, string), int>();

        public IDictionary<(string, string), int> ObservationCorrelations { get; set; } = new Dictionary<(string, string), int>();

        public class ReversalSummary {
            public int RaReversalCount { get; set; }
            public int DecReversalCount { get; set; }
            public int RaAndDecReversal { get; set; }
            public int RaOnlyReversal { get; set; }
            public int DecOnlyReversal { get; set; }
            public int NoReversal { get; set; }
        }

        public ReversalSummary ReversalStats { get; set; } = new ReversalSummary();

        public IDictionary<string, double> DecOscillationEventsPerMinute { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> DecReversalRatePerMinute { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> RaReversalRatePerMinute { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> MeanRaOscillationAmplitudeArcSeconds { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> MeanDecOscillationAmplitudeArcSeconds { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> RaRmsArcSeconds { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> DecRmsArcSeconds { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> RaToDecRatio { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> RaDirectionReversals { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> DecDirectionReversals { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> AverageRaPulseMilliseconds { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> AverageDecPulseMilliseconds { get; set; }
            = new Dictionary<string, double>();

        public IDictionary<string, double> LostStarPercentage { get; set; }
            = new Dictionary<string, double>();
    }

    private void RunRegressionTestOnSample(string samplePath, StringBuilder? csv = null, Summary? sum = null) {
        // Arrange
        var parser = new GuideLogParser();

        var diagnosticEngine = new DiagnosticEngine(
        [
            new RaOscillationDiagnosisRule(),
            new DecOscillationDiagnosisRule(),
            new PoorTransparencyDiagnosisRule(),
            new GuideReversalDiagnosisRule(),
            new LargeGuideCorrectionsDiagnosisRule(),
            new AggressiveGuidingDiagnosisRule(),
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

            var reversals = analysis.GuideReversals;

            if (sum is not null) {
                var raReversal = reversals.RaReversalRatePerMinute > 1;
                var decReversal = reversals.DecReversalRatePerMinute > 1;
                if (raReversal) {
                    sum.ReversalStats.RaReversalCount++;
                }
                if (decReversal) {
                    sum.ReversalStats.DecReversalCount++;
                }
                if (raReversal && decReversal) {
                    sum.ReversalStats.RaAndDecReversal++;
                } else if (raReversal) {
                    sum.ReversalStats.RaOnlyReversal++;
                } else if (decReversal) {
                    sum.ReversalStats.DecOnlyReversal++;
                } else {
                    sum.ReversalStats.NoReversal++;
                }

                var key = $"{sampleName}_{i}";

                sum.RaOscillationEventsPerMinute[key] =
                    analysis.OscillationMetrics.RaOscillationEventsPerMinute;

                sum.DecOscillationEventsPerMinute[key] =
                    analysis.OscillationMetrics.DecOscillationEventsPerMinute;

                sum.DecReversalRatePerMinute[key] =
                    analysis.GuideReversals.DecReversalRatePerMinute;

                sum.RaReversalRatePerMinute[key] =
                    analysis.GuideReversals.RaReversalRatePerMinute;

                sum.MeanRaOscillationAmplitudeArcSeconds[key] =
                    analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds;

                sum.MeanDecOscillationAmplitudeArcSeconds[key] =
                    analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds;

                sum.RaRmsArcSeconds[key] =
                    analysis.Rms?.RaArcSeconds ?? 0;

                sum.DecRmsArcSeconds[key] =
                    analysis.Rms?.DecArcSeconds ?? 0;

                sum.RaToDecRatio[key] =
                    analysis.Rms?.RaToDecRatio ?? 0;

                sum.RaDirectionReversals[key] =
                    analysis.OscillationMetrics.RaDirectionReversals;

                sum.DecDirectionReversals[key] =
                    analysis.OscillationMetrics.DecDirectionReversals;

                sum.AverageRaPulseMilliseconds[key] =
                    analysis.GuideCorrections?.AverageRaPulseMilliseconds ?? 0;

                sum.AverageDecPulseMilliseconds[key] =
                    analysis.GuideCorrections?.AverageDecPulseMilliseconds ?? 0;

                sum.LostStarPercentage[key] =
                    analysis.LostStars?.LostStarPercentage ?? 0;

            }

            Console.WriteLine(
                $"RA reversals: {reversals.RaReversalCount}, " +
                $"DEC reversals: {reversals.DecReversalCount}, " +
                $"RA rate: {reversals.RaReversalRatePerMinute:F2}/min, " +
                $"DEC rate: {reversals.DecReversalRatePerMinute:F2}/min");

            reportSessionMetrics(diagnosticEngine, guideLog, i, analysis);

            var diagnoses = diagnosticEngine.Diagnose(analysis)
                .OrderByDescending(d => d.Score)
                .ThenByDescending(d => d.Confidence)
                .ToList();

            var diagnosisSummary = string.Join(
                ";",
                diagnoses.Select(d => $"{d.Title}({d.Score} {d.Confidence})"));

            var evidenceSummary = string.Join(
                ";",
                diagnoses.Select(d =>
                    $"{d.Code}:" +
                    string.Join("|", d.SupportingObservations.Select(o => o.Code))));

            csv?.AppendLine(
                string.Join(",",
                    Path.GetFileName(sampleName),
                    i,
                    analysis.Rms.RaArcSeconds.ToString("F2"),
                    analysis.Rms.DecArcSeconds.ToString("F2"),
                    analysis.Rms.RaToDecRatio.ToString("F2"),
                    analysis.OscillationMetrics.RaOscillationEventsPerMinute.ToString("F2"),
                    analysis.OscillationMetrics.DecOscillationEventsPerMinute.ToString("F2"),
                    analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds.ToString("F2"),
                    analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds.ToString("F2"),
                    analysis.GuideCorrections.AverageRaPulseMilliseconds.ToString("F0"),
                    analysis.GuideCorrections.AverageDecPulseMilliseconds.ToString("F0"),
                    diagnosisSummary,
                    evidenceSummary));

            if (sum != null) {


                foreach (var diagnosis in diagnoses) {

                    if (!sum.CountByConfidence.ContainsKey(diagnosis.Confidence))
                        sum.CountByConfidence[diagnosis.Confidence] = 0;

                    sum.CountByConfidence[diagnosis.Confidence]++;

                    if (!sum.CountByScore.ContainsKey(diagnosis.Score))
                        sum.CountByScore[diagnosis.Score] = 0;

                    sum.CountByScore[diagnosis.Score]++;

                    foreach (var observation in diagnosis.SupportingObservations) {
                        if (!sum.CountByObservation.ContainsKey(observation.Code))
                            sum.CountByObservation[observation.Code] = 0;

                        sum.CountByObservation[observation.Code]++;
                    }

                    if (!sum.CountByDiagnosisCode.ContainsKey(diagnosis.Code))
                        sum.CountByDiagnosisCode[diagnosis.Code] = 0;

                    sum.CountByDiagnosisCode[diagnosis.Code]++;

                    if (diagnosis.SupportingObservations.Count > 0) {
                        foreach (var obs in diagnosis.SupportingObservations) {
                            var key2 = (diagnosis.Code, obs.Code);
                            if (!sum.ObservationFreqByDiagnosis.ContainsKey(key2))
                                sum.ObservationFreqByDiagnosis[key2] = 0;
                            sum.ObservationFreqByDiagnosis[key2]++;
                        }
                    }

                    if (diagnosis.SupportingObservations.Count > 1) {
                        var obsCodes = diagnosis.SupportingObservations.Select(o => o.Code).OrderBy(c => c).ToList();
                        for (int j = 0; j < obsCodes.Count; j++) {
                            for (int k = j + 1; k < obsCodes.Count; k++) {
                                var pair = (obsCodes[j], obsCodes[k]);
                                if (!sum.ObservationCorrelations.ContainsKey(pair))
                                    sum.ObservationCorrelations[pair] = 0;
                                sum.ObservationCorrelations[pair]++;
                            }
                        }
                    }
                }
                if (diagnoses.Count > 1) {
                    var combination = string.Join(
                        " + ",
                        diagnoses
                            .Select(d => d.Code)
                            .OrderBy(c => c));
                    if (!sum.CountCoOccurrence.ContainsKey(combination))
                        sum.CountCoOccurrence[combination] = 0;
                    sum.CountCoOccurrence[combination]++;
                }

                if (diagnoses.Count > 1) {
                    var diagByCode = diagnoses.Select(d => d.Code).OrderBy(c => c).ToList();
                    for (int j = 0; j < diagByCode.Count; j++) {
                        for (int k = j + 1; k < diagByCode.Count; k++) {
                            var pair = (diagByCode[j], diagByCode[k]);
                            if (!sum.CoOccurrenceMatrix.ContainsKey(pair))
                                sum.CoOccurrenceMatrix[pair] = 0;
                            sum.CoOccurrenceMatrix[pair]++;
                        }
                    }
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
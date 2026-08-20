using PHD2Insight.Analysis.Frequency;
using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Analysis.Tests.Integration;

public sealed class FrequencyAnalysisExplorationTests {

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
    public void Inspect_GuideFrameSampling() {

        var path = Path.Combine(
            SampleFolder,
            "PHD2_GuideLog_001.txt");

        var parser = new GuideLogParser();

        using var stream = File.OpenRead(path);

        var parseResult = parser.Parse(stream);

        Assert.True(
            parseResult.Success,
            string.Join(
                Environment.NewLine,
                parseResult.Errors));

        Assert.NotNull(parseResult.Value);

        var guideLog = parseResult.Value;

        foreach (var (session, index) in
                 guideLog.Sessions.Select((s, i) => (s, i))) {

            var frames = session.Frames
                .OrderBy(frame => frame.ElapsedTime)
                .ToList();

            Assert.NotEmpty(frames);

            var intervals = frames
                .Zip(
                    frames.Skip(1),
                    (a, b) =>
                        (b.ElapsedTime - a.ElapsedTime).TotalSeconds)
                .ToList();

            var medianInterval =
                Median(intervals);

            Console.WriteLine(
                $"Session {index}");

            Console.WriteLine(
                $"  Exposure: " +
                $"{session.ExposureMilliseconds} ms");

            Console.WriteLine(
                $"  Frames: {frames.Count}");

            Console.WriteLine(
                $"  Duration: " +
                $"{frames[^1].ElapsedTime.TotalSeconds:F1}s");

            Console.WriteLine(
                $"  Median interval: " +
                $"{medianInterval:F4}s");

            Console.WriteLine();

            PrintIntervalDistribution(intervals);

            var gapThreshold =
                medianInterval * 1.5;

            var gaps = intervals
                .Select((interval, intervalIndex) =>
                    new {
                        Interval = interval,
                        Index = intervalIndex
                    })
                .Where(x => x.Interval > gapThreshold)
                .ToList();

            Console.WriteLine(
                $"  Gap threshold: " +
                $"{gapThreshold:F2}s");

            Console.WriteLine(
                $"  Significant gaps: " +
                $"{gaps.Count}");

            Console.WriteLine();

            foreach (var gap in gaps) {

                var before = frames[gap.Index];
                var after = frames[gap.Index + 1];

                Console.WriteLine(
                    $"  GAP: " +
                    $"{gap.Interval:F3}s");

                Console.WriteLine(
                    $"    Before: " +
                    $"Frame {before.FrameNumber}, " +
                    $"{before.ElapsedTime.TotalSeconds:F3}s, " +
                    $"RA {before.RaErrorArcSeconds:F3}\", " +
                    $"DEC {before.DecErrorArcSeconds:F3}\"");

                Console.WriteLine(
                    $"    After:  " +
                    $"Frame {after.FrameNumber}, " +
                    $"{after.ElapsedTime.TotalSeconds:F3}s, " +
                    $"RA {after.RaErrorArcSeconds:F3}\", " +
                    $"DEC {after.DecErrorArcSeconds:F3}\"");

                Console.WriteLine(
                    $"    RA pulse before: " +
                    $"{FormatPulse(before.RaPulseMilliseconds)}");

                Console.WriteLine(
                    $"    DEC pulse before: " +
                    $"{FormatPulse(before.DecPulseMilliseconds)}");

                Console.WriteLine();
            }

            Console.WriteLine(
                "----------------------------------------");

            Console.WriteLine();
        }
    }

    private static void PrintIntervalDistribution(
        IReadOnlyList<double> intervals) {

        var buckets = new[]
        {
            ("< 9s", intervals.Count(i => i < 9)),
            ("9–10s", intervals.Count(i => i >= 9 && i < 10)),
            ("10–11s", intervals.Count(i => i >= 10 && i < 11)),
            ("11–12s", intervals.Count(i => i >= 11 && i < 12)),
            ("12–15s", intervals.Count(i => i >= 12 && i < 15)),
            ("15–20s", intervals.Count(i => i >= 15 && i < 20)),
            ("20–30s", intervals.Count(i => i >= 20 && i < 30)),
            (">= 30s", intervals.Count(i => i >= 30))
        };

        Console.WriteLine("  Interval distribution:");

        foreach (var (label, count) in buckets) {
            Console.WriteLine(
                $"    {label,-8} {count}");
        }

        Console.WriteLine();
    }

    private static string FormatPulse(
        double? milliseconds) =>
        milliseconds.HasValue
            ? $"{milliseconds.Value:F1} ms"
            : "none";

    private static double Median(
        IReadOnlyList<double> values) {

        var ordered = values
            .OrderBy(v => v)
            .ToArray();

        var middle = ordered.Length / 2;

        return ordered.Length % 2 == 0
            ? (ordered[middle - 1] + ordered[middle]) / 2.0
            : ordered[middle];
    }
    private static GuideLog AssertParsed(ParseResult<GuideLog> result) {
        Assert.True(
            result.Success,
            $"Parsing failed:{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors)}");

        Assert.NotNull(result.Value);

        return result.Value!;
    }
    [Fact]
    public void Analyse_Guide_Loop_Interval_Multiples() {
        var parser = new GuideLogParser();

        var path = Path.Combine(
            SampleFolder,
            "PHD2_GuideLog_001.txt");

        using var stream = File.OpenRead(path);

        var guideLog = AssertParsed(parser.Parse(stream));

        for (var sessionIndex = 0; sessionIndex < guideLog.Sessions.Count; sessionIndex++) {

            var session = guideLog.Sessions[sessionIndex];
            var frames = session.Frames;

            if (frames.Count < 2) {
                continue;
            }

            var exposureSeconds =
                session.ExposureMilliseconds / 1000.0;

            var intervals = frames
                .Zip(
                    frames.Skip(1),
                    (before, after) =>
                        (after.ElapsedTime - before.ElapsedTime).TotalSeconds)
                .Where(interval => interval > 0)
                .ToList();

            var median = Median(intervals);

            Console.WriteLine($"Session {sessionIndex}");
            Console.WriteLine($"  Exposure: {session.ExposureMilliseconds} ms");
            Console.WriteLine($"  Median interval: {median:F4}s");
            Console.WriteLine("  Interval multiples of exposure:");

            Console.WriteLine("  Interval multiples of exposure:");

            var groups = intervals
                .GroupBy(interval =>
                    (int)Math.Round(
                        interval / exposureSeconds,
                        MidpointRounding.AwayFromZero))
                .OrderBy(group => group.Key);

            foreach (var group in groups) {
                var average = group.Average();
                var minimum = group.Min();
                var maximum = group.Max();

                Console.WriteLine(
                    $"    ~{group.Key * exposureSeconds:0}s" +
                    $" ({group.Key}x):" +
                    $" {group.Count(),4}" +
                    $"  avg {average,7:F3}s" +
                    $"  range {minimum:F3}-{maximum:F3}s");
            }

            Console.WriteLine("  Median interval: " +
                              $"{median:F4}s");

            Console.WriteLine("----------------------------------------");
        }
    }
    [Fact]
    public void Analyse_Guide_Loop_Gaps_And_Corrections() {
        var parser = new GuideLogParser();

        var path = Path.Combine(
            SampleFolder,
            "PHD2_GuideLog_001.txt");

        using var stream = File.OpenRead(path);

        var guideLog = AssertParsed(parser.Parse(stream));

        for (var sessionIndex = 0; sessionIndex < guideLog.Sessions.Count; sessionIndex++) {

            var session = guideLog.Sessions[sessionIndex];
            var frames = session.Frames;

            if (frames.Count < 2) {
                continue;
            }

            var exposureSeconds =
                session.ExposureMilliseconds / 1000.0;

            var intervals = frames
                .Zip(
                    frames.Skip(1),
                    (before, after) => new {
                        Before = before,
                        After = after,
                        Interval =
                            (after.ElapsedTime - before.ElapsedTime)
                                .TotalSeconds
                    })
                .Where(x => x.Interval > 0)
                .ToList();

            var median = Median(
                intervals.Select(x => x.Interval).ToList());

            // Anything more than 1.5 exposure periods above
            // the normal interval is interesting.
            var gapThreshold = median + exposureSeconds * 1.5;

            var gaps = intervals
                .Where(x => x.Interval > gapThreshold)
                .ToList();

            Console.WriteLine($"Session {sessionIndex}");
            Console.WriteLine($"  Exposure: {session.ExposureMilliseconds} ms");
            Console.WriteLine($"  Median interval: {median:F3}s");
            Console.WriteLine($"  Gap threshold: {gapThreshold:F3}s");
            Console.WriteLine($"  Significant gaps: {gaps.Count}");

            var summary = gaps
                .GroupBy(x =>
                    (int)Math.Round(
                        x.Interval / exposureSeconds,
                        MidpointRounding.AwayFromZero))
                .OrderBy(g => g.Key);

            Console.WriteLine();
            Console.WriteLine("  Gap distribution:");

            foreach (var group in summary) {
                Console.WriteLine(
                    $"    {group.Key}x exposure (~{group.Key * exposureSeconds:0}s):" +
                    $" {group.Count()}");
            }

            Console.WriteLine();
            Console.WriteLine("  Correction correlation:");

            var withRaPulse = gaps.Count(
                x => x.Before.RaPulseMilliseconds.HasValue);

            var withoutRaPulse = gaps.Count(
                x => !x.Before.RaPulseMilliseconds.HasValue);

            var withDecPulse = gaps.Count(
                x => x.Before.DecPulseMilliseconds.HasValue);

            var withoutDecPulse = gaps.Count(
                x => !x.Before.DecPulseMilliseconds.HasValue);

            Console.WriteLine(
                $"    RA pulse before gap:    {withRaPulse,4} / {gaps.Count}");

            Console.WriteLine(
                $"    No RA pulse:            {withoutRaPulse,4} / {gaps.Count}");

            Console.WriteLine(
                $"    DEC pulse before gap:   {withDecPulse,4} / {gaps.Count}");

            Console.WriteLine(
                $"    No DEC pulse:           {withoutDecPulse,4} / {gaps.Count}");

            if (gaps.Count > 0) {
                Console.WriteLine();
                Console.WriteLine("  Gap details:");

                foreach (var gap in gaps) {
                    var multiple =
                        gap.Interval / exposureSeconds;

                    Console.WriteLine(
                        $"    {gap.Interval,7:F3}s " +
                        $"({multiple:F2}x) " +
                        $"Frame {gap.Before.FrameNumber} -> " +
                        $"{gap.After.FrameNumber} | " +
                        $"RA pulse: " +
                        $"{FormatPulse(gap.Before.RaPulseMilliseconds),7} | " +
                        $"DEC pulse: " +
                        $"{FormatPulse(gap.Before.DecPulseMilliseconds),7} | " +
                        $"RA error: " +
                        $"{gap.Before.RaErrorArcSeconds,6:F3}\" | " +
                        $"DEC error: " +
                        $"{gap.Before.DecErrorArcSeconds,6:F3}\"");
                }
            }

            Console.WriteLine("----------------------------------------");
        }
    }
    [Fact]
    public void FindsDominantFrequency_InRegularSamples() {
        const double period = 60.0;

        var times = Enumerable
            .Range(0, 300)
            .Select(i => i * 5.0)
            .ToArray();

        var values = times
            .Select(t => Math.Sin(2.0 * Math.PI * t / period))
            .ToArray();

        var result = LombScarglePeriodogram.FindDominantFrequency(
            times,
            values,
            minimumPeriodSeconds: 20,
            maximumPeriodSeconds: 120);

        Assert.NotNull(result);

        Assert.InRange(
            result.PeriodSeconds,
            58.0,
            62.0);
    }
    [Fact]
    public void FindsDominantFrequency_WithIrregularSampling() {
        const double period = 75.0;

        var intervals = new[]
        {
        9.1, 9.0, 9.2, 14.9, 9.0,
        9.1, 18.0, 9.0, 9.2, 15.1,
        9.0, 9.1, 9.0, 21.0, 9.1,
        9.0, 15.0, 9.1, 9.0, 18.1,
        9.0, 9.1, 9.0, 15.0, 9.1,
        9.0, 9.1, 18.0, 9.0, 9.1
    };

        var times = new double[intervals.Length + 1];

        for (var i = 0; i < intervals.Length; i++) {
            times[i + 1] = times[i] + intervals[i];
        }

        var values = times
            .Select(t => Math.Sin(2.0 * Math.PI * t / period))
            .ToArray();

        var result = LombScarglePeriodogram.FindDominantFrequency(
            times,
            values,
            minimumPeriodSeconds: 20,
            maximumPeriodSeconds: 150);

        Assert.NotNull(result);

        Assert.InRange(
            result.PeriodSeconds,
            70.0,
            80.0);
    }
    [Fact]
    public void ReturnsNull_ForConstantSignal() {
        var times = Enumerable
            .Range(0, 100)
            .Select(i => i * 9.0)
            .ToArray();

        var values = Enumerable
            .Repeat(0.5, times.Length)
            .ToArray();

        var result = LombScarglePeriodogram.FindDominantFrequency(
            times,
            values,
            minimumPeriodSeconds: 20,
            maximumPeriodSeconds: 400);

        Assert.Null(result);
    }
}
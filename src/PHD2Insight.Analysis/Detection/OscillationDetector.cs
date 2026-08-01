using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Detection;

internal static class OscillationDetector {
    public static IReadOnlyList<OscillationEvent> Detect(
        IReadOnlyList<GuideFrame> frames,
        Func<GuideFrame, double> selector) {
        var peaks = PeakDetector.Detect(
            frames,
            selector);

        if (peaks.Count < 2) {
            return Array.Empty<OscillationEvent>();
        }

        var events = new List<OscillationEvent>();

/*        Console.WriteLine("Peaks");
        foreach (var peak in peaks) {
            Console.WriteLine(
                $"{peak.ElapsedTime.TotalSeconds,6:F1}s  {peak.Value,6:F2}\"");
        }
*/
        foreach (var pair in peaks.Zip(peaks.Skip(1))) {
            var previous = pair.First;
            var current = pair.Second;

            // Should never happen now.
            if (Math.Sign(previous.Value) ==
                Math.Sign(current.Value)) {
                continue;
            }

            var amplitude =
                (Math.Abs(previous.Value) +
                 Math.Abs(current.Value)) / 2.0;

            if (amplitude <
                OscillationThresholds.MinimumOscillationAmplitudeArcSeconds) {
                continue;
            }

            var e = new OscillationEvent(
                    previous.ElapsedTime,
                    current.ElapsedTime,
                    Math.Max(previous.Value, current.Value),
                    Math.Min(previous.Value, current.Value));
/*            Console.WriteLine(
                $"Oscillation: {amplitude:F2}\" " +
                $"Period={e.Period.TotalSeconds,5:F1}s  " +
                $"Amp={e.MeanAmplitudeArcSeconds,4:F2}\"  " +
                $"P2P={e.PeakToPeakAmplitudeArcSeconds,4:F2}\"");
*/
            events.Add(e);
        }

        return events;
    }
}

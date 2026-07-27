using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Core.Models;

internal static class OscillationDetector {
    public static IReadOnlyList<OscillationEvent> Detect(
        IReadOnlyList<GuideFrame> frames,
        Func<GuideFrame, double> selector) {
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentNullException.ThrowIfNull(selector);

        if (frames.Count < 2) {
            return Array.Empty<OscillationEvent>();
        }

        var events = new List<OscillationEvent>();

        var previousFrame = frames[0];
        var previousValue = selector(previousFrame);

        for (int i = 1; i < frames.Count; i++) {
            var currentFrame = frames[i];
            var currentValue = selector(currentFrame);

            if (CrossesZero(
                previousValue,
                currentValue)) {
                events.Add(
                    new OscillationEvent(
                        currentFrame.ElapsedTime,
                        previousValue,
                        currentValue));
            }

            previousFrame = currentFrame;
            previousValue = currentValue;
        }

        return events;
    }

    private static bool CrossesZero(
    double previous,
    double current) {
        if (Math.Abs(previous) <
            OscillationThresholds.MinimumAmplitudeArcSeconds) {
            return false;
        }

        if (Math.Abs(current) <
            OscillationThresholds.MinimumAmplitudeArcSeconds) {
            return false;
        }

        return Math.Sign(previous) != Math.Sign(current);
    }

}

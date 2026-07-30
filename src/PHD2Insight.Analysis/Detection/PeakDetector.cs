using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Detection;

internal static class PeakDetector {
    public static IReadOnlyList<Peak> Detect(
        IReadOnlyList<GuideFrame> frames,
        Func<GuideFrame, double> selector) {
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentNullException.ThrowIfNull(selector);

        if (frames.Count == 0) {
            return Array.Empty<Peak>();
        }

        var peaks = new List<Peak>();

        Peak? currentPeak = null;
        int currentSign = 0;

        foreach (var frame in frames) {
            var value = selector(frame);
            var sign = Math.Sign(value);

            // Ignore exact zero values.
            if (sign == 0) {
                continue;
            }

            // Starting a new excursion.
            if (currentSign == 0) {
                currentSign = sign;
                currentPeak = new Peak(frame.ElapsedTime, value);
                continue;
            }

            // Same excursion?
            if (sign == currentSign) {
                if (currentSign > 0) {
                    if (value > currentPeak?.Value) {
                        currentPeak = new Peak(frame.ElapsedTime, value);
                    }
                } else {
                    if (value < currentPeak?.Value) {
                        currentPeak = new Peak(frame.ElapsedTime, value);
                    }
                }

                continue;
            }

            // Excursion finished - emit the peak.
            if (currentPeak is not null) {
                peaks.Add(currentPeak);
            }

            // Begin the next excursion.
            currentSign = sign;
            currentPeak = new Peak(frame.ElapsedTime, value);
        }

        return peaks;
    }
}
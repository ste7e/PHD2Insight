using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class SettlingAnalysis {
    public static SettlingResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var durations = new List<TimeSpan>();

        TimeSpan? settlingStarted = null;

        var successfulSettles = 0;
        var failedSettles = 0;
        var cancelledSettles = 0;

        foreach (var settlingEvent in session.SettlingEvents) {
            switch (settlingEvent.State) {
                case SettlingState.Started:
                    settlingStarted = settlingEvent.ElapsedTime;
                    break;

                case SettlingState.Completed:

                    if (settlingStarted is not null) {
                        durations.Add(
                            settlingEvent.ElapsedTime - settlingStarted.Value);

                        successfulSettles++;
                        settlingStarted = null;
                    }

                    break;

                case SettlingState.Failed:

                    failedSettles++;
                    settlingStarted = null;
                    break;

                case SettlingState.Cancelled:

                    cancelledSettles++;
                    settlingStarted = null;
                    break;
            }
        }

        return new SettlingResult {
            SettlingAttemptCount =
                successfulSettles +
                failedSettles +
                cancelledSettles,

            SuccessfulSettles = successfulSettles,

            FailedSettles = failedSettles,

            CancelledSettles = cancelledSettles,

            AverageSettlingTime =
                durations.Count == 0
                    ? TimeSpan.Zero
                    : TimeSpan.FromTicks(
                        Convert.ToInt64(
                            durations.Average(d => d.Ticks))),

            ShortestSettlingTime =
                durations.Count == 0
                    ? TimeSpan.Zero
                    : durations.Min(),

            LongestSettlingTime =
                durations.Count == 0
                    ? TimeSpan.Zero
                    : durations.Max()
        };
    }
}
using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Internal;

internal sealed class GuidingSessionBuilder {
    public DateTime StartTime { get; }

    public DateTime? EndTime { get; private set; }

    public GuidingSessionBuilder(DateTime startTime) {
        StartTime = startTime;
    }

    public void Close(DateTime endTime) {
        EndTime = endTime;
    }

    public GuidingSession Build() {
        return new GuidingSession {
            StartTime = StartTime,
            EndTime = EndTime
        };
    }
}
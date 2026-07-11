namespace PHD2Insight.Core.Models;


public sealed class GuidingSession {
    public List<GuideSample> Samples { get; }
        = new();


    public DateTime? StartTime =>
        Samples.Count == 0
            ? null
            : Samples.First().Timestamp;


    public DateTime? EndTime =>
        Samples.Count == 0
            ? null
            : Samples.Last().Timestamp;


    public TimeSpan Duration =>
        StartTime.HasValue && EndTime.HasValue
            ? EndTime.Value - StartTime.Value
            : TimeSpan.Zero;
}
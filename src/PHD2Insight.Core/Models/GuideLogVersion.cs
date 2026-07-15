namespace PHD2Insight.Core.Models;

public sealed record GuideLogVersion {
    public string Phd2Version { get; init; } = string.Empty;

    public string LogVersion { get; init; } = string.Empty;
}
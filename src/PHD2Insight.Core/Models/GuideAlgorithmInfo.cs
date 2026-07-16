public sealed class GuideAlgorithmInfo {
    public string Name { get; set; } = string.Empty;

    public double? MinimumMove { get; set; }

    public double? Aggression { get; set; }

    public double? ControlGain { get; set; }

    public double? PredictionGain { get; set; }

    public bool? FastSwitch { get; set; }
}
using PHD2Insight.Analysis.Observations;

public static class ObservationExtensions {
    public static bool Has(
        this IReadOnlyList<Observation> observations,
        string code) {
        return observations.Any(o => o.Code == code);
    }
}
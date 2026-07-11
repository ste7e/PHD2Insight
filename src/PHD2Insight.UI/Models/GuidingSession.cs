namespace PHD2Insight.UI.Models;

public class GuidingSession {
    public List<GuideFrame> Frames { get; } = new();

    public double RaRms =>
        CalculateRms(Frames.Select(x => x.RaError));

    public double DecRms =>
        CalculateRms(Frames.Select(x => x.DecError));

    public double TotalRms =>
        Math.Sqrt(
            RaRms * RaRms +
            DecRms * DecRms);

    public double MaxRa =>
        Frames.Count == 0
        ? 0
        : Frames.Max(x => Math.Abs(x.RaError));

    public double MaxDec =>
        Frames.Count == 0
        ? 0
        : Frames.Max(x => Math.Abs(x.DecError));


    private static double CalculateRms(IEnumerable<double> values) {
        var list = values.ToList();

        if (list.Count == 0)
            return 0;

        return Math.Sqrt(
            list.Sum(v => v * v)
            /
            list.Count);
    }
}
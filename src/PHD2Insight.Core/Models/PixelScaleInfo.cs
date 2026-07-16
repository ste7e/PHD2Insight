public sealed record PixelScaleInfo {
    public double PixelScale { get; init; }

    public int Binning { get; init; }

    public int FocalLengthMm { get; init; }
}
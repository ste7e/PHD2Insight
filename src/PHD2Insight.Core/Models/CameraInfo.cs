public sealed record CameraInfo {
    public string Name { get; init; } = string.Empty;

    public int Gain { get; init; }

    public double PixelSizeMicrons { get; init; }
}
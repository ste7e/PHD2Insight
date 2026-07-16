using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Internal;

internal sealed class GuidingSessionBuilder {
    public DateTime StartTime { get; }

    public DateTime? EndTime { get; private set; }

    public GuidingSessionBuilder(DateTime startTime) {
        StartTime = startTime;
    }

    public EquipmentProfile? Equipment { get; set; }

    public int ExposureMilliseconds { get; set; }

    public double PixelScale { get; set; }

    public int Binning { get; set; }

    public int FocalLengthMm { get; set; }

    public CameraInfo? Camera { get; set; }

    public MountInfo? Mount { get; set; }

    public GuideAlgorithmInfo? xGuideAlgorithm { get; set; }

    public GuideAlgorithmInfo? yGuideAlgorithm { get; set; }

    public void Close(DateTime endTime) {
        EndTime = endTime;
    }

    public GuidingSession Build() {
        return new GuidingSession {
            StartTime = StartTime,
            EndTime = EndTime,
            Equipment = Equipment,
            ExposureMilliseconds = ExposureMilliseconds,
            Binning = Binning,
            FocalLengthMm = FocalLengthMm,
            PixelScale = PixelScale,
            Camera = Camera,
            Mount = Mount,
            xGuideAlgorithm = xGuideAlgorithm,
            yGuideAlgorithm = yGuideAlgorithm
        };
    }
}
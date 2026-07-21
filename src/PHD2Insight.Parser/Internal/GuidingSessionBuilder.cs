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

    public GuideAlgorithmInfo? XGuideAlgorithm { get; set; }

    public GuideAlgorithmInfo? YGuideAlgorithm { get; set; }

    public GuideFrameSchema? GuideFrameSchema { get; set; }

    public ICollection<GuideFrame> Frames { get; } = [];

    internal ICollection<SettlingEvent> SettlingEvents { get; } = [];

    internal void Close(DateTime endTime) {
        EndTime = endTime;
    }

    public void AddFrame(GuideFrame frame) {
        ArgumentNullException.ThrowIfNull(frame);

        Frames.Add(frame);
    }

    public void AddSettlingEvent(SettlingEvent settlingEvent) {
        ArgumentNullException.ThrowIfNull(settlingEvent);

        SettlingEvents.Add(settlingEvent);
    }


    public GuidingSession Build() {
        var session = new GuidingSession {
            StartTime = StartTime,
            EndTime = EndTime,
            Equipment = Equipment,
            ExposureMilliseconds = ExposureMilliseconds,
            Binning = Binning,
            FocalLengthMm = FocalLengthMm,
            PixelScale = PixelScale,
            Camera = Camera,
            Mount = Mount,
            XGuideAlgorithm = XGuideAlgorithm,
            YGuideAlgorithm = YGuideAlgorithm,
            GuideFrameSchema = GuideFrameSchema,

            Frames = Frames.ToList(),
            SettlingEvents = SettlingEvents.ToList()
        };


        return session;
    }
}
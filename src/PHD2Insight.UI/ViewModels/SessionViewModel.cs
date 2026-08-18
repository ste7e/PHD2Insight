using CommunityToolkit.Mvvm.ComponentModel;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;
using PHD2Insight.UI.ViewModels;
using CommunityToolkit.Mvvm.Input;

public sealed partial class SessionViewModel :
    ViewModelBase {

    public SessionViewModel(
        int sessionNumber,
        GuidingSession session,
        AnalysisResult analysis,
        IReadOnlyList<Diagnosis> diagnoses,
        GuidingQuality quality) {

        SessionNumber = sessionNumber;
        Session = session;
        Analysis = analysis;
        Diagnoses = diagnoses;
        Quality = quality;

        ToggleDetailCommand =
            new RelayCommand(
                () => IsDetailOpen = !IsDetailOpen);
    }
    public IRelayCommand ToggleDetailCommand {
        get;
    }
    public string FrameCountDisplay =>
    $"{Analysis.SessionStatistics.FrameCount:N0}";

    public string SignalToNoiseDisplay =>
        $"{Analysis.SessionStatistics.AverageSignalToNoiseRatio:F2}";

    public string StarMassDisplay =>
        $"{Analysis.SessionStatistics.AverageStarMass:F0}";

    public string RaToDecRatioDisplay =>
        double.IsInfinity(Analysis.Rms.RaToDecRatio)
            ? "—"
            : $"{Analysis.Rms.RaToDecRatio:F2}";

    public string MeanRadialOffsetDisplay =>
        $"{Analysis.Rms.MeanRadialOffsetArcSeconds:F2}\"";

    public string MeanRaDisplay =>
        $"{Analysis.Rms.MeanRaArcSeconds:F2}\"";

    public string MeanDecDisplay =>
        $"{Analysis.Rms.MeanDecArcSeconds:F2}\"";
    public int SessionNumber {
        get;
    }

    public GuidingQuality Quality {
        get;
    }

    public string QualityDisplay =>
    Quality switch {
        GuidingQuality.Good => "Good",
        GuidingQuality.Acceptable => "Acceptable",
        GuidingQuality.Poor => "Poor",
        _ => "Unknown"
    };

    public GuidingSession Session {
        get;
    }


    public AnalysisResult Analysis {
        get;
    }


    public IReadOnlyList<Diagnosis> Diagnoses {
        get;
    }


    public double? TotalRmsValue =>
        Analysis.Rms?.TotalArcSeconds;


    public string TotalRms =>
        TotalRmsValue.HasValue
            ? $"{TotalRmsValue.Value:F2}\""
            : "—";


    // date is missing from this as it can be inferred from the log header
    public string StartTime =>
        Session.StartTime.ToString(
            "HH:mm:ss");


    public string Duration {

        get {

            if (Session.EndTime is null)
                return "—";

            var duration =
                Session.EndTime.Value -
                Session.StartTime;

            return duration.TotalMinutes >= 60
                ? duration.ToString(@"h\:mm\:ss")
                : duration.ToString(@"m\:ss");
        }
    }


    public string Camera =>
        Session.Camera?.Name ?? "—";


    public string Mount =>
        Session.Mount?.Name ?? "—";


    public string Exposure =>
        $"{Session.ExposureMilliseconds} ms";


    public string PixelScale =>
        $"{Session.PixelScale:F2}\"/px";


    public string FocalLength =>
        $"{Session.FocalLengthMm} mm";


    public string Binning =>
        Session.Binning.ToString();

    public string SessionTitle =>
    $"{SessionNumber}";

    public string DurationDisplay =>
        $"{Duration}";

    public string TotalRmsDisplay =>
        $"{TotalRms}";

    public string RaRmsDisplay =>
        Analysis.Rms?.RaArcSeconds is double value
            ? $"{value:F2}\""
            : "—";

    public string DecRmsDisplay =>
        Analysis.Rms?.DecArcSeconds is double value
            ? $"{value:F2}\""
            : "—";

    public string RaOscillationRateDisplay =>
    $"{Analysis.OscillationMetrics.RaOscillationEventsPerMinute:F1}/min";

    public string DecOscillationRateDisplay =>
        $"{Analysis.OscillationMetrics.DecOscillationEventsPerMinute:F1}/min";

    public string RaOscillationAmplitudeDisplay =>
        $"{Analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds:F2}\"";

    public string DecOscillationAmplitudeDisplay =>
        $"{Analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds:F2}\"";

    public string RaDirectionChangesDisplay =>
        $"{Analysis.OscillationMetrics.RaDirectionChangesPerMinute:F1}/min";

    public string DecDirectionChangesDisplay =>
        $"{Analysis.OscillationMetrics.DecDirectionChangesPerMinute:F1}/min";

    public string RaGuideReversalRateDisplay =>
        $"{Analysis.GuideReversals.RaReversalRatePerMinute:F1}/min";

    public string DecGuideReversalRateDisplay =>
        $"{Analysis.GuideReversals.DecReversalRatePerMinute:F1}/min";

    public string RaCorrectionCountDisplay =>
        $"{Analysis.GuideCorrections.RaCorrectionCount:N0}";

    public string DecCorrectionCountDisplay =>
        $"{Analysis.GuideCorrections.DecCorrectionCount:N0}";

    public string RaAveragePulseDisplay =>
        $"{Analysis.GuideCorrections.AverageRaPulseMilliseconds:F0} ms";

    public string DecAveragePulseDisplay =>
        $"{Analysis.GuideCorrections.AverageDecPulseMilliseconds:F0} ms";

    public string LostStarsDisplay =>
        Analysis.LostStars.LostStarCount == 0
            ? "None"
            : $"{Analysis.LostStars.LostStarCount} ({Analysis.LostStars.LostStarPercentage:F1}%)";

    public string SettlingDisplay =>
        Analysis.Settling.SettlingAttemptCount == 0
            ? "No settling data"
            : $"{Analysis.Settling.SuccessfulSettles}/{Analysis.Settling.SettlingAttemptCount} successful";

    public string AverageSettlingTimeDisplay =>
        Analysis.Settling.SettlingAttemptCount == 0
            ? "—"
            : Analysis.Settling.AverageSettlingTime.ToString(@"s\.fff");

    public string LongestSettlingTimeDisplay =>
        Analysis.Settling.SettlingAttemptCount == 0
            ? "—"
            : Analysis.Settling.LongestSettlingTime.ToString(@"s\.fff");


    public string RaMaximumPulseDisplay =>
        $"{Analysis.GuideCorrections.MaximumRaPulseMilliseconds:F0} ms";

    public string DecMaximumPulseDisplay =>
        $"{Analysis.GuideCorrections.MaximumDecPulseMilliseconds:F0} ms";

    public string RaTotalCorrectionTimeDisplay =>
        Analysis.GuideCorrections.TotalRaCorrectionTime.ToString(@"m\:ss");

    public string DecTotalCorrectionTimeDisplay =>
        Analysis.GuideCorrections.TotalDecCorrectionTime.ToString(@"m\:ss");

    public string RaDirectionalImbalanceDisplay =>
        $"{Analysis.GuideCorrections.RaDirectionalImbalance:F2}";

    public string DecDirectionalImbalanceDisplay =>
        $"{Analysis.GuideCorrections.DecDirectionalImbalance:F2}";

    public string DiagnosisCountDisplay =>
        Diagnoses.Count == 0
            ? "No issues"
            : $"{Diagnoses.Count} finding{(Diagnoses.Count == 1 ? "" : "s")}";

    public sealed record DiagnosisDisplayItem(
    string Title,
    string Description);

    public IReadOnlyList<DiagnosisDisplayItem> DiagnosisItems =>
        Diagnoses
            .Select(d => new DiagnosisDisplayItem(
                d.Code.ToString(),
                d.Description))
            .ToList();

    [ObservableProperty]
    private bool isDetailOpen;

    public string QualityForeground =>
    Quality switch {
        GuidingQuality.Good => "Green",
        GuidingQuality.Acceptable => "DarkOrange",
        GuidingQuality.Poor => "Red",
        _ => "Gray"
    };

    public string DiagnosisSeverityForeground(DiagnosisSeverity severity) =>
        severity switch {
            DiagnosisSeverity.Information => "Gray",
            DiagnosisSeverity.Warning => "DarkOrange",
            DiagnosisSeverity.Critical => "Red",
            _ => "Gray"
        };

    public string DiagnosisConfidenceForeground(DiagnosisConfidence confidence) =>
        confidence switch {
            DiagnosisConfidence.High => "Green",
            DiagnosisConfidence.Medium => "DarkOrange",
            DiagnosisConfidence.Low => "Gray",
            _ => "Gray"
        };

    public string GetSeverityForeground(DiagnosisSeverity severity) =>
    severity switch {
        DiagnosisSeverity.Information => "Gray",
        DiagnosisSeverity.Warning => "DarkOrange",
        DiagnosisSeverity.Critical => "Red",
        _ => "Gray"
    };

    public string GetConfidenceForeground(DiagnosisConfidence confidence) =>
        confidence switch {
            DiagnosisConfidence.High => "Green",
            DiagnosisConfidence.Medium => "DarkOrange",
            DiagnosisConfidence.Low => "Gray",
            _ => "Gray"
        };

    public IReadOnlyList<DiagnosisViewModel> DiagnosisViewModels =>
    Diagnoses
        .Select(d => new DiagnosisViewModel(d))
        .ToList();

    public string PeakRaErrorDisplay =>
    $"{Analysis.PeakErrors.MaximumRaErrorArcSeconds:F2}\"";

    public string PeakDecErrorDisplay =>
        $"{Analysis.PeakErrors.MaximumDecErrorArcSeconds:F2}\"";

    public string PeakTotalErrorDisplay =>
        $"{Analysis.PeakErrors.MaximumTotalErrorArcSeconds:F2}\"";

    public string DiagnosisCountNumberDisplay =>
    Diagnoses.Count.ToString();
}
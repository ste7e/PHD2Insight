using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Observations;

public static class ObservationEngine {

    public static IReadOnlyList<Observation> Observe(
        AnalysisResult analysis) {

        var observations = new List<Observation>();

        AddObservationIf(
            analysis.Rms.RaArcSeconds >= DiagnosisThresholds.HighRaRmsArcSeconds,
            observations,
            ObservationCodes.HighRaRms,
            "RA RMS",
            $"{analysis.Rms.RaArcSeconds:F2}\"",
            "RA RMS exceeds the expected range.",
            ObservationWeights.HighRaRms);

        AddObservationIf(
            analysis.Rms.RaToDecRatio >= DiagnosisThresholds.HighRaToDecRatio,
            observations,
            ObservationCodes.RaDominance,
            "RA/DEC RMS Ratio",
            $"{analysis.Rms.RaToDecRatio:F2}",
            "RA guiding errors dominate DEC.",
            ObservationWeights.RaDominance);

        if (!AddObservationIf(
            analysis.OscillationMetrics.RaOscillationEventsPerMinute >=
            DiagnosisThresholds.HighRaOscillationEventsPerMinute,
            observations,
            ObservationCodes.HighRaOscillationRate,
            "RA Oscillation Rate",
            $"{analysis.OscillationMetrics.RaOscillationEventsPerMinute:F2}",
            "Frequent oscillation activity detected.",
            ObservationWeights.HighRateRaOscillationEvents)) {
            AddObservationIf(
                analysis.OscillationMetrics.RaOscillationEventsPerMinute >=
                DiagnosisThresholds.MediumRaOscillationEventsPerMinute,
                observations,
                ObservationCodes.MediumRaOscillationRate,
                "RA Oscillation Rate",
                $"{analysis.OscillationMetrics.RaOscillationEventsPerMinute:F2}",
                "Moderate oscillation activity detected.",
                ObservationWeights.MediumRateRaOscillationEvents);
        }

        if (!AddObservationIf(
            analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds >=
            DiagnosisThresholds.HighRaOscillationAmplitudeArcSeconds,
            observations,
            ObservationCodes.HighRaOscillationAmplitude,
            "RA Oscillation Amplitude",
            $"{analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds:F2}\"",
            "Large oscillation amplitude detected.",
            ObservationWeights.HighRaOscillationAmplitude)) {
            AddObservationIf(
                analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds >=
                DiagnosisThresholds.MediumRaOscillationAmplitudeArcSeconds,
                observations,
                ObservationCodes.MediumRaOscillationAmplitude,
                "RA Oscillation Amplitude",
                $"{analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds:F2}\"",
                "Moderate oscillation amplitude detected.",
                ObservationWeights.MediumRaOscillationAmplitude);
        }

        AddObservationIf(
            analysis.GuideCorrections.AverageRaPulseMilliseconds >=
            DiagnosisThresholds.LargeAverageRaPulseMilliseconds,
            observations,
            ObservationCodes.LargeRaGuidePulses,
            "Average RA Pulse",
            $"{analysis.GuideCorrections.AverageRaPulseMilliseconds:F0} ms",
            "Guide corrections are consistently large.",
            ObservationWeights.LargeGuidePulses);


        // DEC oscillation rate

        if (!AddObservationIf(
            analysis.OscillationMetrics.DecOscillationEventsPerMinute >=
                DiagnosisThresholds.HighDecOscillationEventsPerMinute,
            observations,
            ObservationCodes.HighDecOscillationRate,
            "DEC Oscillation Rate",
            $"{analysis.OscillationMetrics.DecOscillationEventsPerMinute:F2}",
            "Frequent oscillation events were detected in the DEC axis.",
            ObservationWeights.HighDecOscillationRate)) {

            AddObservationIf(
                analysis.OscillationMetrics.DecOscillationEventsPerMinute >=
                    DiagnosisThresholds.MediumDecOscillationEventsPerMinute,
                observations,
                ObservationCodes.MediumDecOscillationRate,
                "DEC Oscillation Rate",
                $"{analysis.OscillationMetrics.DecOscillationEventsPerMinute:F2}",
                "Moderate oscillation events were detected in the DEC axis.",
                ObservationWeights.MediumDecOscillationRate);
        }

        // DEC oscillation amplitude

        if (!AddObservationIf(
            analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds >=
                DiagnosisThresholds.HighDecOscillationAmplitudeArcSeconds,
            observations,
            ObservationCodes.HighDecOscillationAmplitude,
            "DEC Oscillation Amplitude",
            $"{analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds:F2}\"",
            "Large oscillation amplitudes were detected in the DEC axis.",
            ObservationWeights.HighDecOscillationAmplitude)) {

            AddObservationIf(
                analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds >=
                    DiagnosisThresholds.MediumDecOscillationAmplitudeArcSeconds,
                observations,
                ObservationCodes.MediumDecOscillationAmplitude,
                "DEC Oscillation Amplitude",
                $"{analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds:F2}\"",
                "Moderate oscillation amplitudes were detected in the DEC axis.",
                ObservationWeights.MediumDecOscillationAmplitude);
        }

        AddObservationIf(
            analysis.Rms.DecArcSeconds >= DiagnosisThresholds.HighDecRmsArcSeconds,
            observations,
            ObservationCodes.HighDecRms,
            "DEC RMS",
            $"{analysis.Rms.DecArcSeconds:F2}\"",
            "DEC RMS exceeds the expected range.",
            ObservationWeights.HighDecRms);

        AddObservationIf(
            analysis.Rms.DecToRaRatio >= DiagnosisThresholds.HighDecToRaRatio,
            observations,
            ObservationCodes.DecDominance,
            "DEC/RA RMS Ratio",
            $"{analysis.Rms.DecToRaRatio:F2}",
            "DEC guiding errors dominate RA.",
            ObservationWeights.DecDominance);

        AddObservationIf(
            analysis.GuideCorrections.AverageDecPulseMilliseconds >=
                DiagnosisThresholds.LargeAverageDecPulseMilliseconds,
            observations,
            ObservationCodes.LargeDecGuidePulses,
            "Average DEC Pulse",
            $"{analysis.GuideCorrections.AverageDecPulseMilliseconds:F0} ms",
            "Guide corrections are consistently large.",
            ObservationWeights.LargeDecGuidePulses);

        AddLostStarObservations(
            analysis,
            observations);

        AddObservationIf(
            analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds > 0 &&
            analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds <=
                DiagnosisThresholds.LowRaOscillationAmplitudeArcSeconds,
            observations,
            ObservationCodes.LowRaOscillationAmplitude,
            "RA Oscillation Amplitude",
            $"{analysis.OscillationMetrics.MeanRaOscillationAmplitudeArcSeconds:F2}\"",
            "RA oscillation amplitude is low.",
            ObservationWeights.LowRaOscillationAmplitude);

        AddObservationIf(
            analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds > 0 &&
            analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds <=
                DiagnosisThresholds.LowDecOscillationAmplitudeArcSeconds,
            observations,
            ObservationCodes.LowDecOscillationAmplitude,
            "DEC Oscillation Amplitude",
            $"{analysis.OscillationMetrics.MeanDecOscillationAmplitudeArcSeconds:F2}\"",
            "DEC oscillation amplitude is low.",
            ObservationWeights.LowDecOscillationAmplitude);

        AddObservationIf(
            analysis.GuideCorrections.AverageRaPulseMilliseconds > 0 &&
            analysis.GuideCorrections.AverageRaPulseMilliseconds <=
                DiagnosisThresholds.NormalAverageGuidePulseMilliseconds,
            observations,
            ObservationCodes.NormalRaGuidePulses,
            "Average RA Pulse",
            $"{analysis.GuideCorrections.AverageRaPulseMilliseconds:F0} ms",
            "Average RA guide pulse is within the expected range.",
            ObservationWeights.NormalRaGuidePulses);

        AddObservationIf(
            analysis.GuideCorrections.AverageDecPulseMilliseconds > 0 &&
            analysis.GuideCorrections.AverageDecPulseMilliseconds <=
                DiagnosisThresholds.NormalAverageGuidePulseMilliseconds,
            observations,
            ObservationCodes.NormalDecGuidePulses,
            "Average DEC Pulse",
            $"{analysis.GuideCorrections.AverageDecPulseMilliseconds:F0} ms",
            "Average DEC guide pulse is within the expected range.",
            ObservationWeights.NormalDecGuidePulses);

        AddObservationIf(
            analysis.GuideReversals.RaReversalRatePerMinute >= 1.0,
            observations,
            ObservationCodes.RaGuideReversal,
            "RA Guide Reversal Rate",
            $"{analysis.GuideReversals.RaReversalRatePerMinute:F2}/min",
            "RA guide corrections frequently reverse direction after the guide error changes sign.",
            ObservationWeights.RaGuideReversal);

        AddObservationIf(
            analysis.GuideReversals.DecReversalRatePerMinute >= 1.0,
            observations,
            ObservationCodes.DecGuideReversal,
            "DEC Guide Reversal Rate",
            $"{analysis.GuideReversals.DecReversalRatePerMinute:F2}/min",
            "DEC guide corrections frequently reverse direction after the guide error changes sign.",
            ObservationWeights.DecGuideReversal);


        return observations;
    }

    private static void AddLostStarObservations(
    AnalysisResult analysis,
    ICollection<Observation> observations) {
        var percentage = analysis.LostStars.LostStarPercentage;

        if (percentage == 0)
            return;

        if (percentage >= DiagnosisThresholds.SevereLostStarPercentage) {
            observations.Add(new Observation {
                Code = ObservationCodes.SevereLostStars,
                Metric = "Lost Star Percentage",
                Value = $"{percentage:F1}%",
                Description = "Guide star loss occurred throughout the session.",
                Weight = ObservationWeights.SevereLostStars
            });

            return;
        }

        if (percentage >= DiagnosisThresholds.FrequentLostStarPercentage) {
            observations.Add(new Observation {
                Code = ObservationCodes.FrequentLostStars,
                Metric = "Lost Star Percentage",
                Value = $"{percentage:F1}%",
                Description = "Guide star loss occurred repeatedly.",
                Weight = ObservationWeights.FrequentLostStars
            });

            return;
        }

        observations.Add(new Observation {
            Code = ObservationCodes.OccasionalLostStars,
            Metric = "Lost Star Percentage",
            Value = $"{percentage:F1}%",
            Description = "A small number of guide stars were lost.",
            Weight = ObservationWeights.OccasionalLostStars
        });

    }


    private static bool AddObservationIf(
        bool condition,
        ICollection<Observation> observations,
        string code,
        string metric,
        string value,
        string description,
        int weight) {

        if (!condition) {
            return false;
        }

        observations.Add(new Observation {
            Code = code,
            Metric = metric,
            Value = value,
            Description = description,
            Weight = weight
        });

        return true;
    }
    private static void AddRaRmsObservations(
        AnalysisResult analysis,
        ICollection<Observation> observations) {
        if (analysis.Rms.RaArcSeconds >= DiagnosisThresholds.HighRaRmsArcSeconds) {
            observations.Add(new Observation {
                Code = ObservationCodes.HighRaRms,
                Metric = "RA RMS",
                Value = $"{analysis.Rms.RaArcSeconds:F2}\"",
                Description = "RA RMS exceeds the expected range.",
                Weight = ObservationWeights.HighRaRms
            });
        }
    }

    private static void AddRaDominanceObservations(
    AnalysisResult analysis,
    ICollection<Observation> observations) {
        if (analysis.Rms.RaToDecRatio >= DiagnosisThresholds.HighRaToDecRatio) {
            observations.Add(new Observation {
                Code = ObservationCodes.RaDominance,
                Metric = "RA/DEC RMS Ratio",
                Value = analysis.Rms.RaToDecRatio.ToString("F2"),
                Description = "RA guiding errors dominate DEC.",
                Weight = ObservationWeights.RaDominance
            });
        }
    }

}
using System;
using System.Collections.Generic;
using System.Text;

namespace PHD2Insight.Analysis.Observations {
    internal static class ObservationWeights {
        public const int HighRaRms = 2;

        public const int RaDominance = 3;

        public const int MediumRateRaOscillationEvents = 2;
        public const int HighRateRaOscillationEvents = 3;

        public const int FrequentDirectionReversals = 2;

        public const int MediumRaOscillationAmplitude = 2;
        public const int HighRaOscillationAmplitude = 3;

        public const int MediumDecOscillationAmplitude = 1;
        public const int HighDecOscillationAmplitude = 2;

        public const int MediumDecOscillationRate = 2;
        public const int HighDecOscillationRate = 3;

        public const int HighDecRms = 2;

        public const int DecDominance = 3;

        public const int OccasionalLostStars = 1;

        public const int FrequentLostStars = 2;

        public const int SevereLostStars = 3;

        public const int LowRaOscillationAmplitude = 1;

        public const int LowDecOscillationAmplitude = 1;

        public const int NormalRaGuidePulses = 1;

        public const int NormalDecGuidePulses = 1;

        public const int RaGuideReversal = 2;
        public const int DecGuideReversal = 2;

        public const int LargeRaGuidePulses = 1;
        public const int LargeDecGuidePulses = 1;
    }
}

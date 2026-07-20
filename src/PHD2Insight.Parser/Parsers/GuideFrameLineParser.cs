using System.Diagnostics.CodeAnalysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Parsers;

internal static class GuideFrameLineParser {
    private static class Columns {
        public const int Frame = 0;
        public const int Time = 1;
        public const int Mount = 2;
        public const int Dx = 3;
        public const int Dy = 4;
        public const int RARawDistance = 5;
        public const int DECRawDistance = 6;
        public const int RAGuideDistance = 7;
        public const int DECGuideDistance = 8;
        public const int RADuration = 9;
        public const int RADirection = 10;
        public const int DECDuration = 11;
        public const int DECDirection = 12;
        public const int XStep = 13;
        public const int YStep = 14;
        public const int StarMass = 15;
        public const int SNR = 16;
        public const int ErrorCode = 17;
    }

    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out GuideFrame? frame) {
        frame = null;

        var fields = CsvLineParser.Parse(line);

        if (!FieldValueParser.TryGetString(
                fields,
                Columns.Mount,
                out var mount)) {
            return false;
        }

        if (!string.Equals(
                mount,
                "Mount",
                StringComparison.Ordinal)) {
            return false;
        }

        if (!FieldValueParser.TryGetInt32(
                fields,
                Columns.Frame,
                out var frameNumber)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.Time,
                out var elapsedSeconds)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.Dx,
                out var dx)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.Dy,
                out var dy)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
        fields,
        Columns.RARawDistance,
        out var raErrorArcSeconds)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.DECRawDistance,
                out var decErrorArcSeconds)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.RAGuideDistance,
                out var raGuideDistance)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.DECGuideDistance,
                out var decGuideDistance)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.RADuration,
                out var raDuration)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.DECDuration,
                out var decDuration)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
        fields,
        Columns.StarMass,
        out var starMass)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.SNR,
                out var snr)) {
            return false;
        }

        if (!FieldValueParser.TryGetInt32(
                fields,
                Columns.ErrorCode,
                out var errorCode)) {
            return false;
        }

        var raDirection = GuideDirection.None;

        if (FieldValueParser.TryGetString(
                fields,
                Columns.RADirection,
                out var raDirectionText) &&
            !string.IsNullOrEmpty(raDirectionText)) {
            if (!GuideDirectionParser.TryParse(
                    raDirectionText[0],
                    out raDirection)) {
                return false;
            }
        }

        var decDirection = GuideDirection.None;

        if (FieldValueParser.TryGetString(
                fields,
                Columns.DECDirection,
                out var decDirectionText) &&
            !string.IsNullOrEmpty(decDirectionText)) {
            if (!GuideDirectionParser.TryParse(
                    decDirectionText[0],
                    out decDirection)) {
                return false;
            }
        }
        frame = new GuideFrame {
            FrameNumber = frameNumber,
            ElapsedTime = TimeSpan.FromSeconds(elapsedSeconds),

            RaErrorPixels = dx,
            DecErrorPixels = dy,

            RaErrorArcSeconds = raErrorArcSeconds,
            DecErrorArcSeconds = decErrorArcSeconds,

            RaGuideDistance = raGuideDistance,
            DecGuideDistance = decGuideDistance,

            RaPulseMilliseconds = raDuration == 0 ? null : raDuration,
            DecPulseMilliseconds = decDuration == 0 ? null : decDuration,

            RaDirection = raDirection,
            DecDirection = decDirection,

            StarMass = starMass,
            SignalToNoiseRatio = snr,
            ErrorCode = errorCode,
        };
        return true;
    }

}
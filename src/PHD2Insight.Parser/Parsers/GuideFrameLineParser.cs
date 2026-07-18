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

        frame = new GuideFrame {
            FrameNumber = frameNumber,
            ElapsedTime = TimeSpan.FromSeconds(elapsedSeconds),

            RaErrorPixels = dx,
            DecErrorPixels = dy,

            RaPulseMilliseconds = raDuration,
            DecPulseMilliseconds = decDuration
        };

        return true;
    }
}
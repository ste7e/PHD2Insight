using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PHD2Insight.Parser.Parsers;

internal static class PixelScaleLineParser {
    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out PixelScaleInfo? result) {
        result = null;

        double? pixelScale = null;
        int? binning = null;
        int? focalLength = null;

        foreach (var token in DelimitedLineParser.Parse(line)) {
            if (!PropertyLineParser.TryParse(
                    token,
                    out var key,
                    out var value)) {
                continue;
            }

            switch (key) {
                case "Pixel scale": {

                        const string Suffix = " arc-sec/px";

                        if (value.EndsWith(Suffix, StringComparison.Ordinal)) {
                            var text = value[..^Suffix.Length];

                            if (double.TryParse(
                                text,
                                NumberStyles.Float,
                                CultureInfo.InvariantCulture,
                                out var scale)) {
                                pixelScale = scale;
                            }
                        }
                    }

                    break;

                case "Binning":

                    if (int.TryParse(
                        value,
                        out var b)) {
                        binning = b;
                    }

                    break;

                case "Focal length": {
                        const string Suffix = " mm";

                        if (value.EndsWith(Suffix, StringComparison.Ordinal)) {
                            var text = value[..^Suffix.Length];

                            if (int.TryParse(
                                text,
                                out var f)) {
                                focalLength = f;
                            }
                        }
                    }
                    break;
            }
        }

        if (pixelScale is null ||
            binning is null ||
            focalLength is null) {
            return false;
        }

        result = new PixelScaleInfo {
            PixelScale = pixelScale.Value,
            Binning = binning.Value,
            FocalLengthMm = focalLength.Value
        };

        return true;
    }
}
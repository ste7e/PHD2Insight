using PHD2Insight.Parser.Parsers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

internal static class CameraInfoLineParser {
    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out CameraInfo? camera) {

        string? name = null;
        int? gain = null;
        double? pixelSize = null;
        camera = null;

        line = line.TrimStart();

        if (!line.StartsWith("Camera =", StringComparison.Ordinal)) {
            return false;
        }

        foreach (var token in DelimitedLineParser.Parse(line)) {
            if (!PropertyLineParser.TryParse(
                    token,
                    out var key,
                    out var value)) {
                continue;
            }

            switch (key) {
                case "Camera":
                    name = value;
                    break;

                case "gain":
                    if (int.TryParse(
                        value,
                        out var g)) {
                        gain = g;
                    }
                    break;

                case "pixel size": {
                        const string Suffix = " um";

                        if (value.EndsWith(Suffix, StringComparison.Ordinal)) {
                            var text = value[..^Suffix.Length];

                            if (double.TryParse(
                                text,
                                NumberStyles.Float,
                                CultureInfo.InvariantCulture,
                                out var size)) {
                                pixelSize = size;
                            }
                        }
                    }
                    break;
            }
        }

        if (name is null || pixelSize is null || gain is null) {
            return false;
        }

        camera = new CameraInfo {
            Name = name,
            Gain = gain.Value,
            PixelSizeMicrons = pixelSize.Value
        };

        return true;
    }
}
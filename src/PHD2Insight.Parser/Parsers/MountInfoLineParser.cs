using PHD2Insight.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace PHD2Insight.Parser.Parsers;

internal static class MountInfoLineParser {
    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out MountInfo? mount) {
        string? name = null;
        bool connected = false;
        bool guidingEnabled = false;

        double? xAngle = null;
        double? xRate = null;
        double? yAngle = null;
        double? yRate = null;

        string? parity = null;

        mount = null;

        line = line.TrimStart();

        if (!line.StartsWith(
                "Mount =",
                StringComparison.Ordinal)) {
            return false;
        }

        foreach (var token in DelimitedLineParser.Parse(line)) {
            if (PropertyLineParser.TryParse(
                    token,
                    out var key,
                    out var value)) {
                switch (key) {
                    case "Mount":
                        name = value;
                        break;

                    case "xAngle":
                        if (InvariantNumberParser.TryParseDouble(
                                value,
                                out var xa)) {
                            xAngle = xa;
                        }
                        break;

                    case "xRate":
                        if (InvariantNumberParser.TryParseDouble(
                                value,
                                out var xr)) {
                            xRate = xr;
                        }
                        break;

                    case "yAngle":
                        if (InvariantNumberParser.TryParseDouble(
                                value,
                                out var ya)) {
                            yAngle = ya;
                        }
                        break;

                    case "yRate":
                        if (InvariantNumberParser.TryParseDouble(
                                value,
                                out var yr)) {
                            yRate = yr;
                        }
                        break;

                    case "parity":
                        parity = value;
                        break;
                }

                continue;
            }

            switch (token) {
                case "connected":
                    connected = true;
                    break;

                case "guiding enabled":
                    guidingEnabled = true;
                    break;
            }
        }

        if (name is null ||
            xAngle is null ||
            xRate is null ||
            yAngle is null ||
            yRate is null) {
            return false;
        }

        mount = new MountInfo {
            Name = name,
            Connected = connected,
            GuidingEnabled = guidingEnabled,
            XAngleDegrees = xAngle.Value,
            XRate = xRate.Value,
            YAngleDegrees = yAngle.Value,
            YRate = yRate.Value,
            Parity = parity
        };

        return true;
    }
}
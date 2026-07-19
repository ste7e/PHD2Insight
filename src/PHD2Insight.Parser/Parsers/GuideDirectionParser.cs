using PHD2Insight.Common.Enums;

namespace PHD2Insight.Parser.Parsers;

internal static class GuideDirectionParser {
    public static bool TryParse(
        char direction,
        out GuideDirection guideDirection) {
        switch (char.ToUpperInvariant(direction)) {
            case 'N':
                guideDirection = GuideDirection.North;
                return true;

            case 'S':
                guideDirection = GuideDirection.South;
                return true;

            case 'E':
                guideDirection = GuideDirection.East;
                return true;

            case 'W':
                guideDirection = GuideDirection.West;
                return true;

            default:
                guideDirection = GuideDirection.None;
                return false;
        }
    }

    public static GuideDirection ParseOrNone(string? text) {
        if (string.IsNullOrWhiteSpace(text)) {
            return GuideDirection.None;
        }

        return text.Length == 1 &&
               TryParse(text[0], out var direction)
            ? direction
            : GuideDirection.None;
    }
}
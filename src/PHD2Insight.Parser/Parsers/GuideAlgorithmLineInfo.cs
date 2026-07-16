using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Parsers;

public sealed record GuideAlgorithmLineInfo(
    GuideAxis Axis,
    GuideAlgorithmInfo Algorithm);
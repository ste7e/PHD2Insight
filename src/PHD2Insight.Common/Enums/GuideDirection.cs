namespace PHD2Insight.Common.Enums;

/// <summary>
/// Indicates the direction of a guide correction.
/// </summary>
public enum GuideDirection {
    /// <summary>
    /// No guide pulse was issued.
    /// </summary>
    None = 0,

    /// <summary>
    /// North guide correction.
    /// </summary>
    North,

    /// <summary>
    /// South guide correction.
    /// </summary>
    South,

    /// <summary>
    /// East guide correction.
    /// </summary>
    East,

    /// <summary>
    /// West guide correction.
    /// </summary>
    West
}
public sealed record MechanicalPeriodPowerResult {
    public double RaWormFundamentalArcSeconds { get; init; }

    public double RaWormFundamentalPower { get; init; }

    public double RaWormPeriodSeconds { get; init; }

    public bool IsValid { get; init; }
}
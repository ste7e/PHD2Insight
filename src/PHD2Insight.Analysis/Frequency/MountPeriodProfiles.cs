namespace PHD2Insight.Analysis.Frequency;

public static class MountPeriodProfiles {
    public static MountPeriodProfile Eq6RPro { get; } =
        new(
            "Sky-Watcher EQ6-R Pro",

            RaPeriods:
            [
                new("RA worm fundamental", 478.69, 1),
                new("RA worm 2nd harmonic", 239.34, 2),
                new("RA worm 3rd harmonic", 159.56, 3),
                new("RA worm 4th harmonic", 119.67, 4),
                new("RA worm 5th harmonic", 95.74, 5),
                new("RA worm 6th harmonic", 79.78, 6),
                new("RA worm 7th harmonic", 68.38, 7),
                new("RA worm 8th harmonic", 59.84, 8)
            ],

            DecPeriods:
            [
                new("DEC worm fundamental", 478.69, 1),
                new("DEC worm 2nd harmonic", 239.34, 2),
                new("DEC worm 3rd harmonic", 159.56, 3),
                new("DEC worm 4th harmonic", 119.67, 4),
                new("DEC worm 5th harmonic", 95.74, 5),
                new("DEC worm 6th harmonic", 79.78, 6),
                new("DEC worm 7th harmonic", 68.38, 7),
                new("DEC worm 8th harmonic", 59.84, 8)
            ]);
}
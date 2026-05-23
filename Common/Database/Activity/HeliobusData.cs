namespace March7thHoney.Database.Activity;

/// <summary>
///     Persistent state for the "Heliobus" event (Aha's lab raid + SNS feed).
///     Tracks player level/phase, raid/battle progress and the social-feed interactions.
/// </summary>
public class HeliobusData
{
    /// <summary>Heliobus persona level.</summary>
    public uint Level { get; set; } = 1;

    /// <summary>Current event phase.</summary>
    public uint Phase { get; set; } = 1;

    /// <summary>Challenge ids the player has completed.</summary>
    public List<uint> FinishedChallenges { get; set; } = new();

    /// <summary>SNS post ids the player has read.</summary>
    public List<uint> ReadSnsIds { get; set; } = new();

    /// <summary>SNS post ids the player has liked.</summary>
    public List<uint> LikedSnsIds { get; set; } = new();

    /// <summary>Allocator for player-authored SNS post/comment ids.</summary>
    public uint NextSnsId { get; set; } = 9000;
}

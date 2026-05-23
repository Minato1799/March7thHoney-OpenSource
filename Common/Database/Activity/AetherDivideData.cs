namespace March7thHoney.Database.Activity;

/// <summary>
///     Persistent state for the "Aetherium Wars" event (AetherDivide / 以太战线).
///     Tracks the player's spirit level, equipped passive skills, lineup, and reward progress.
/// </summary>
public class AetherDivideData
{
    /// <summary>Aether spirit level / overall progress.</summary>
    public uint Level { get; set; } = 1;

    /// <summary>Accumulated spirit EXP within the current level.</summary>
    public uint SpiritExp { get; set; } = 0;

    /// <summary>Equipped passive skills keyed by slot (slot -> passive item id).</summary>
    public Dictionary<uint, uint> PassiveSkillBySlot { get; set; } = new();

    /// <summary>Challenge ids whose rewards have been claimed.</summary>
    public List<uint> TakenChallengeRewards { get; set; } = new();

    /// <summary>Avatar ids forming the current event lineup.</summary>
    public List<uint> Lineup { get; set; } = new();

    /// <summary>Endless-mode refresh counter.</summary>
    public uint EndlessRefreshCount { get; set; } = 0;
}

namespace March7thHoney.Database.Activity;

/// <summary>
///     Persistent state for the "March 7th Sword Training" event (CmdSwordTraining* family).
///     Tracks meta progression (learned/traced skills, viewed endings, unlocked story lines)
///     and the currently running game session so it can be resumed/restored across logins.
/// </summary>
public class SwordTrainingData
{
    /// <summary>Skill ids the player has permanently learned.</summary>
    public List<uint> LearnedSkills { get; set; } = new();

    /// <summary>Skill ids currently traced (highlighted) by the player.</summary>
    public List<uint> TracedSkills { get; set; } = new();

    /// <summary>Ending ids the player has already viewed.</summary>
    public List<uint> ViewedEndings { get; set; } = new();

    /// <summary>Story line ids the player has unlocked / can start a game with.</summary>
    public List<uint> UnlockedStoryLines { get; set; } = new();

    /// <summary>Whether a game session is currently in progress.</summary>
    public bool GameActive { get; set; } = false;

    /// <summary>Story line id of the active (or last) game session.</summary>
    public uint CurGameStoryLineId { get; set; } = 0;

    /// <summary>Monotonic turn counter for the active session.</summary>
    public uint CurTurn { get; set; } = 0;

    /// <summary>Daily phase counter, surfaced in the main data response.</summary>
    public uint DailyPhase { get; set; } = 0;
}

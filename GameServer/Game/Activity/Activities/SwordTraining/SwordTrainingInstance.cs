using March7thHoney.Database.Activity;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Game.Activity.Activities.SwordTraining;

/// <summary>
///     "March 7th Sword Training" event logic (CmdSwordTraining* family).
///     Drives the meta progression snapshot and the per-session game lifecycle.
///     Field semantics are reconstructed from the obfuscated proto layout, so the
///     instance favours always returning a well-formed, non-erroring response while
///     persisting the player's progress through <see cref="SwordTrainingData"/>.
/// </summary>
public class SwordTrainingInstance : BaseActivityInstance
{
    public SwordTrainingInstance(ActivityManager manager) : base(manager)
    {
        Data = manager.Data.SwordTrainingData;

        // Seed the catalogue of playable story lines the first time the event is opened
        // so the panel is browsable instead of empty.
        if (Data.UnlockedStoryLines.Count == 0)
            Data.UnlockedStoryLines.AddRange(DefaultStoryLines);
    }

    public SwordTrainingData Data { get; }

    /// <summary>Story lines unlocked by default for a fresh save.</summary>
    private static readonly uint[] DefaultStoryLines = [1001, 1002, 1003, 1004, 1005];

    #region Snapshot

    /// <summary>Builds the full event data response shown when the panel opens.</summary>
    public GetSwordTrainingDataScRsp BuildDataSnapshot()
    {
        var rsp = new GetSwordTrainingDataScRsp
        {
            Retcode = 0,
            KEAFLGNLKBO = true,
            GCJFLELINJO = Data.DailyPhase,
            FCAPBPHLNCJ = BuildUnlockInfo()
        };

        rsp.FHNPAPAMNNE.AddRange(Data.ViewedEndings);
        rsp.FOEKDMEALKF.AddRange(Data.LearnedSkills);
        rsp.HEEHPMLAHPK.AddRange(Data.TracedSkills);

        if (Data.GameActive)
            rsp.BMKAEFAKNFJ = BuildGameObject();

        return rsp;
    }

    /// <summary>Builds the story-line unlock info block.</summary>
    public LLKEGAOLGGF BuildUnlockInfo()
    {
        var info = new LLKEGAOLGGF();
        info.IFEJLJCINCI.AddRange(Data.UnlockedStoryLines);
        foreach (var storyLine in Data.UnlockedStoryLines)
            info.NKJHKMBLIBL.Add(new PPDPDGCBDEH { Id = storyLine, Progress = 0 });
        return info;
    }

    /// <summary>Builds the current game-session object.</summary>
    public LMBHDCFPPLL BuildGameObject()
    {
        return new LMBHDCFPPLL
        {
            IMBOKGFIACA = Data.CurGameStoryLineId
        };
    }

    #endregion

    #region Session lifecycle

    public LMBHDCFPPLL StartGame(uint gameStoryLineId)
    {
        if (gameStoryLineId != 0 && !Data.UnlockedStoryLines.Contains(gameStoryLineId))
            Data.UnlockedStoryLines.Add(gameStoryLineId);

        Data.GameActive = true;
        Data.CurGameStoryLineId = gameStoryLineId;
        Data.CurTurn = 0;
        return BuildGameObject();
    }

    public LMBHDCFPPLL? ResumeGame(uint gameStoryLineId)
    {
        if (!Data.GameActive) return null;
        if (gameStoryLineId != 0) Data.CurGameStoryLineId = gameStoryLineId;
        return BuildGameObject();
    }

    public LMBHDCFPPLL? RestoreGame()
    {
        return Data.GameActive ? BuildGameObject() : null;
    }

    public void GiveUpGame()
    {
        Data.GameActive = false;
        Data.CurTurn = 0;
    }

    public uint AdvanceTurn()
    {
        return ++Data.CurTurn;
    }

    #endregion

    #region Progression

    public void LearnSkill(uint skillId)
    {
        if (skillId != 0 && !Data.LearnedSkills.Contains(skillId))
            Data.LearnedSkills.Add(skillId);
    }

    public void SetSkillTrace(uint skillId)
    {
        if (skillId == 0) return;
        if (Data.TracedSkills.Contains(skillId))
            Data.TracedSkills.Remove(skillId);
        else
            Data.TracedSkills.Add(skillId);
    }

    public void MarkEndingViewed(uint endingId)
    {
        if (endingId != 0 && !Data.ViewedEndings.Contains(endingId))
            Data.ViewedEndings.Add(endingId);
    }

    public void ConfirmDailyPhase(uint phase)
    {
        if (phase > Data.DailyPhase) Data.DailyPhase = phase;
    }

    #endregion
}

using March7thHoney.Database.Activity;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Game.Activity.Activities.Heliobus;

/// <summary>
///     "Heliobus" event logic (Aha's lab raid + SNS social feed). Builds the activity-data
///     snapshot and tracks level/phase progression and the social-feed interactions.
/// </summary>
public class HeliobusInstance : BaseActivityInstance
{
    public HeliobusInstance(ActivityManager manager) : base(manager)
    {
        Data = manager.Data.HeliobusData;
    }

    public HeliobusData Data { get; }

    public HeliobusActivityDataScRsp BuildActivityDataSnapshot()
    {
        return new HeliobusActivityDataScRsp
        {
            Retcode = 0,
            Level = Data.Level,
            Phase = Data.Phase
        };
    }

    public uint UpgradeLevel()
    {
        return ++Data.Level;
    }

    public void MarkChallengeFinished(uint challengeId)
    {
        if (challengeId != 0 && !Data.FinishedChallenges.Contains(challengeId))
            Data.FinishedChallenges.Add(challengeId);
    }

    public uint CreatePost()
    {
        return ++Data.NextSnsId;
    }

    public void ReadPost(uint snsId)
    {
        if (snsId != 0 && !Data.ReadSnsIds.Contains(snsId))
            Data.ReadSnsIds.Add(snsId);
    }

    /// <summary>Toggles a like on the given SNS post; returns the resulting like state.</summary>
    public bool ToggleLike(uint snsId)
    {
        if (snsId == 0) return false;
        if (Data.LikedSnsIds.Contains(snsId))
        {
            Data.LikedSnsIds.Remove(snsId);
            return false;
        }

        Data.LikedSnsIds.Add(snsId);
        return true;
    }
}

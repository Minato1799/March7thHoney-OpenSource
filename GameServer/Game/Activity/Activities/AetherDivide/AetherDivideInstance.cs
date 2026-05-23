using March7thHoney.Database.Activity;
using March7thHoney.Proto;

namespace March7thHoney.GameServer.Game.Activity.Activities.AetherDivide;

/// <summary>
///     "Aetherium Wars" (AetherDivide) event logic. Builds the info snapshots and tracks
///     meta progression (level, spirit exp, passive skills, lineup, challenge rewards).
///     Battle resolution is acknowledged client-side; the server keeps progress consistent.
/// </summary>
public class AetherDivideInstance : BaseActivityInstance
{
    public AetherDivideInstance(ActivityManager manager) : base(manager)
    {
        Data = manager.Data.AetherDivideData;
    }

    public AetherDivideData Data { get; }

    public GetAetherDivideInfoScRsp BuildInfoSnapshot()
    {
        return new GetAetherDivideInfoScRsp { Retcode = 0 };
    }

    public GetAetherDivideChallengeInfoScRsp BuildChallengeInfoSnapshot()
    {
        return new GetAetherDivideChallengeInfoScRsp { Retcode = 0 };
    }

    public void SetLineup(IEnumerable<uint> avatarIds)
    {
        Data.Lineup.Clear();
        Data.Lineup.AddRange(avatarIds);
    }

    public void EquipPassiveSkill(uint slot, uint itemId)
    {
        if (itemId == 0) Data.PassiveSkillBySlot.Remove(slot);
        else Data.PassiveSkillBySlot[slot] = itemId;
    }

    public void ClearPassiveSkill(uint slot)
    {
        Data.PassiveSkillBySlot.Remove(slot);
    }

    public uint SpiritExpUp(uint exp)
    {
        Data.SpiritExp += exp;
        // Every 100 accumulated EXP grants a spirit level.
        while (Data.SpiritExp >= 100)
        {
            Data.SpiritExp -= 100;
            Data.Level++;
        }
        return Data.Level;
    }

    public bool TakeChallengeReward(uint challengeId)
    {
        if (challengeId == 0 || Data.TakenChallengeRewards.Contains(challengeId)) return false;
        Data.TakenChallengeRewards.Add(challengeId);
        return true;
    }

    public uint RefreshEndless()
    {
        return ++Data.EndlessRefreshCount;
    }
}

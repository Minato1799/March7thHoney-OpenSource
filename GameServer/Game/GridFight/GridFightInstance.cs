using March7thHoney.Data;
using March7thHoney.Data.Excel;
using March7thHoney.GameServer.Game.GridFight.Sync;
using March7thHoney.GameServer.Game.Battle;
using March7thHoney.GameServer.Game.GridFight.Battle;
using March7thHoney.GameServer.Game.GridFight.Component;
using March7thHoney.GameServer.Game.Player;
using March7thHoney.GameServer.Server.Packet.Send.GridFight;
using March7thHoney.Proto;
using March7thHoney.Enums.GridFight;
using March7thHoney.Util;

namespace March7thHoney.GameServer.Game.GridFight;

public class GridFightInstance(PlayerInstance player, uint season, uint divisionId, bool isOverLock, uint uniqueId)
{
    public sealed class RoleMergeResult
    {
        public List<uint> RemovedUniqueIds { get; } = new();
        public uint KeptUniqueId { get; set; }
        public uint NewStar { get; set; }
        public bool Merged => RemovedUniqueIds.Count > 0 && KeptUniqueId > 0;
    }
    public PlayerInstance Player { get; } = player;
    public uint Season { get; } = season;
    public uint DivisionId { get; } = divisionId;
    public bool IsOverLock { get; } = isOverLock;
    public uint UniqueId { get; } = uniqueId;

    
    public uint Gold { get; set; } = ConfigManager.Config.ServerOption.CurrencyWar.StartGold;
    public uint LineupHp { get; set; } = Math.Min(ConfigManager.Config.ServerOption.CurrencyWar.StartLineupHp, 100);
    public uint LineupMaxHp { get; } = 100;
    public uint BattleMaxHp { get; } = 10939;
    public uint Level { get; } = 1;
    public uint SectionId { get; set; } = 1;
    public uint NDOCIKPLKIF { get; } = 1801;
    public uint CurrentChapterId { get; set; } = 1;
    public uint CurrentBranchId { get; set; } = 1;
    public uint RouteId { get; set; } = 1200;
    public uint KeepWinCnt { get; set; }
    public int BattlesFinished { get; set; }
    public GridFightBattleComponent BattleComponent { get; } = new();

    
    public uint QueuePosition { get; set; } = 1;
    public GridFightPendingAction? PendingAction { get; set; }
    public uint NextRoleUniqueId { get; set; } = 100;
    public uint NextEquipUniqueId { get; set; } = 50;
    public uint NextOrbUniqueId { get; set; } = 10;

    
    public Dictionary<uint, uint> RoleByUniqueId { get; } = new();
    public Dictionary<uint, uint> UniqueIdByPos { get; } = new();
    public Dictionary<uint, uint> RoleStarByUniqueId { get; } = new();
    public Dictionary<uint, List<uint>> EquipUniqueIdsByRoleUniqueId { get; } = new();

    
    public uint ShopRefreshLeft { get; set; } = ConfigManager.Config.ServerOption.CurrencyWar.ShopRefreshPerSection;
    public uint ShopRollCounter { get; set; }
    public List<GridFightShopGoodsInfo> ShopGoods { get; } = new();
    public List<uint> ShopRolePool { get; } = new();

    
    public List<uint> StageCandidates { get; } = new() { 35030205, 35030405, 35030208, 350202, 35030606 };
    public List<uint> CurrentEquipDraft { get; private set; } = new() { 35030205u, 35030405u, 35030208u };
    public List<GridFightEquipmentInfo> Equipments { get; } = new();
    public List<GridFightConsumableInfo> Consumables { get; } = new();

    
    public List<uint> SectionAffixIds { get; private set; } = new();
    public List<uint> SessionCampIds { get; private set; } = new();
    public List<uint> SessionBossMonsterIds { get; private set; } = new();
    public List<uint> ActivePortalBuffIds { get; } = new();
    public List<uint> AvailablePortalBuffRerolls { get; } = new() { 105, 1014, 132 };
    public List<uint> CurrentPortalBuffOffer { get; private set; } = new();
    
    public List<uint> ActiveAugmentIds { get; } = new();
    
    public uint LastEncounterQuality { get; set; }
    public uint LastEncounterAppliedSection { get; set; }
    public uint LastEncounterAppliedChapter { get; set; }

    public List<uint> CurrentAugmentOffer { get; private set; } = new();
    public uint CurrentAugmentReroll { get; set; } = 3;
    public uint LastAugmentConsumedSection { get; set; }
    public List<(uint RoleId, uint EquipId)> CurrentSupplyOffer { get; private set; } = new();
    public uint CurrentSupplyReroll { get; set; } = 1;
    public uint LastSupplyConsumedSection { get; set; }

    public sealed class EliteBranchOption
    {
        public uint EncounterId;
        public uint StageId;
        public uint PenaltyRuleId;
        public List<uint> MonsterIds = new();
        public uint RewardItemId;
        public uint RewardCount;
        public uint DifficultyTier;
    }
    public List<EliteBranchOption> CurrentEliteBranchOptions { get; private set; } = new();
    public uint CurrentEliteBranchReroll { get; set; } = 1;
    public uint LastEliteBranchConsumedSection { get; set; }

    private static readonly uint[] FallbackPortalBuffPool =
    {
        101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115,
        116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 127, 129, 134, 135, 138,
        147, 1001, 1002, 1003, 1004, 1005, 1007, 1008, 1010, 1014, 1016,
        1101, 1102, 1104, 1106, 1107, 1108, 1112, 1113, 1114, 1115, 1116, 1118
    };

    public List<uint> RollPortalBuffs(int count = 3, IEnumerable<uint>? exclude = null)
    {
        var pool = (GameData.GridFightSeasonPortalData.TryGetValue(Season, out var seasonPool) && seasonPool.Count > 0)
            ? seasonPool.ToList()
            : FallbackPortalBuffPool.ToList();
        if (exclude != null)
            foreach (var id in exclude) pool.Remove(id);
        if (pool.Count <= count)
        {
            var fallback = pool.Distinct().ToList();
            while (fallback.Count < count)
                fallback.Add(FallbackPortalBuffPool[fallback.Count % FallbackPortalBuffPool.Length]);
            CurrentPortalBuffOffer = fallback.Take(count).ToList();
            return CurrentPortalBuffOffer;
        }
        var rng = Random.Shared;
        var picked = new List<uint>(count);
        while (picked.Count < count)
        {
            var idx = rng.Next(pool.Count);
            picked.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        CurrentPortalBuffOffer = picked;
        return CurrentPortalBuffOffer;
    }

    public List<uint> EnsurePortalBuffOffer()
    {
        if (CurrentPortalBuffOffer.Count == 0) RollPortalBuffs();
        return CurrentPortalBuffOffer;
    }

    public void ClearPortalBuffOffer() => CurrentPortalBuffOffer = new List<uint>();

    
    public List<(uint AvatarId, uint Pos, uint UniqueId, string Component)> RolledBenchRoles { get; private set; }
        = new();

    public void EnsureDefaultRoles()
    {
        
        if (ShopGoods.Count == 0) RotateShop();
        EnsureSessionPreview();
    }

    public void EnsureSessionPreview()
    {
        if (SessionCampIds.Count == 0) SessionCampIds = RollSessionCamps();
        if (SessionBossMonsterIds.Count == 0) SessionBossMonsterIds = ResolveSessionBossMonsterIds();
        if (SectionAffixIds.Count == 0) SectionAffixIds = RollSessionAffixes();
    }

    
    
    private List<uint> RollSessionCamps(int count = 3)
    {
        var pool = GameData.GridFightCampData.Values
            .Where(c => c.SeasonID == Season || c.SeasonID == 0)
            .Where(c => c.BossBattleArea > 0)
            .Where(c => c.Monsters.Any(m => m.MonsterTier >= 5))
            .Select(c => c.ID)
            .Distinct()
            .ToList();
        if (pool.Count == 0)
            pool = new List<uint> { 5, 8, 10, 16, 23, 25, 26, 27, 28 };
        return PickDistinct(pool, count);
    }

    
    private List<uint> ResolveSessionBossMonsterIds()
    {
        var bosses = new List<uint>(SessionCampIds.Count);
        foreach (var campId in SessionCampIds)
        {
            if (!GameData.GridFightCampData.TryGetValue(campId, out var camp))
            {
                bosses.Add(0);
                continue;
            }
            var boss = camp.Monsters
                .Where(m => m.MonsterTier >= 5)
                .OrderBy(m => m.MonsterTier)
                .ThenBy(m => m.MonsterID)
                .FirstOrDefault();
            bosses.Add(boss?.MonsterID ?? 0u);
        }
        return bosses;
    }

    public uint GetCurrentBossMonsterId()
    {
        EnsureSessionPreview();
        var idx = (int)CurrentChapterId - 1;
        if (idx < 0 || idx >= SessionBossMonsterIds.Count) return 0;
        return SessionBossMonsterIds[idx];
    }

    private List<uint> RollSessionAffixes()
    {
        var totalCount = 3;
        if (GameData.GridFightDivisionStageData.TryGetValue(DivisionId, out var stage)
            && stage.AffixChooseNumList.Count > 0)
        {
            totalCount = (int)stage.AffixChooseNumList.Sum(x => x);
        }
        if (totalCount <= 0) return new List<uint>();
        var pool = GameData.GridFightAffixConfigData.Keys.ToList();
        if (pool.Count == 0)
            pool = new List<uint> { 1001, 1002, 1003, 1004, 1005, 2002, 2003, 2004, 2005, 2006, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 4001, 4002, 4003, 4005, 4006, 4007, 4008, 4009, 4010, 4011, 4012, 4013 };
        return PickDistinct(pool, totalCount);
    }

    private static List<uint> PickDistinct(List<uint> pool, int count)
    {
        var rng = Random.Shared;
        var copy = pool.ToList();
        var picked = new List<uint>(count);
        while (picked.Count < count && copy.Count > 0)
        {
            var idx = rng.Next(copy.Count);
            picked.Add(copy[idx]);
            copy.RemoveAt(idx);
        }
        while (picked.Count < count) picked.Add(pool[picked.Count % Math.Max(1, pool.Count)]);
        return picked;
    }

    
    
    public void MaterializeInitialBenchTeam(uint requiredTrait = 0)
    {
        if (RoleByUniqueId.Count > 0) return;
        if (RolledBenchRoles.Count == 0
            || (requiredTrait > 0 && !RolledBenchRoles.Any(r => RoleHasTrait(r.AvatarId, requiredTrait))))
        {
            RollInitialBenchRoles(requiredTrait: requiredTrait);
        }
        foreach (var (avatarId, pos, uniqueId, _) in RolledBenchRoles)
        {
            RoleByUniqueId[uniqueId] = avatarId;
            RoleStarByUniqueId[uniqueId] = 1;
            UniqueIdByPos[pos] = uniqueId;
        }
    }

    private static bool RoleHasTrait(uint avatarId, uint trait) =>
        GameData.GridFightRoleBasicInfoData.Values
            .Any(r => r.AvatarID == avatarId && r.TraitList.Contains(trait));

    public List<(uint AvatarId, uint Pos, uint UniqueId, string Component)> RollInitialBenchRoles(int count = 4, uint requiredTrait = 0)
    {
        var pool = GameData.GridFightRoleBasicInfoData.Values
            .Where(r => r.IsInPool && (r.SeasonID == Season || r.SeasonID == 0))
            .Where(r => r.AvatarID >= 1000 && r.AvatarID < 2000)
            .Where(r => r.RoleSavedValueList.Count > 0)
            .ToList();

        var rng = Random.Shared;
        var copy = pool.ToList();
        var picked = new List<GridFightRoleBasicInfoExcel>(count);

        
        if (requiredTrait > 0)
        {
            var traitMatched = copy.Where(r => r.TraitList.Contains(requiredTrait)).ToList();
            if (traitMatched.Count > 0)
            {
                var pivot = traitMatched[rng.Next(traitMatched.Count)];
                picked.Add(pivot);
                copy.Remove(pivot);
            }
        }

        while (picked.Count < count && copy.Count > 0)
        {
            var idx = rng.Next(copy.Count);
            picked.Add(copy[idx]);
            copy.RemoveAt(idx);
        }

        RolledBenchRoles = new List<(uint, uint, uint, string)>(picked.Count);
        for (var i = 0; i < picked.Count; i++)
        {
            var role = picked[i];
            RolledBenchRoles.Add((role.AvatarID, (uint)(14 + i), AllocRoleUniqueId(),
                role.RoleSavedValueList[0]));
        }
        return RolledBenchRoles;
    }


    public uint AllocRoleUniqueId() => ++NextRoleUniqueId;
    public uint AllocEquipUniqueId() => ++NextEquipUniqueId;
    public uint AllocOrbUniqueId() => ++NextOrbUniqueId;

    public GridFightEquipmentInfo? FindEquipment(uint uniqueId)
        => Equipments.FirstOrDefault(e => e.UniqueId == uniqueId);

    public void RemoveEquipmentByUniqueId(uint uniqueId)
    {
        Equipments.RemoveAll(e => e.UniqueId == uniqueId);
        foreach (var kv in EquipUniqueIdsByRoleUniqueId)
            kv.Value.RemoveAll(x => x == uniqueId);
    }

    public List<uint> UnequipAllFromRole(uint roleUid)
    {
        if (!EquipUniqueIdsByRoleUniqueId.TryGetValue(roleUid, out var list)) return new List<uint>();
        var snapshot = list.ToList();
        list.Clear();
        return snapshot;
    }

    public GridFightEquipmentInfo AddEquipment(uint equipmentId, uint source = 1)
    {
        var info = new GridFightEquipmentInfo
        {
            GridFightEquipmentId = equipmentId,
            Source = source,
            UniqueId = AllocEquipUniqueId()
        };
        Equipments.Add(info);
        return info;
    }

    
    public GridFightEquipmentInfo? RollEquipment(uint oldUniqueId, uint newEquipId, uint source = 1)
    {
        var old = FindEquipment(oldUniqueId);
        if (old == null) return null;
        uint? wearer = null;
        foreach (var kv in EquipUniqueIdsByRoleUniqueId)
        {
            if (kv.Value.Contains(oldUniqueId)) { wearer = kv.Key; break; }
        }
        RemoveEquipmentByUniqueId(oldUniqueId);
        var added = AddEquipment(newEquipId, source);
        if (wearer.HasValue)
        {
            if (!EquipUniqueIdsByRoleUniqueId.TryGetValue(wearer.Value, out var list))
                EquipUniqueIdsByRoleUniqueId[wearer.Value] = list = new List<uint>();
            list.Add(added.UniqueId);
        }
        return added;
    }

    public bool TryConsumeConsumable(uint itemId, int amount = 1)
    {
        for (var i = 0; i < amount; i++)
        {
            var entry = Consumables.FirstOrDefault(c => c.ItemId == itemId);
            if (entry == null) return false;
            if (entry.Num <= 1) Consumables.Remove(entry);
            else entry.Num -= 1;
        }
        return true;
    }

    
    public static uint RollSameCategoryEquipment(uint currentEquipId)
    {
        if (!GameData.GridFightEquipmentData.TryGetValue(currentEquipId, out var current)) return 0;
        var category = current.EquipCategory;
        var pool = GameData.GridFightEquipmentData.Values
            .Where(e => e.EquipCategory == category && e.ID != currentEquipId)
            .Select(e => e.ID)
            .ToList();
        if (pool.Count == 0) return 0;
        return pool[Random.Shared.Next(pool.Count)];
    }

    
    public uint PreBattleLineupHp { get; set; }
    public uint PreBattleLevel { get; set; }
    public uint PreBattleExp { get; set; }
    public uint PlayerLevel { get; set; } = 3;
    public uint PlayerExp { get; set; }
    public uint PlayerMaxLevel { get; } = 10;
    public uint CampId { get; set; } = 18;
    public uint PreBattleGold { get; set; }
    public Proto.GridFightDamageSttInfo? LastBattleDamageStt { get; set; }
    public uint LastBattleIDEAAPCCFPF { get; set; }
    public Proto.GridFightSettleReason LastSettleReason { get; set; } = Proto.GridFightSettleReason.CdphdhnlhaoFmpbhelfgee;
    public List<(uint ItemId, uint UniqueId)> LastRewardedOrbs { get; } = new();
    public Dictionary<uint, uint> OrbItemByUniqueId { get; } = new() { [7u] = 199u };
    public List<Proto.GridFightHpModifyInfo> LastHpModifyTimeline { get; } = new();
    public Dictionary<uint, Proto.GridFightDropInfo> LastSectionRewards { get; } = new();

    public void AdvanceQueue(uint by = 1)
    {
        QueuePosition += by;
        PendingAction = null;
    }

    public void ApplyPositionList(IEnumerable<GridFightPosInfo> posInfoList)
    {
        EnsureDefaultRoles();
        foreach (var posInfo in posInfoList)
        {
            if (posInfo.Pos == 0) continue;
            foreach (var stale in UniqueIdByPos
                         .Where(kv => kv.Key != posInfo.Pos && kv.Value == posInfo.UniqueId)
                         .Select(kv => kv.Key).ToList())
                UniqueIdByPos.Remove(stale);
            UniqueIdByPos[posInfo.Pos] = posInfo.UniqueId;
        }
    }

    public List<(uint RoleId, uint Pos)> ResolveForegroundRoles()
    {
        EnsureDefaultRoles();
        return UniqueIdByPos.OrderBy(kv => kv.Key)
            .Where(kv => kv.Key is > 0 and <= 4)
            .Select(kv => (Role: RoleByUniqueId.GetValueOrDefault(kv.Value), Pos: kv.Key))
            .Where(t => t.Role != 0)
            .Select(t => (RoleId: t.Role, t.Pos))
            .ToList();
    }

    public List<(uint RoleId, uint Pos)> ResolveBackgroundRoles()
    {
        EnsureDefaultRoles();
        return UniqueIdByPos.OrderBy(kv => kv.Key)
            .Where(kv => kv.Key is > 4 and <= 13)
            .Select(kv => (Role: RoleByUniqueId.GetValueOrDefault(kv.Value), Pos: kv.Key))
            .Where(t => t.Role != 0)
            .Select(t => (RoleId: t.Role, t.Pos))
            .ToList();
    }

    public List<int> BuildForegroundAvatarIds(int maxCount = 4)
    {
        var list = ResolveForegroundRoles().Select(t => RoleIdToAvatarId(t.RoleId)).Where(id => id != 0).Take(maxCount).ToList();
        if (list.Count == 0)
            list = RoleByUniqueId.Values.Take(maxCount).Select(x => (int)x).ToList();
        return list;
    }

    public List<int> BuildBackgroundAvatarIds()
    {
        return ResolveBackgroundRoles().Select(t => RoleIdToAvatarId(t.RoleId)).Where(id => id != 0).ToList();
    }

    private static int RoleIdToAvatarId(uint roleId) =>
        GameData.GridFightRoleBasicInfoData.TryGetValue(roleId, out var e) ? (int)e.AvatarID : (int)roleId;

    public List<IENNMHMOONM> CheckTrait()
    {
        EnsureDefaultRoles();
        var counts = new Dictionary<uint, uint>();
        var memberMap = new Dictionary<uint, List<(uint AvatarId, uint UniqueId)>>();

        foreach (var kv in UniqueIdByPos.Where(kv => kv.Key is > 0 and <= 13))
        {
            var uniqueId = kv.Value;
            if (uniqueId == 0 || !RoleByUniqueId.TryGetValue(uniqueId, out var roleId)) continue;
            if (!GameData.GridFightRoleBasicInfoData.TryGetValue(roleId, out var role)) continue;
            var roleTraits = new HashSet<uint>(role.TraitList);
            if (EquipUniqueIdsByRoleUniqueId.TryGetValue(uniqueId, out var dressed))
            {
                foreach (var equipUid in dressed)
                {
                    var equip = Equipments.FirstOrDefault(e => e.UniqueId == equipUid);
                    if (equip == null) continue;
                    if (!GameData.GridFightEquipmentData.TryGetValue(equip.GridFightEquipmentId, out var equipExcel)) continue;
                    if (equipExcel.EquipFunc != Enums.GridFight.GridFightEquipFuncTypeEnum.OriginEmblem
                        && equipExcel.EquipFunc != Enums.GridFight.GridFightEquipFuncTypeEnum.ClassEmblem) continue;
                    if (equipExcel.EquipFuncParamList.Count == 0) continue;
                    roleTraits.Add(equipExcel.EquipFuncParamList[0]);
                }
            }
            foreach (var tid in roleTraits)
            {
                counts[tid] = counts.GetValueOrDefault(tid) + 1;
                if (!memberMap.TryGetValue(tid, out var list))
                    memberMap[tid] = list = new List<(uint, uint)>();
                list.Add((role.AvatarID, uniqueId));
            }
        }

        var result = new List<IENNMHMOONM>();
        foreach (var (tid, count) in counts)
        {
            if (count == 0) continue;
            if (!GameData.GridFightTraitBasicInfoData.TryGetValue(tid, out var traitExcel)) continue;

            uint layer = 0;
            if (GameData.GridFightTraitLayerData.TryGetValue(tid, out var layerMap) && layerMap.Count > 0)
            {
                foreach (var (threshold, excel) in layerMap)
                    if (threshold <= count && excel.Layer > layer)
                        layer = excel.Layer;
            }

            var trait = new IENNMHMOONM { TraitId = tid, NKFDBEHPNLG = layer };
            if (memberMap.TryGetValue(tid, out var members))
            {
                foreach (var (avatarId, uniqueId) in members)
                {
                    trait.MEEPFKLLIJB.Add(new BGNGLHHBGMI
                    {
                        EIHHLAOKAPH = GAPBBJCLMGP.Dgecgaafdjm,
                        GDNIKJGAEBH = avatarId,
                        IPDCMHIEKIJ = uniqueId,
                        GridUpdateSrc = PFODGDGFBBN.Iomeeecoiob
                    });
                }
            }
            if (layer > 0)
            {
                foreach (var effectId in traitExcel.TraitEffectList)
                {
                    var effectInfo = new BattleGridFightTraitEffectInfo { EffectId = effectId };
                    AttachTraitEffectLevelInfo(effectInfo, effectId);
                    trait.TraitEffectList.Add(effectInfo);
                }
            }
            result.Add(trait);
        }
        return result;
    }

    private static void AttachTraitEffectLevelInfo(BattleGridFightTraitEffectInfo effectInfo, uint effectId)
    {
        if (!GameData.GridFightTraitEffectData.TryGetValue(effectId, out var effectExcel)) return;
        if (effectExcel.TraitEffectType != GridFightTraitEffectTypeEnum.TraitBonus) return;
        if (!GameData.GridFightTraitBonusData.TryGetValue(effectId, out var thresholdMap)) return;

        var levelInfo = new GridFightTraitEffectLevelInfo();
        foreach (var (threshold, bonusExcel) in thresholdMap)
        {
            var drop = new GridFightDropInfo();
            foreach (var combinationId in bonusExcel.BonusParamList)
            {
                if (!GameData.GridFightCombinationBonusData.TryGetValue(combinationId, out var combination)) continue;
                foreach (var poolId in combination.CombinationBonusList)
                {
                    if (!GameData.GridFightBasicBonusPoolV2Data.TryGetValue(poolId, out var pool)) continue;
                    var itemId = pool.BonusTypeParamList.Count > 0 ? pool.BonusTypeParamList[0] : pool.BonusTypeParam;
                    if (itemId == 0) continue;
                    drop.PIBLJLBCKJL.Add(new LHPPIAKKFME
                    {
                        BGKDAMDFFKH = GridFightDropType.HiolcnpoponMkppcdpchie,
                        JJFFLMCCCMM = itemId,
                        Num = 1
                    });
                }
            }
            if (drop.PIBLJLBCKJL.Count > 0)
                levelInfo.TraitEffectLevelReward[threshold] = drop;
        }
        if (levelInfo.TraitEffectLevelReward.Count > 0)
            effectInfo.TraitEffectLevelInfo = levelInfo;
    }

    public void RotateShop()
    {
        ShopGoods.Clear();
        ShopRollCounter++;

        var rng = Random.Shared;
        var rarityWeights = GetShopRarityWeights();
        var rolePoolByRarity = BuildShopRolePoolByRarity();

        for (var i = 0; i < 5; i++)
        {
            var rarity = RollShopRarity(rarityWeights, rng);
            if (!rolePoolByRarity.TryGetValue(rarity, out var pool) || pool.Count == 0)
            {
                pool = rolePoolByRarity.Values.FirstOrDefault(x => x.Count > 0) ?? [];
            }
            if (pool.Count == 0) break;

            var roleId = pool[rng.Next(pool.Count)];
            var price = GetShopGoodsPrice(rarity, 1);
            ShopGoods.Add(new GridFightShopGoodsInfo
            {
                ShopGoodsPrice = price,
                RoleGoodsInfo = new GridFightRoleGoodsInfo { RoleId = roleId, RoleStar = 1 }
            });
        }
    }

    private List<uint> GetShopRarityWeights()
    {
        if (GameData.GridFightPlayerLevelData.TryGetValue(PlayerLevel, out var conf)
            && conf.RarityWeights.Count >= 5)
            return conf.RarityWeights;
        return [100, 0, 0, 0, 0];
    }

    private Dictionary<uint, List<uint>> BuildShopRolePoolByRarity()
    {
        var dict = new Dictionary<uint, List<uint>>();
        foreach (var role in GameData.GridFightRoleBasicInfoData.Values)
        {
            if (!role.IsInPool) continue;
            if (role.SeasonID != 0 && role.SeasonID != Season) continue;
            if (role.AvatarID < 1000 || role.AvatarID >= 2000) continue;
            if (!dict.TryGetValue(role.Rarity, out var list))
                dict[role.Rarity] = list = [];
            list.Add(role.ID);
        }
        return dict;
    }

    private static uint RollShopRarity(IReadOnlyList<uint> weights, Random rng)
    {
        var total = 0u;
        for (var i = 0; i < Math.Min(5, weights.Count); i++) total += weights[i];
        if (total == 0) return 1;

        var roll = (uint)rng.Next(1, (int)total + 1);
        for (var i = 0; i < Math.Min(5, weights.Count); i++)
        {
            if (roll <= weights[i]) return (uint)(i + 1);
            roll -= weights[i];
        }
        return 1;
    }

    private static uint GetShopGoodsPrice(uint rarity, uint star)
    {
        if (!GameData.GridFightShopPriceData.TryGetValue(rarity, out var priceConf))
            return 1;
        var idx = (int)Math.Clamp(star, 1u, 4u) - 1;
        return priceConf.BuyGoldList[idx];
    }

    public void RefreshEquipDraft()
    {
        var startIdx = (BattlesFinished * 3) % StageCandidates.Count;
        CurrentEquipDraft = new List<uint>
        {
            StageCandidates[startIdx % StageCandidates.Count],
            StageCandidates[(startIdx + 1) % StageCandidates.Count],
            StageCandidates[(startIdx + 2) % StageCandidates.Count]
        };
    }

    public (bool ok, uint addedUniqueId, uint roleId, uint pos) TryBuyGoods(int shopIndex)
    {
        if (shopIndex < 0 || shopIndex >= ShopGoods.Count) return (false, 0, 0, 0);
        var goods = ShopGoods[shopIndex];
        if (goods.RoleGoodsInfo == null || goods.IsSoldOut) return (false, 0, 0, 0);
        var price = goods.ShopGoodsPrice;
        if (Gold < price) return (false, 0, 0, 0); // reject instead of granting a free buy
        Gold -= price;
        goods.IsSoldOut = true;
        var roleId = goods.RoleGoodsInfo.RoleId;
        var uniqueId = AllocRoleUniqueId();
        RoleByUniqueId[uniqueId] = roleId;
        RoleStarByUniqueId[uniqueId] = 1;
        
        uint pos = 0;
        foreach (var candidate in new uint[] { 14, 15, 16, 17, 18, 6, 7, 8, 9, 10, 11, 12, 13 })
        {
            if (!UniqueIdByPos.ContainsKey(candidate))
            {
                pos = candidate;
                break;
            }
        }
        if (pos > 0) UniqueIdByPos[pos] = uniqueId;
        return (true, uniqueId, roleId, pos);
    }

    public RoleMergeResult TryAutoMergeRole(uint roleId, uint roleUniqueId)
    {
        var result = new RoleMergeResult();
        if (roleId == 0 || roleUniqueId == 0) return result;

        var keepUniqueId = roleUniqueId;
        var currentStar = RoleStarByUniqueId.GetValueOrDefault(keepUniqueId, 1u);
        while (true)
        {
            var nextStar = currentStar + 1;
            if (!GameData.GridFightRoleStarData.ContainsKey(roleId << 4 | nextStar))
                break;

            var sameTier = RoleByUniqueId
                .Where(kv => kv.Value == roleId && RoleStarByUniqueId.GetValueOrDefault(kv.Key, 1u) == currentStar)
                .Select(kv => kv.Key)
                .Distinct()
                .ToList();
            if (sameTier.Count < 3) break;

            if (!sameTier.Contains(keepUniqueId))
                keepUniqueId = sameTier[0];

            var consume = sameTier.Where(x => x != keepUniqueId).Take(2).ToList();
            if (consume.Count < 2) break;

            foreach (var uid in consume)
            {
                RoleByUniqueId.Remove(uid);
                RoleStarByUniqueId.Remove(uid);
                foreach (var pos in UniqueIdByPos.Where(kv => kv.Value == uid).Select(kv => kv.Key).ToList())
                    UniqueIdByPos.Remove(pos);
                result.RemovedUniqueIds.Add(uid);
            }

            currentStar = nextStar;
            RoleStarByUniqueId[keepUniqueId] = currentStar;
        }

        if (result.RemovedUniqueIds.Count > 0)
        {
            result.KeptUniqueId = keepUniqueId;
            result.NewStar = RoleStarByUniqueId.GetValueOrDefault(keepUniqueId, 1u);
        }
        return result;
    }

    public (bool ok, uint refund) TryRecycleRole(uint uniqueId)
    {
        if (!RoleByUniqueId.TryGetValue(uniqueId, out var roleId)) return (false, 0);
        RoleByUniqueId.Remove(uniqueId);
        var roleStar = RoleStarByUniqueId.GetValueOrDefault(uniqueId, 1u);
        RoleStarByUniqueId.Remove(uniqueId);
        foreach (var pos in UniqueIdByPos.Where(kv => kv.Value == uniqueId).Select(kv => kv.Key).ToList())
            UniqueIdByPos.Remove(pos);
        var refund = GetRoleSellPrice(roleId, roleStar);
        Gold += refund;
        return (true, refund);
    }

    private static uint GetRoleSellPrice(uint roleId, uint roleStar)
    {
        if (!GameData.GridFightRoleBasicInfoData.TryGetValue(roleId, out var role))
            return 1;
        if (!GameData.GridFightShopPriceData.TryGetValue(role.Rarity, out var priceConf))
            return 1;

        var idx = (int)Math.Clamp(roleStar, 1u, 4u) - 1;
        return priceConf.SellGoldList[idx];
    }

    public bool TryRefreshShop()
    {
        
        var cost = ShopRefreshLeft == 0 ? 2u : ShopRefreshLeft;
        Gold = Gold < cost ? 0 : Gold - cost;
        RotateShop();
        return true;
    }

    public bool TryBuyExp()
    {
        var cost = GetBuyExpCost();
        if (cost == 0) cost = 4;

        Gold = Gold < cost ? 0 : Gold - cost;
        AddPlayerExp(cost);
        return true;
    }

    public uint GetBuyExpCost()
    {
        if (GameData.GridFightPlayerLevelData.TryGetValue(PlayerLevel, out var conf))
        {
            if (conf.LevelUpExp == 0) return 0; 
        }
        return 4;
    }

    public uint GetCurrentMaxBattleRoleNum()
    {
        if (GameData.GridFightPlayerLevelData.TryGetValue(PlayerLevel, out var conf))
            return Math.Clamp(conf.AvatarMaxNumber, 0u, 13u);
        return 4;
    }

    private void AddPlayerExp(uint value)
    {
        if (value == 0) return;

        PlayerExp += value;
        while (PlayerLevel < GetConfiguredMaxPlayerLevel())
        {
            var need = GetLevelUpExpNeed(PlayerLevel);
            if (need == 0 || PlayerExp < need) break;
            PlayerExp -= need;
            PlayerLevel++;
        }
    }

    public void GainPlayerExp(uint value) => AddPlayerExp(value);

    public sealed class OrbUseResult
    {
        public uint UniqueId;
        public uint ItemId;
        public uint GoldAfter; 
        public bool GoldChanged;
        public bool LevelChanged;
        public uint? AddRoleId;
        public uint? AddRoleUniqueId;
        public uint? AddRolePos;
        public string? AddRoleComponent;
        public uint? AddEquipmentId;
        public uint? AddEquipmentUniqueId;
        public uint? AddConsumableItemId;
        public uint AddConsumableNewTotal;
    }

    private static readonly uint[] OrbEquipmentPool =
    {
        350201, 350202, 350203, 350204, 350205, 350206, 350207, 350208
    };

    private static readonly uint[] OrbConsumablePool = { 350101, 350103 };

    public List<OrbUseResult> TryUseOrbsDetailed(IEnumerable<uint> uniqueIds, bool isGetAll)
    {
        var targets = isGetAll ? OrbItemByUniqueId.Keys.ToList() : uniqueIds.Distinct().ToList();
        var results = new List<OrbUseResult>();
        var rng = Random.Shared;

        foreach (var uniqueId in targets)
        {
            if (!OrbItemByUniqueId.TryGetValue(uniqueId, out var orbItemId)) continue;
            OrbItemByUniqueId.Remove(uniqueId);

            var r = new OrbUseResult { UniqueId = uniqueId, ItemId = orbItemId };

            
            if (TryResolveRoleGrantOrb(orbItemId, out var roleId))
            {
                ApplyRoleGrant(r, roleId);
                results.Add(r);
                continue;
            }

            
            
            
            
            
            
            switch (orbItemId)
            {
                case 199u:
                    ApplyGold(r, 2);
                    break;
                case 102u:
                case 103u:
                    ApplyGold(r, 1);
                    ApplyConsumable(r, orbItemId == 102u ? 350101u : 350103u);
                    break;
                case 203u:
                    ApplyEquipment(r, OrbEquipmentPool[rng.Next(OrbEquipmentPool.Length)]);
                    break;
                case 204u:
                    ApplyGold(r, 3);
                    break;
                default:
                    ApplyTypeFallback(r, orbItemId, rng);
                    break;
            }

            results.Add(r);
        }

        return results;
    }

    private static bool TryResolveRoleGrantOrb(uint orbItemId, out uint roleId)
    {
        roleId = 0;
        
        if (orbItemId < 100000u) return false;
        var tail = orbItemId % 1000u;
        if (tail < 1u || tail > 999u) return false;
        
        var candidate = 1000u + tail;
        if (!GameData.GridFightRoleBasicInfoData.ContainsKey(candidate)) return false;
        roleId = candidate;
        return true;
    }

    private void ApplyGold(OrbUseResult r, uint amount)
    {
        Gold += amount;
        r.GoldAfter = Gold;
        r.GoldChanged = true;
    }

    private void ApplyExp(OrbUseResult r, uint amount)
    {
        var oldLevel = PlayerLevel;
        var oldExp = PlayerExp;
        AddPlayerExp(amount);
        if (PlayerLevel != oldLevel || PlayerExp != oldExp) r.LevelChanged = true;
    }

    private void ApplyConsumable(OrbUseResult r, uint itemId)
    {
        var entry = Consumables.FirstOrDefault(c => c.ItemId == itemId);
        if (entry == null)
        {
            entry = new GridFightConsumableInfo { ItemId = itemId, Num = 1 };
            Consumables.Add(entry);
        }
        else entry.Num += 1;
        r.AddConsumableItemId = itemId;
        r.AddConsumableNewTotal = entry.Num;
    }

    private void ApplyEquipment(OrbUseResult r, uint equipId)
    {
        var uid = AllocEquipUniqueId();
        Equipments.Add(new GridFightEquipmentInfo
        {
            GridFightEquipmentId = equipId,
            Source = 1,
            UniqueId = uid
        });
        r.AddEquipmentId = equipId;
        r.AddEquipmentUniqueId = uid;
    }

    private void ApplyRoleGrant(OrbUseResult r, uint roleId)
    {
        if (!GameData.GridFightRoleBasicInfoData.TryGetValue(roleId, out var roleExcel)) return;
        uint pos = 0;
        foreach (var candidate in new uint[] { 14, 15, 16, 17, 18, 19, 6, 7, 8, 9, 10, 11, 12, 13 })
        {
            if (!UniqueIdByPos.ContainsKey(candidate)) { pos = candidate; break; }
        }
        if (pos == 0) return;
        var uid = AllocRoleUniqueId();
        RoleByUniqueId[uid] = roleExcel.AvatarID;
        RoleStarByUniqueId[uid] = 1;
        UniqueIdByPos[pos] = uid;
        r.AddRoleId = roleExcel.AvatarID;
        r.AddRoleUniqueId = uid;
        r.AddRolePos = pos;
        if (roleExcel.RoleSavedValueList.Count > 0)
            r.AddRoleComponent = roleExcel.RoleSavedValueList[0];
    }

    private void ApplyTypeFallback(OrbUseResult r, uint orbItemId, Random rng)
    {
        if (!GameData.GridFightOrbData.TryGetValue(orbItemId, out var orbExcel))
        {
            ApplyGold(r, 1);
            return;
        }
        switch (orbExcel.Type)
        {
            case GridFightOrbTypeEnum.White: ApplyGold(r, 2); break;
            case GridFightOrbTypeEnum.Blue: ApplyExp(r, 2); break;
            case GridFightOrbTypeEnum.Glod:
                if (rng.Next(2) == 0) ApplyEquipment(r, OrbEquipmentPool[rng.Next(OrbEquipmentPool.Length)]);
                else ApplyGold(r, 5);
                break;
            case GridFightOrbTypeEnum.Colorful: ApplyExp(r, 4); break;
            case GridFightOrbTypeEnum.BigColorful: ApplyExp(r, 8); break;
            case GridFightOrbTypeEnum.GoldenEgg: ApplyGold(r, 8); break;
            default: ApplyGold(r, 1); break;
        }
    }

    public uint GetLevelUpExpNeed(uint level)
    {
        if (GameData.GridFightPlayerLevelData.TryGetValue(level, out var conf))
            return conf.LevelUpExp;
        return 4;
    }

    public uint GetConfiguredMaxPlayerLevel()
    {
        if (GameData.GridFightPlayerLevelData.Count > 0)
            return GameData.GridFightPlayerLevelData.Keys.Max();
        return PlayerMaxLevel;
    }

    public uint GetEnemyDifficultyLevel()
    {
        if (GameData.GridFightDivisionStageData.TryGetValue(DivisionId, out var stage)
            && stage.EnemyDifficultyLevel > 0)
            return stage.EnemyDifficultyLevel;
        return 1;
    }

    public uint GetDivisionLevel()
    {
        if (GameData.GridFightDivisionInfoData.TryGetValue(DivisionId, out var info) && info.DivisionLevel > 0)
            return info.DivisionLevel;
        
        return Math.Clamp((DivisionId - 10000u) / 100u, 1u, 9u);
    }

    
    
    public uint GetActiveAugmentDifficultyAdd()
    {
        var divLv = GetDivisionLevel();
        if (!GameData.GridFightAugmentMonsterData.TryGetValue(divLv, out var byQuality)) return 0;
        uint sum = 0;
        foreach (var augId in ActiveAugmentIds)
        {
            if (!GameData.GridFightAugmentData.TryGetValue(augId, out var aug)) continue;
            if (byQuality.TryGetValue(aug.Quality, out var entry)) sum += entry.EnemyDiffLvAdd;
        }
        return sum;
    }

    
    public uint GetEncounterDifficultyAdd()
    {
        if (LastEncounterQuality == 0
            || LastEncounterAppliedSection != SectionId
            || LastEncounterAppliedChapter != CurrentChapterId) return 0;
        if (!GameData.GridFightDivisionStageData.TryGetValue(DivisionId, out var stage)) return 0;
        var rule = stage.BinaryNodeDiffAddRule;
        if (rule == 0) return 0;
        if (!GameData.GridFightBinaryDiffAddRuleData.TryGetValue(rule, out var byQuality)) return 0;
        return byQuality.TryGetValue(LastEncounterQuality, out var entry) ? entry.EnemyDifficultyAddValue : 0u;
    }

    
    public uint GetEffectiveEnemyDifficultyLevel()
        => GetEnemyDifficultyLevel() + GetActiveAugmentDifficultyAdd() + GetEncounterDifficultyAdd();

    
    
    
    
    public uint ResolveEliteGroupForCurrentSection()
    {
        var route = Battle.GridFightLevelResolver.ResolveRoute(this);
        if (route == null) return 1816;
        if (route.NodeType == Enums.GridFight.GridFightNodeTypeEnum.Monster)
            return ResolveRewardEliteGroup(CurrentChapterId, SectionId);
        if (route.NodeType == Enums.GridFight.GridFightNodeTypeEnum.Supply)
            return 0;
        
        var n = CountCombatNodesUpToCurrent();
        return 1800u + Math.Max(1u, n);
    }

    private static uint ResolveRewardEliteGroup(uint chapter, uint section)
    {
        
        if (chapter == 1 && section <= 2) return 1816;
        if (chapter == 1) return 1817;     
        return 1816u + chapter;            
    }

    private uint CountCombatNodesUpToCurrent()
    {
        if (!GameData.GridFightStageRouteData.TryGetValue(RouteId, out var bucket)) return 1;
        uint count = 0;
        foreach (var r in bucket.Values.OrderBy(r => r.ChapterID).ThenBy(r => r.SectionID))
        {
            if (r.NodeType == Enums.GridFight.GridFightNodeTypeEnum.Supply) continue;
            if (r.NodeType == Enums.GridFight.GridFightNodeTypeEnum.Monster) continue; 
            count++;
            if (r.ChapterID == CurrentChapterId && r.SectionID == SectionId) return count;
        }
        return count;
    }

    public bool TryEquipFromDraft(uint idx)
    {
        if (idx >= CurrentEquipDraft.Count) return false;
        var equipId = CurrentEquipDraft[(int)idx];
        Equipments.Add(new GridFightEquipmentInfo
        {
            GridFightEquipmentId = equipId,
            Source = 1,
            UniqueId = AllocEquipUniqueId()
        });
        return true;
    }

    public void OnBattleResolved(bool win)
    {
        BattlesFinished++;
        if (win)
        {
            var nextSection = SectionId + 1;
            var maxSection = GetChapterSectionCount(CurrentChapterId);
            if (maxSection > 0 && nextSection > maxSection)
            {
                var nextChapter = CurrentChapterId + 1;
                if (GetChapterSectionCount(nextChapter) > 0)
                {
                    CurrentChapterId = nextChapter;
                    SectionId = 1;
                    LastAugmentConsumedSection = 0;
                    LastSupplyConsumedSection = 0;
                    LastEliteBranchConsumedSection = 0;
                    ClearAugmentOffer();
                    ClearSupplyOffer();
                    ClearEliteBranchOptions();
                    CurrentAugmentReroll = 3;
                    CurrentSupplyReroll = 1;
                    CurrentEliteBranchReroll = 1;
                }
            }
            else
            {
                SectionId = nextSection;
            }
        }
        ShopRefreshLeft = 2;
        RotateShop();
        RefreshEquipDraft();
    }

    public uint GetChapterSectionCount(uint chapter)
    {
        if (!GameData.GridFightStageRouteData.TryGetValue(RouteId, out var bucket)) return 0;
        var max = 0u;
        foreach (var route in bucket.Values)
        {
            if (route.ChapterID == chapter && route.SectionID > max) max = route.SectionID;
        }
        return max;
    }

    public uint GetMaxChapterId()
    {
        if (!GameData.GridFightStageRouteData.TryGetValue(RouteId, out var bucket)) return 0;
        var max = 0u;
        foreach (var route in bucket.Values)
            if (route.ChapterID > max) max = route.ChapterID;
        return max;
    }

    public bool IsFinalSection(uint chapter, uint section)
    {
        var maxChapter = GetMaxChapterId();
        if (maxChapter == 0 || chapter != maxChapter) return false;
        return section == GetChapterSectionCount(chapter);
    }

    private static readonly uint[] Section1RewardOrbs = { 102u, 199u, 203u, 204u };

    
    private static readonly uint[] RewardOrbPool = { 102u, 103u, 199u, 199u, 203u, 204u };

    private List<uint> RollRewardOrbs()
    {
        var rng = Random.Shared;
        var count = rng.Next(4, 6); 
        var picks = new List<uint>(count);
        
        picks.Add(rng.Next(2) == 0 ? 102u : 103u);
        picks.Add(199u);
        picks.Add(203u);
        picks.Add(204u);
        while (picks.Count < count)
            picks.Add(RewardOrbPool[rng.Next(RewardOrbPool.Length)]);
        return picks;
    }

    public void RecordBattleSnapshot(PVEBattleResultCsReq req, bool win)
    {
        PreBattleLineupHp = LineupHp;
        PreBattleLevel = PlayerLevel;
        PreBattleExp = PlayerExp;
        PreBattleGold = Gold;

        LastBattleDamageStt = new GridFightDamageSttInfo();
        LastBattleIDEAAPCCFPF = req.Stt?.OGPOFMOGPIP?.IDEAAPCCFPF ?? 0;
        var activeRoleIds = new HashSet<uint>();
        if (req.Stt?.OGPOFMOGPIP?.JPDJMCCKENI != null)
        {
            foreach (var entry in req.Stt.OGPOFMOGPIP.JPDJMCCKENI)
            {
                var roleId = entry.RoleId;
                if (roleId == 0) continue;
                var inActive = entry.Damage > 0 || entry.BOIEGPAPHOP > 0;
                if (inActive) activeRoleIds.Add(roleId);
                LastBattleDamageStt.EABPCKEDDBH.Add(new HHHMMJBGCNG
                {
                    RoleId = roleId,
                    RoleStar = 1,
                    TotalDamage = entry.Damage,
                    LDMNBDIDFCC = inActive,
                    HNLEDBPGDBC = !inActive
                });
            }
        }

        LastRewardedOrbs.Clear();
        LastSectionRewards.Clear();
        LastHpModifyTimeline.Clear();

        if (!win) return;

        
        Gold += 3;
        Gold += 1;
        AddPlayerExp(2);

        
        
        var route = GridFightLevelResolver.ResolveRoute(this);
        if (route?.NodeType != GridFightNodeTypeEnum.Monster) return;

        var dropInfo = new GridFightDropInfo();
        foreach (var orbId in RollRewardOrbs())
        {
            var orbUniqueId = AllocOrbUniqueId();
            LastRewardedOrbs.Add((orbId, orbUniqueId));
            OrbItemByUniqueId[orbUniqueId] = orbId;
            dropInfo.PIBLJLBCKJL.Add(new LHPPIAKKFME
            {
                BGKDAMDFFKH = GridFightDropType.HiolcnpoponMkppcdpchie,
                JJFFLMCCCMM = orbId,
                Num = 1
            });
        }
        LastSectionRewards[2u] = dropInfo;

        
        var perfectClear = req.Stt?.OGPOFMOGPIP?.GMOBFEBBFIE >= 0; 
        if (req.EndStatus == BattleEndStatus.BattleEndWin && perfectClear)
        {
            LineupHp = Math.Min(LineupMaxHp, LineupHp + 2);
        }

        
        var hpBefore = PreBattleLineupHp;
        foreach (var reason in new[]
        {
            GridFightUpdateGlobalHpReason.BakggpnhnneAhikonfebmj,
            GridFightUpdateGlobalHpReason.BakggpnhnneIkhlilhaiol,
            GridFightUpdateGlobalHpReason.BakggpnhnnePonicjjabek,
            GridFightUpdateGlobalHpReason.BakggpnhnneGpdhibafdcg
        })
        {
            LastHpModifyTimeline.Add(new GridFightHpModifyInfo
            {
                Reason = reason,
                FGEDKOINMAG = (int)hpBefore,
                EHMKLNEKIOE = (int)hpBefore
            });
        }
        LastHpModifyTimeline.Add(new GridFightHpModifyInfo
        {
            Reason = GridFightUpdateGlobalHpReason.BakggpnhnneGfimnccfkik,
            FGEDKOINMAG = (int)PreBattleLineupHp,
            EHMKLNEKIOE = (int)LineupHp,
            // uint subtraction underflows when HP dropped during the battle (the common
            // case), sending a ~4-billion garbage delta to the client. Report the magnitude.
            HPOPDNGCALL = LineupHp >= PreBattleLineupHp
                ? LineupHp - PreBattleLineupHp
                : PreBattleLineupHp - LineupHp,
            PDEKDHPNCEN = 1081
        });
    }

    public GridFightSeasonHandBookNotify BuildHandbookNotifyForBattle()
    {
        var n = new GridFightSeasonHandBookNotify
        {
            HandbookGridFightPortalInfo = new GridFightHandBookPortalInfo()
        };
        n.HandbookGridFightPortalInfo.GridFightPortalBuffList.Add(132u);
        return n;
    }

    public GridFightEndBattleStageNotify BuildEndBattleNotify(bool win)
    {
        var notify = new GridFightEndBattleStageNotify
        {
            BCOLJFHDLLD = LineupHp,
            FGEDKOINMAG = PreBattleLineupHp,
            IJEIEJLPGGJ = LineupMaxHp,
            KDOINLGKMBI = LineupMaxHp,
            DCPKPNLKGMM = CurrentChapterId,
            NDOCIKPLKIF = NDOCIKPLKIF,
            SectionId = SectionId,
            EJEIBPEKHLD = PlayerLevel,
            PNOJLNNHBIH = win ? 4u : 1u,
            JGLEDADBNGP = win ? 1u : 0u,
            HPOACJCPJHN = win,
            PACIAIJBOHO = KeepWinCnt,
            IDEAAPCCFPF = LastBattleIDEAAPCCFPF,
            IPCFJBAKLCG = 10,
            MAGCGPMHHEA = PlayerMaxLevel,
            GCBBEEGANEG = new MIOMFOAEHEC
            {
                IPHHGMECAKB = new BLKAIEHOCBC
                {
                    Level = PlayerLevel,
                    Exp = PlayerExp,
                    DKHKEJIAJBN = GetLevelUpExpNeed(PlayerLevel)
                },
                IJAEBDCFEMG = new BLKAIEHOCBC
                {
                    Level = PlayerLevel,
                    DKHKEJIAJBN = GetLevelUpExpNeed(PlayerLevel)
                },
                MAGCGPMHHEA = PlayerMaxLevel
            }
        };

        if (LastBattleDamageStt != null)
            notify.GridFightDamageSttInfo = LastBattleDamageStt.Clone();

        foreach (var (sectionIdx, drop) in LastSectionRewards)
            notify.DHMBKAPKJFN[sectionIdx] = drop.Clone();

        foreach (var hp in LastHpModifyTimeline)
            notify.NNFAFGCGMBB.Add(hp.Clone());

        return notify;
    }

    public BattleInstance? StartBattle()
    {
        return GridFightBattleModule.StartBattle(Player, this);
    }

    public void ConfigureNextBattle(uint stageId, IEnumerable<uint> monsterIds)
    {
        BattleComponent.SetEncounter(stageId, monsterIds);
    }

    public async ValueTask EndBattle(BattleInstance battle, PVEBattleResultCsReq req)
    {
        var win = req.EndStatus != BattleEndStatus.BattleEndLose;
        RecordBattleSnapshot(req, win);

        if (win) KeepWinCnt++;

        var isSeasonFinish = win && IsFinalSection(CurrentChapterId, SectionId);
        LastSettleReason = win
            ? Proto.GridFightSettleReason.CdphdhnlhaoClnlgbcmoij
            : Proto.GridFightSettleReason.CdphdhnlhaoFlnhnhbcdkm;

        AdvanceQueue(7);
        PendingAction = new GridFightPendingAction
        {
            QueuePosition = QueuePosition,
            RoundBeginAction = new GridFightRoundBeginActionInfo()
        };

        await Player.SendPacket(new PacketGridFightSyncUpdateResultScNotify(Player, GridFightSyncKind.PreSettle));
        await Player.SendPacket(new PacketGridFightEndBattleStageNotify(this, win));

        if (isSeasonFinish)
        {
            BattlesFinished++;
            await Player.SendPacket(new PacketGridFightSyncKeepWinCntNotify(1));
            await Player.SendPacket(new PacketGridFightSettleNotify(Player));
            Player.GridFightManager!.GridFightInstance = null;
            return;
        }

        await Player.SendPacket(new PacketGridFightSyncUpdateResultScNotify(Player, GridFightSyncKind.PostBattle));

        OnBattleResolved(win);
        await Player.SendPacket(new PacketGridFightSyncUpdateResultScNotify(Player, GridFightSyncKind.RefreshShop));
    }

    public GridFightCurrentInfo ToProto()
    {
        EnsureDefaultRoles();
        if (CurrentEquipDraft.Count == 0) RefreshEquipDraft();
        if (ShopGoods.Count == 0) RotateShop();

        var info = new GridFightCurrentInfo
        {
            Season = Season,
            DivisionId = DivisionId,
            IsOverlock = IsOverLock,
            UniqueId = UniqueId,
            PendingAction = PendingAction ?? new GridFightPendingAction
            {
                QueuePosition = QueuePosition,
                PortalBuffAction = new GridFightPortalBuffActionInfo
                {
                    FCHPJKAIBHB = 1,
                    GridFightPortalBuffList = { EnsurePortalBuffOffer() }
                }
            },
            BCHPAOCOHIL = new FCBEHGJBJCN { AMNJHJJMPJF = { CurrentEquipDraft } },
            GridFightGameData = new GridFightGameData
            {
                PGFMICHMHFC =
                {
                    new GridFightGameItemInfo { UniqueId = 4, LMGLPGNACLP = new DCGINOONGLH() },
                    new GridFightGameItemInfo
                    {
                        UniqueId = 8,
                        MDFGMBJONAM = new HJJHNNDAEPF
                        {
                            KIBHCHLKGGC =
                            {
                                [1] = 2, [2] = 2, [3] = 4, [4] = 6, [5] = 20,
                                [6] = 40, [7] = 52, [8] = 72, [9] = 84, [10] = 0
                            }
                        }
                    },
                    new GridFightGameItemInfo { UniqueId = 9 },
                    new GridFightGameItemInfo { UniqueId = 12, ONJFAJOAFOG = new KPNJMMPJDKG() },
                    new GridFightGameItemInfo { UniqueId = 49, CJPMGPEIFGG = new BFBNMHEFKIG { CGDLGFHOECL = 2 } },
                    new GridFightGameItemInfo { UniqueId = 87, CJPMGPEIFGG = new BFBNMHEFKIG { CGDLGFHOECL = 1 } }
                }
            }
        };

        info.RogueCurrentGameInfo.Add(BuildBasicInfoSection());
        info.RogueCurrentGameInfo.Add(BuildTeamSection());
        info.RogueCurrentGameInfo.Add(BuildDraftSection());
        info.RogueCurrentGameInfo.Add(BuildItemsShopSection());
        info.RogueCurrentGameInfo.Add(BuildOrbSection());
        info.RogueCurrentGameInfo.Add(BuildLevelSection());
        
        var augSync = new GridFightGameAugmentSync();
        foreach (var aug in ActiveAugmentIds)
            augSync.SyncAugmentInfo.Add(new GridGameAugmentInfo { AugmentId = aug, MHMLMKDFJLN = true });
        info.RogueCurrentGameInfo.Add(new GridFightGameInfo { GridOrbInfo = augSync });
        info.RogueCurrentGameInfo.Add(BuildAugmentSection());
        info.RogueCurrentGameInfo.Add(BuildTeamGameInfoSection());
        return info;
    }

    private GridFightGameInfo BuildBasicInfoSection() => new()
    {
        GridBasicInfo = new GridFightGameBasicInfo
        {
            ANBBPPHBCJH = 3,
            FLEJPPKLJIC = 3,
            HAEOPKELNEO = 10,
            Gold = Gold,
            GridFightBuyExpCost = 4,
            GridFightLineupHp = LineupHp,
            GridFightLineupMaxHp = LineupMaxHp,
            GridFightMaxAvatarCount = 9,
            GridFightMaxInterestGold = 5,
            GridFightOffFieldMaxCount = 6,
            GridFightSyncCurtaskInfo = new GridFightSyncCurrentTaskInfo(),
            GameLockInfo = new GridFightLockInfo()
        }
    };

    private GridFightGameInfo BuildTeamSection()
    {
        var team = new GridFightGameTeamInfo();
        foreach (var (pos, uniqueId) in UniqueIdByPos.OrderBy(kv => kv.Key))
        {
            if (uniqueId == 0 || !RoleByUniqueId.TryGetValue(uniqueId, out var avatarId)) continue;
            team.GridGameRoleList.Add(new GridGameRoleInfo
            {
                Id = avatarId,
                Pos = pos,
                RoleStar = RoleStarByUniqueId.GetValueOrDefault(uniqueId, 1u),
                UniqueId = uniqueId
            });
        }
        return new GridFightGameInfo { GridTraitGameInfo = team };
    }

    private GridFightGameInfo BuildDraftSection()
    {
        var draft = new GridFightGameItemsInfo();
        foreach (var c in Consumables) draft.GridFightConsumableList.Add(c);
        foreach (var e in Equipments) draft.GridFightEquipmentList.Add(e);
        return new GridFightGameInfo { GridDraftInfo = draft };
    }

    private GridFightGameInfo BuildItemsShopSection() => new()
    {
        GridItemsInfo = new GridFightGameShopInfo
        {
            GLIFNMBMMBL = ShopRefreshLeft,
            DNOIFMMLJDN = { ShopRolePool },
            LDEDGOOKHFL = new FJPONJFLOOH
            {
                EDJPMNLLGGB =
                {
                    new MJJEHCBNOKI { MMKNFIFOPPA = 1, FLICPMGFKOK = 100 },
                    new MJJEHCBNOKI { MMKNFIFOPPA = 2 },
                    new MJJEHCBNOKI { MMKNFIFOPPA = 3 },
                    new MJJEHCBNOKI { MMKNFIFOPPA = 4 },
                    new MJJEHCBNOKI { MMKNFIFOPPA = 5 }
                }
            },
            ShopGoodsList = { ShopGoods }
        }
    };

    private GridFightGameInfo BuildOrbSection() => new()
    {
        GridLevelInfo = new GridFightGameOrbInfo
        {
            GridGameOrbList =
            {
                OrbItemByUniqueId.Select(kv => new GridGameOrbInfo { UniqueId = kv.Key, OrbItemId = kv.Value })
            }
        }
    };

    private GridFightGameInfo BuildLevelSection()
    {
        var lvl = new GridFightLevelInfo
        {
            DCPKPNLKGMM = CurrentChapterId,
            NDOCIKPLKIF = NDOCIKPLKIF,
            SectionId = SectionId,
            ECCGJDMOGAN = new DDJIOFONKME(),
            BossInfo = new GridFightBossInfo(),
            CMHBDMOJJEN = new IKFEDFBLOOG()
        };
        EnsureSessionPreview();
        foreach (var campId in SessionCampIds)
            lvl.HGAHMIPIBLO.Add(new OPBCCOLPDPC { PMOGHFIGKPO = campId });
        foreach (var bossId in SessionBossMonsterIds.Where(id => id != 0))
            lvl.BossInfo.IJOPBPABPPM.Add(new PJLBDMPEKFP { MonsterId = bossId, RoleStar = 1 });
        foreach (var affix in SectionAffixIds)
            lvl.IAKFPMOEJLF.Add(new DIBJGAKOCLO { AffixId = affix });
        foreach (var portalBuff in ActivePortalBuffIds)
            lvl.GridFightPortalBuffList.Add(new GridFightGamePortalBuffInfo { PortalBuffId = portalBuff });
        return new GridFightGameInfo { GridShopInfo = lvl };
    }

    private GridFightGameInfo BuildAugmentSection()
    {
        var trait = new GridFightGameTraitInfo();
        trait.ALIDDLBDPDH.Add(new ELEOGABGBKG { DMEKIFJDKFL = 8007, CELFGCJFMPH = { 8007u, 8009u } });
        trait.ALIDDLBDPDH.Add(new ELEOGABGBKG { DMEKIFJDKFL = 11011, CELFGCJFMPH = { 11011u, 11012u } });
        return new GridFightGameInfo { GridAugmentInfo = trait };
    }

    private GridFightGameInfo BuildTeamGameInfoSection() => new()
    {
        GridTeamGameInfo = new LHFDOPGEOML
        {
            ANLGPLOLMOH = 8,
            OGHGLMGJGEM = { ["GP_Avatar_Sparxie_00"] = 2 }
        }
    };

    public List<uint> RollAugments(int count = 3, IEnumerable<uint>? exclude = null)
    {
        var pool = GridFightManager.AugmentPoolKD.ToList();
        if (exclude != null) foreach (var id in exclude) pool.Remove(id);
        var rng = Random.Shared;
        var picked = new List<uint>(count);
        while (picked.Count < count && pool.Count > 0)
        {
            var idx = rng.Next(pool.Count);
            picked.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        while (picked.Count < count)
            picked.Add(GridFightManager.AugmentPoolKD[picked.Count % GridFightManager.AugmentPoolKD.Length]);
        CurrentAugmentOffer = picked;
        return CurrentAugmentOffer;
    }

    public void ClearAugmentOffer() => CurrentAugmentOffer = new List<uint>();

    public List<(uint RoleId, uint EquipId)> RollSupplies(int count = 5)
    {
        var rolePool = GameData.GridFightRoleBasicInfoData.Values
            .Where(r => r.IsInPool && (r.SeasonID == Season || r.SeasonID == 0))
            .Where(r => r.AvatarID >= 1000 && r.AvatarID < 2000)
            .Select(r => r.AvatarID)
            .Distinct()
            .ToList();
        if (rolePool.Count == 0)
            rolePool = new List<uint> { 1218, 1304, 1308, 1220, 1014 };

        var equipPool = GameData.GridFightEquipmentData.Values
            .Where(e => e.EquipCategory == Enums.GridFight.GridFightEquipCategoryEnum.Basic)
            .Select(e => e.ID)
            .ToList();
        if (equipPool.Count == 0)
            equipPool = new List<uint> { 350201, 350202, 350203, 350204, 350207 };

        var rng = Random.Shared;
        var roleCopy = rolePool.ToList();
        var picks = new List<(uint, uint)>(count);
        while (picks.Count < count && roleCopy.Count > 0)
        {
            var rIdx = rng.Next(roleCopy.Count);
            var roleId = roleCopy[rIdx];
            roleCopy.RemoveAt(rIdx);
            var equipId = equipPool[rng.Next(equipPool.Count)];
            picks.Add((roleId, equipId));
        }
        while (picks.Count < count)
            picks.Add((rolePool[picks.Count % rolePool.Count], equipPool[picks.Count % equipPool.Count]));
        CurrentSupplyOffer = picks;
        return CurrentSupplyOffer;
    }

    public void ClearSupplyOffer() => CurrentSupplyOffer = new List<(uint, uint)>();

    public List<EliteBranchOption> RollEliteBranchOptions()
    {
        var camp = GameData.GridFightCampData.Values
            .Where(c => c.Monsters.Count > 0 && (c.SeasonID == Season || c.SeasonID == 0))
            .OrderBy(_ => Random.Shared.Next())
            .FirstOrDefault();
        var monsters = camp?.Monsters ?? new List<GridFightMonsterExcel>();

        var route = GridFightLevelResolver.ResolveRoute(this);
        var baseStage = route?.StageID ?? 70000007u;
        var groupBase = (route != null && route.ParamList.Count > 0) ? route.ParamList[0] : 103u;

        var rng = Random.Shared;
        List<uint> RollMonsters(int count, uint maxTier)
        {
            var picks = new List<uint>(count);
            for (var i = 0; i < count; i++)
            {
                if (monsters.Count == 0) break;
                var pool = monsters.Where(m => m.MonsterTier <= maxTier).ToList();
                if (pool.Count == 0) pool = monsters.ToList();
                picks.Add(pool[rng.Next(pool.Count)].MonsterID);
            }
            return picks;
        }

        var qualities = new[] { 1u, 2u, 3u, 4u }
            .OrderBy(_ => rng.Next())
            .Take(2)
            .OrderBy(q => q)
            .ToList();

        var rewardByQuality = new (uint Type, uint Item, uint Count)[]
        {
            (0, 0, 0),
            (1, 2u, 2),        
            (1, 2u, 5),        
            (5, 350103u, 1),   
            (5, 350105u, 1)    
        };

        var chapter = groupBase / 100;
        var variant = (groupBase / 10) % 10;
        var penaltyBase = 91000u + chapter * 100 + variant * 10;
        CurrentEliteBranchOptions = qualities.Select(quality => new EliteBranchOption
        {
            EncounterId = groupBase * 100 + quality,
            StageId = baseStage,
            PenaltyRuleId = penaltyBase + quality,
            MonsterIds = RollMonsters(10, quality <= 2 ? 2u : 3u),
            RewardItemId = rewardByQuality[quality].Item,
            RewardCount = rewardByQuality[quality].Count,
            DifficultyTier = quality
        }).ToList();
        return CurrentEliteBranchOptions;
    }

    public void ClearEliteBranchOptions() => CurrentEliteBranchOptions = new List<EliteBranchOption>();

    public GridFightPendingAction BuildSectionEntryPending(uint queuePos)
    {
        var route = GridFightLevelResolver.ResolveRoute(this);
        if (route == null)
            return new GridFightPendingAction { QueuePosition = queuePos, ReturnPreparationAction = new GridFightReturnPreparationActionInfo() };

        if (route.NodeType == GridFightNodeTypeEnum.Supply && LastSupplyConsumedSection != SectionId)
        {
            if (CurrentSupplyOffer.Count == 0) RollSupplies();
            var supplyInfo = new GridFightSupplyActionInfo
            {
                FCHPJKAIBHB = CurrentSupplyReroll,
                CGFLMCHMBHL = 1
            };
            foreach (var (roleId, equipId) in CurrentSupplyOffer)
            {
                var sup = new GridFightSupplyRoleInfo { RoleId = roleId };
                sup.GridFightItemList.Add(equipId);
                supplyInfo.SupplyRoleInfoList.Add(sup);
            }
            return new GridFightPendingAction
            {
                QueuePosition = queuePos,
                SupplyAction = supplyInfo
            };
        }

        if (route.NodeType == GridFightNodeTypeEnum.EliteBranch && LastEliteBranchConsumedSection != SectionId)
        {
            if (CurrentEliteBranchOptions.Count == 0) RollEliteBranchOptions();
            return new GridFightPendingAction
            {
                QueuePosition = queuePos,
                EliteBranchAction = new GridFightEliteBranchActionInfo { FCHPJKAIBHB = CurrentEliteBranchReroll }
            };
        }

        if (route.NodeType == GridFightNodeTypeEnum.CampMonster && route.IsAugment == 1
            && LastAugmentConsumedSection != SectionId)
        {
            if (CurrentAugmentOffer.Count == 0) RollAugments();
            var augInfo = new GridFightAugmentActionInfo();
            foreach (var aid in CurrentAugmentOffer)
                augInfo.PendingAugmentInfoList.Add(new GridFightPendingAugmentInfo { AugmentId = aid, ALJBADEOPAH = 1 });
            return new GridFightPendingAction
            {
                QueuePosition = queuePos,
                AugmentAction = augInfo
            };
        }

        return new GridFightPendingAction
        {
            QueuePosition = queuePos,
            ReturnPreparationAction = new GridFightReturnPreparationActionInfo()
        };
    }
}

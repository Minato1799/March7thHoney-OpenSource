# Aetherium Wars (AetherDivide) & Heliobus

Two previously-unimplemented event modules. Both had full protocol definitions and
`CmdIds` but no handlers, logic, or persistence. Implemented following the existing
`DiceCombat` / `SwordTraining` activity pattern.

## Aetherium Wars — `AetherDivide` (以太战线)

| Area | Status |
|---|---|
| Event info / challenge info panels | ✅ snapshots (`GetAetherDivideInfo`, `GetAetherDivideChallengeInfo`) |
| Lineup set / slot switch | ✅ `SetAetherDivideLineUp`, `SwitchAetherDivideLineUpSlot` |
| Spirit EXP / level up | ✅ `AetherDivideSpiritExpUp` (100 EXP → 1 level, persisted) |
| Passive skills | ✅ `Equip/ClearAetherDividePassiveSkill` (slot→item, persisted; no ScRsp — fire-and-forget) |
| Challenge reward | ✅ `AetherDivideTakeChallengeReward` (dedupe persisted) |
| Endless refresh | ✅ `AetherDivideRefreshEndless` |
| Battles (stage/scene/challenge) | ✅ acknowledged (retcode 0); battle simulation not server-driven |
| Leave scene | ✅ consumed (no ScRsp) |

13 handlers (`Recv/AetherDivide/`), 10 response packets (`Send/AetherDivide/`),
`AetherDivideInstance`, persisted `AetherDivideData`.

## Heliobus (Aha's lab raid + SNS feed)

| Area | Status |
|---|---|
| Activity data panel | ✅ `HeliobusActivityData` (level + phase snapshot) |
| Level upgrade | ✅ `HeliobusUpgradeLevel` (persisted) |
| Enter battle / start raid | ✅ acknowledged (retcode 0); battle not server-driven |
| SNS read / like / post / comment | ✅ persisted read & like-toggle state, post id allocator |

8 handlers (`Recv/Heliobus/`), 8 response packets (`Send/Heliobus/`),
`HeliobusInstance`, persisted `HeliobusData`.

## Known limitations

Same bar as the other event stubs in this project: panels open, actions are
acknowledged without protocol errors, and meta progression persists across logins.
The interactive battles (AetherDivide auto-battler / Heliobus raids) are acknowledged
rather than fully simulated server-side — `BattleInfo`/`Scene` are not dispatched, and
`Reward` grants on settle are client-driven. The data-panel snapshots return
`Retcode = 0` with the persisted scalar fields populated; complex nested fields are left
default. Proto fields are name-obfuscated, so semantics were reconstructed from layout.
Builds clean (0 errors).

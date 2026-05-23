# Roadmap Progress Notes

Tracks work against the README roadmap. Each entry lists what was actually changed
and what remains.

## Settlement / sync stability (edge cases) — ✅ fixes landed

- **Battle HP settlement could corrupt avatar HP.** In `BattleManager.EndBattle`,
  `prop.LeftHp / prop.MaxHp` divided by a client-supplied `MaxHp`. When `MaxHp == 0`
  the ratio became `Infinity`/`NaN`; casting that to `int` yields `int.MinValue`,
  which then wraps to a multi-billion value through the later `(uint)` cast and
  desyncs the client. Now guarded (`MaxHp > 0`, NaN/Infinity check) and the result is
  `Math.Clamp(..., minimumHp, 10000)`. SP is likewise clamped to `[0, 10000]`.
- **Storage-layer backstop.** `AvatarData.SetCurHp` / `SetCurSp` now clamp to
  `[0, 10000]` so no settlement/sync path can persist an out-of-range value.

## Currency War (Aether Divide / GridFight) — ✅ edge-rule fixes

- **Free purchase when out of gold.** `GridFightInstance.TryBuyGoods` topped the
  player's gold up to the price (`if (Gold < price) Gold = price;`) and then
  subtracted — effectively a free buy. Now it rejects the purchase when gold is
  insufficient, matching official behaviour. The caller already handles the
  rejection gracefully (no gold spent, no role granted).
- **HP-delta underflow in the post-battle timeline.** `HPOPDNGCALL = LineupHp -
  PreBattleLineupHp` is `uint` arithmetic; after a damaging battle (`LineupHp <
  PreBattleLineupHp`, the common case) it underflowed to ~4 billion and sent a
  garbage delta to the client UI. Now reports the magnitude of the change.

## Configurable server options — ✅ added and wired

New `ServerOption` settings (in `Config.json`), each wired into real logic:

| Option | Default | Effect |
|---|---|---|
| `BattleReviveHpPercent` | `20` | HP% avatars revive to after losing a battle (was hardcoded `2000`/20%). |
| `CurrencyWar.StartGold` | `3` | Starting gold for a Currency War run. |
| `CurrencyWar.StartLineupHp` | `80` | Starting lineup HP (capped at 100). |
| `CurrencyWar.ShopRefreshPerSection` | `2` | Free shop refreshes per section. |

## Event-mode coverage — ✅ ongoing

- **March 7th Sword Training** implemented from scratch (see `docs/SwordTraining.md`).
- Next unimplemented events that already have protos + `CmdIds` but no handlers:
  EvolveBuild / Aetherium Wars (9 reqs), Heliobus (8), StarFight (2), FantasticStory (2).

## Divergent Universe / Simulated Universe — ⚠️ not feasible to "improve" yet

There is **no Simulated Universe / Rogue manager or handlers** in the project at all —
the entire `Rogue*` family (Base SU, Divergent Universe, Unknowable Domain, Chess Rogue)
is unimplemented. The protocol layer exists (311 `Rogue*` protos, `Rogue*Excel` data
classes), but:

1. There is no game-side scaffolding to "improve" — it would be a from-scratch build of
   room generation, blessings/miracles/curios, dice/chess rogue, boss decay, and settlement.
2. The protos are name-obfuscated and the room/blessing logic depends heavily on the
   `Resources/` dataset (`Config/Level/Rogue`, `Config/Gameplays/RogueDLC`) — which is not
   in-repo (see `docs/Resources.md`). Correctness cannot be verified without it.

**Recommendation:** treat Divergent Universe as a dedicated, multi-session feature build
(model on the existing `Challenge` mode flow) once `Resources/` is in place, rather than a
"fix". Tracked as a future item.

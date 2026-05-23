# March 7th Sword Training

Implementation notes for the **March 7th Sword Training** event
(`CmdSwordTraining*` opcode family). This event was previously unimplemented —
all protocol definitions and `CmdIds` existed, but there were no handlers,
no game logic, and no persistence.

## What is implemented

| Area | Status | Detail |
|---|---|---|
| Event data panel | ✅ | `GetSwordTrainingData` returns a well-formed snapshot (unlocked story lines, learned/traced skills, viewed endings, daily phase, active game). |
| Game session lifecycle | ✅ | Start / Resume / Restore / Give Up, persisted across logins. |
| Turn actions | ✅ | `TurnAction` advances the turn counter and echoes the resolved action ids. |
| Skill progression | ✅ | Learn skill, toggle skill trace — both persisted. |
| Story flow | ✅ | Story confirm, dialogue option select, story battle acknowledgement. |
| Exam flow | ✅ | Enter exam, exam result confirm. |
| Endings | ✅ | Select ending, mark ending viewed (gallery progress persisted). |
| Daily phase | ✅ | Daily phase confirm advances the persisted phase counter. |

## Architecture

Mirrors the existing `DiceCombat` activity pattern:

- **Persistence** — `Common/Database/Activity/SwordTrainingData.cs`, stored as a JSON
  column on the existing `ActivityData` table. Saved automatically by the periodic
  database save loop (no explicit save calls needed in handlers).
- **Game logic** — `GameServer/Game/Activity/Activities/SwordTraining/SwordTrainingInstance.cs`,
  constructed by `ActivityManager` and reachable via `player.ActivityManager.SwordTraining`.
- **Handlers** — 16 handlers under `GameServer/Server/Packet/Recv/SwordTraining/`,
  auto-registered through the `[Opcode(...)]` reflection scan.
- **Responses** — 16 `BasePacket` subclasses under `GameServer/Server/Packet/Send/SwordTraining/`.

## Known limitations

The Sword Training protos are name-obfuscated (e.g. `LMBHDCFPPLL`, `BMKAEFAKNFJ`),
and the deep combat-resolution structures (`PendingAction`, `SkillInfo`, the per-turn
battle simulation) are reconstructed from field layout rather than official semantics.
As a result:

- The interactive turn-based combat is acknowledged rather than fully simulated
  server-side; story battles return success without dispatching a `SceneBattleInfo`.
- Reward grants on `GameSettle` are not wired (the settle is client-driven here).

These can be deepened later if a reference dataset or live-client trace becomes available.
The current implementation makes the event open, browse, start, progress, and persist
without protocol errors — matching the bar used by the other event stubs in this project.

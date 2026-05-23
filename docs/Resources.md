# Resources Setup (ExcelOutput / Config / LevelOutput)

The server does **not** ship game resources — the `Resources/` folder is
`.gitignore`d. Without it the server cannot load excels, scene floors, missions,
or Rogue/Gameplays config, so it will fail to start or behave incorrectly.

## Where the server looks

`PathConfig.ResourcePath` defaults to `Resources` (relative to the running
executable / working directory). At startup `ResourceManager` reads, among others:

```
Resources/
├── ExcelOutput/                         # all *.json excel tables
├── Config/
│   ├── LevelOutput/RuntimeFloor/        # scene floor info
│   ├── LevelOutput/SharedRuntimeGroup/
│   ├── Level/Mission/                   # mission info
│   ├── ConfigCharacter/
│   ├── ConfigAdventureModifier/
│   └── Gameplays/RogueDLC/              # Simulated Universe DLC config
└── ... (TextMap, etc.)
```

This is the standard **DanhengServer / StarRail data** layout. March7thHoney is
derived from DanhengServer, so the same resource packs apply.

## Recommended source — Dimbreath (GitLab)

**`Dimbreath/turnbasedgamedata`** is the canonical, complete data dump and is all
you need: <https://gitlab.com/Dimbreath/turnbasedgamedata>

> ✅ **`main` already tracks `4.2.0`** — verified: latest commits are tagged
> `OSPRODWin4.2.0_...` (May 2026), which matches `game_version=4.2.0` in
> `Star Rail Games/config.ini`. Just clone `main`; no old-commit checkout needed.
>
> If the game client is ever updated past 4.2, check out an `OSPRODWin4.2.0_*`
> commit from the history instead of the newest one (the repo has no version tags,
> only commit messages name the version).

It contains every folder the server reads:

```
Config/   (incl. LevelOutput/RuntimeFloor, LevelOutput/SharedRuntimeGroup,
           Level/Mission, Level/Rogue, Level/RogueDialogue, Gameplays, ConfigCharacter, ...)
ExcelOutput/
Stages/
Story/
TextMap/
```

This raw dump already includes the generated `LevelOutput`/`Level/Mission` pieces, so a
separate `DanhengServer-Resources` overlay is **not required** for a 4.2 boot.

### Setup

```bash
# 1. Clone the data (shallow = much faster; the full history is large)
git clone --depth 1 https://gitlab.com/Dimbreath/turnbasedgamedata.git Resources

# 2. Resulting layout (next to the built server executable):
#    Resources/ExcelOutput/  Resources/Config/  Resources/TextMap/  ...
```

Or set `Path.ResourcePath` in the server config to the absolute path of that folder.

> Optional: the project Discord (<https://discord.gg/castoriceps>) may host a
> pre-trimmed `Resources.zip` if you want a smaller download than the full dump.

## Verifying

On a successful boot the log prints resource-load lines (excel counts, floor/mission
loading). If you see "file not found" for `ExcelOutput/...` or empty floor data,
the `Resources/` path or version is wrong.

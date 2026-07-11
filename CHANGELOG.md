# Changelog

All notable changes to Backyard Battle. Versioning: Major.Minor.SubMinor — bump
sub-minor on every merged change.

## v0.1.0 — 2026-07-11

Initial project foundation (Milestone M0, in progress):

- Repository scaffold: Unity 6000.0 LTS project skeleton (Packages manifest,
  ProjectVersion), assembly-definition layout (`BB.Data` ← `BB.Simulation` ←
  `BB.Core`/`BB.Presentation`/`BB.UI`, editor-only `BB.Editor`, tests)
- Full game design document at `docs/GDD.md` (The Great Backyonder: lore, 8-fighter
  roster, 3 demo stages, The Shoebox award system, game-feel pillars, mode roadmap)
- Data layer: `FighterDefinition`, `AttackDefinition`/`HitboxWindow`,
  `StageDefinition`, `HazardDefinition`, `AwardDefinition`, `FighterRoster`,
  `StageCatalog` ScriptableObject schemas
- Simulation layer: `InputSnapshot`/`FighterState` structs, kinematic
  `FighterController` motor core (tick-driven, XY-plane locked), `KnockbackFormula`
  (Smash-style), `HitResolver` with explicit overlap queries and per-activation
  dedupe, `Hurtbox`, hazard timer scaffolding with mandatory telegraphs
- Core layer: `PlayerSlot` + `IInputSource` abstraction (device/bot/network),
  `AwardLedger` (append-only reward event ledger, wallet = fold), `SaveSystem`
  (versioned local JSON), `MatchController` state-machine skeleton
- EditMode tests: knockback formula, ledger fold/persistence round-trip
- Build tooling: `scripts/build.sh` + `BB.Editor.BuildScripts` (Linux/Windows
  clients, Linux dedicated server), `ServerBuild/Dockerfile`
- CI: Forgejo Actions workflow — structural validation always; Unity test job
  gated behind `UNITY_CI_ENABLED` until license secret is provisioned

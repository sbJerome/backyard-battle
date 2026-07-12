# Changelog

All notable changes to Backyard Battle. Versioning: Major.Minor.SubMinor — bump
sub-minor on every merged change.

## v0.1.3 — 2026-07-11

- Editor bootstrap: `BB.Editor.ProjectBootstrap.Run` generates layers
  (Fighter/Hurtbox/Stage), URP pipeline assets, greybox material, Chip + Dummy
  fighters (attack assets, prefabs with wired hurtboxes), Stage_Greybox
  definition + scene (geometry, camera, match harness), Boot/MainMenu/Lobby
  scenes, and build settings — fully headless-runnable
- `DevMatchStarter`: dev harness that starts a match on Play (P1 device input,
  N training-dummy bots) — the M1 gate loop
- `BrawlCamera` self-acquires targets from the live match (fixes a would-be
  circular assembly reference from Core → Presentation)
- ProjectVersion pinned to 6000.0.79f1 (latest 6000.0 LTS, matches installed editor)

## v0.1.2 — 2026-07-11

- CI: validate job now runs on the runner's default image (alpine container had
  no node for actions/checkout); JSON/layering checks rewritten in node
- CI: Unity EditMode tests split into a manual-dispatch workflow
  (`unity-tests.yml`) until license provisioning; removes the broken `vars`
  gate and the node-less unityci checkout failure

## v0.1.1 — 2026-07-11

- CI: fix `runs-on` labels (`docker` → `ubuntu-latest`) so the Forgejo Actions
  runner actually claims the validate job

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

# Backyard Battle — Demo Plan

## Context

Greenfield game: **Backyard Battle** — a multiplayer party platform brawler where 8-year-old **Max Finch** pits backyard objects and critters against each other in imaginary battles. Decisions locked with the user:

- **Core mode:** Smash-style platform brawler first; **Rumble Pile** (Gang Beasts-like physics brawler) and **Recess** (Mario Party-like minigames) shipped as "Coming Soon" and designed-for from day one (shared roster/stages/awards).
- **Engine:** Unity 6 (6000.0 LTS), C#, URP. **Multiplayer:** couch (6 local) + LAN direct-IP + full online in the demo. **Art:** 3D low-poly "toy shelf," 2.5D side-view camera.
- **Demo scope:** 6 simultaneous players, 8 fighters, 3 stages, shared award system. Later: 20+ players, 13 stages.

Environment (audited): Fedora 40, 40c/204GB (great for builds), Quadro P400 (OK for low-poly URP; NVIDIA driver + X11 session). Have podman/git/kubectl. **Must install:** Unity Hub + editor, git-lfs, dotnet-sdk-8.0, blender (dnf — handed to user as `! sudo` commands).

---

## 1. Creative design (condensed from full GDD — keep full doc at `docs/GDD.md` in the repo)

**World — The Great Backyonder.** From bug's-eye view the Finch backyard is a continent of warring kingdoms (The Lawnlands, Paver Steppes, Garden Reaches, Mount Grillmore, The Chlorine Sea, and the Forbidden Zone over the fence). Everything comes alive through Max's imagination, played deadpan-straight: in-world lore is written with total gravity ("The Gnome has not blinked in six years"), while the "gods" are a bored kid, a sprinkler timer, and a dog named Biscuit. Eternal McGuffin: the **Golden Bottle Cap**, which Max buried and forgot where. Framing device drives the UI: Max's giant hands place fighters at match start, crayon/notebook menus, kid-voice countdown ("Three… two… one… BACKYARD BATTLE!"), pause = Mom called Max in for lunch. Tone rules: deadpan epic, no real violence coding (bonks and launches), every fighter believes they're the protagonist, Max is a benevolent easily-distracted god.

**Demo roster (8, archetype-balanced):**

| Fighter | Object | Archetype | Signature |
|---|---|---|---|
| **Chip** | Pebble | Compact armored heavy | Skip Shot (bouncing pebble), hand-flick "Skipping Stone" recovery, becomes terrain (Sediment); Dense armor gimmick |
| **Sarge** | Black ant | Honest rushdown (the "Mario slot") | Crumb Toss homing mark, ant-ladder tether recovery; perfect-shield → damage buff. **Rival: Pepper** (unique intro splash) |
| **Pepper** | Red fire ant | Aggro rushdown / burn DoT | Sizzle stacks (burn), flame-trail dash, command-grab bites; knockback scales with victim's Sizzle stacks |
| **Noodle** | Garden snake | Grappler with range | Coil charge lunge, Big Hug command grab (crowd-breakable), decoy shed skin; tail is a real reduced-damage hurtbox |
| **Gnorman the Unblinking** | Garden gnome | Super-heavy turret/zoner | Fishing-line yank, mushroom turrets, pedestal recovery that leaves a platform, Ceramic Stance reflect (shatters to charged smashes) |
| **Dandy Lion** | Dandelion | Floaty aerial zoner | Drifting seed shots + wind gust that carries them; at 100%+ goes to seed — frailer but double seeds |
| **Turbo** | Garden snail | Trap/defense tank | Slime patches (fast for him, slippery for you), shell bowling, parks shell as bunker and steps out; rear-facing damage reduction |
| **Wiener** | Squeaky dog toy | Chaos trickster | SQUEAK ring (reflects projectiles), random-bounce self-throw, Play Dead counter; bounces off surfaces when launched |

Coming-soon silhouettes (8): Duchess Vespa (wasp), Big Slab (paver), Sprinx (sprinkler), Doug (worm), Flip (flip-flop), Glimmer (firefly), Sir Cone-a-Lot (pinecone), Mingo (lawn flamingo).

**Demo stages (3, classic variety mix):**
- **The Sandbox** — neutral/tournament: flat packed sand, shovel + ruler side platforms, clean ledges; hazardless in tournament toggle (ship first — zero moving colliders, netcode baseline). Army-men spectators, sandcastle "Old Capital" background.
- **Mount Grillmore** — hazard-heavy: fought on a kettle grill; telegraphed flare-up pillars (~25s), rideable kebab skewer (~45s), once-per-match Lid phase (Max closes the lid, dome becomes the floor). Surf-rock with sizzle-foley percussion.
- **The Tomato Trellis** — platform-heavy/vertical: pot rim floor with wall-jumps, lattice drop-throughs, sagging vine, tomato platforms that snap after ~4s standing and regrow; sprinkler windbox pass (~60s). Hazards are terrain-based, contrasting Grillmore.

Coming-soon stages (10 one-liners in GDD): Chlorine Sea, Pergola Heights, Gazebo Rotunda, Porch Step Ascent, Shed of Legends, Birdbath Falls, Fort Doghouse, Gutter Run, Compost Kingdom, The Forbidden Fence.

**Awards — "The Shoebox" (shared by all three modes):** **Bottle Caps** soft currency (every match, every mode) + **Shinies** (rare, uniquely-named collectibles from hard achievements only). Achievements = **Brags** (Doodle/Marker/Gold Star tiers) in crayon merit-badge notebook pages. **Style Stickers** end-of-match comedy bonuses ("Most Polite," "Gravity's Favorite"). Cosmetics only (hats/paints/podium flair; every fighter gets an accessory anchor point from day one). Rank ladder **The Pecking Order** (Ant Food → Monarch of the Backyonder). Post-match **juice-box podium** ceremony + **Macaroni Medal** + **Polaroid** saved to a Scrapbook (shareable image export). Shop = **Max's Trade Blanket**.

**Game-feel pillars:** (1) Chaos is fair — every hazard telegraphs ≥2s; (2) readable at 6 players — silhouette-first design, VFX budget caps; (3) every object is a personality; (4) toy physics, arcade truth — animation lies, hitboxes never do; (5) a seven-year-old and their parent lose to each other.

**Mode roadmap:** Rumble Pile reuses roster physical stats (Chip's density, Wiener's restitution, Noodle's length) as active-ragdoll comedy parameters on the same stages; Recess reuses stages as minigame backdrops and needs generic run/jump/carry/celebrate clips per fighter (on the animation checklist now). Both emit the same reward events into the Shoebox — **no mode ever writes Caps directly**.

---

## 2. Technical architecture

### Install (Fedora 40)
Unity Hub RPM repo → **Unity 6000.0 LTS** with Linux IL2CPP, **Linux Dedicated Server**, and Windows build modules (cross-build for Windows friends). `sudo dnf install unityhub git-lfs dotnet-sdk-8.0 blender` via `!` commands (sudo needs TTY). Unity Personal license. Gotchas: X11 session over Wayland, akmod-nvidia driver, case-sensitive FS naming conventions.

### Netcode: Fish-Net v4, client-server, server-authoritative + client prediction
- Fish-Net over NGO/Mirror/Photon: built-in tick prediction (`[Replicate]`/`[Reconcile]`), free, scales past 20 players. **GGPO rollback rejected** (needs deterministic custom sim; wrong tradeoff for a party brawler). Hedge: sim consumes plain `InputSnapshot` structs, all fighter state in plain structs, never `Input.*`/`Time.deltaTime` in gameplay — keeps the rollback door open.
- 60 Hz fixed tick; local player predicted, remotes interpolated (30 Hz send); **hits resolve server-side only** (never predict hits on remotes).
- Per mode: couch = same sim, no transport; LAN = listen server (Tugboat UDP, direct IP); internet = dedicated headless server.

### Input & couch
Unity Input System only; `PlayerInputManager` join-by-button-press in Lobby; keyboard split schemes; player cap data-driven (6→20). **Load-bearing abstraction:** `PlayerSlot` + `IInputSource` (Device/Network/Bot) — fighters never own `PlayerInput`. Demo: a machine is couch (offline) OR online (1 local player); couch-in-online-lobby is post-demo (design already supports it).

### Online infra: self-hosted dedicated server on k3s
Dedicated Server build (Linux IL2CPP headless) → podman image → Forgejo registry → k3s Deployment + MetalLB Service UDP 7777 (**check LB IPs across ALL namespaces first**). Public ingress: RackNerd VPS (198.46.187.16) DNAT udp/7777 over existing WireGuard to the MetalLB IP. LAN players hit the MetalLB IP directly. Demo lobby = Join screen with prefilled official server; no browser. UGS Relay deferred. Accepted: single region, VPS hop latency (~20–60 ms — measure at M4/M5; fallback = cheap cloud VM, same container).

### Project structure (`~/source/BackyardBattle/`)
- `Assets/_Project/{Code,DataAssets,Prefabs,Art,Audio,Scenes,Settings}`; full GDD at `docs/GDD.md`.
- **Asmdefs are the load-bearing decision:** `BB.Data` ← `BB.Simulation` (motor, HitResolver, hazards — zero input/UI/netcode refs) ← `BB.Netcode` / `BB.Presentation` / `BB.UI`, plus `BB.Core`, `BB.Editor`, tests. Clean dedicated-server builds, sane prediction, rollback option preserved.
- Scenes: `Boot` (persistent GameManager/services/save) → `MainMenu` → `Lobby` (char+stage select; couch join & online) → additive `Stage_Sandbox` / `Stage_Grillmore` / `Stage_Trellis` → Results overlay → Lobby. Plus `Stage_Greybox` dev stage (never ships).
- **ScriptableObject-driven content** (new fighter/stage = assets + prefab, zero code): `FighterDefinition` (stats incl. physical params Rumble Pile will reuse, moves, skins, cosmetic anchor), `AttackDefinition` (tick frame data + capsule `HitboxWindow`s: damage, angle, base KB, KB growth), `StageDefinition` (count-scalable spawns, blast zone, camera bounds, hazards), `HazardDefinition` (mandatory 2s telegraph field), `AwardDefinition` + reward tables, `FighterRoster`/`StageCatalog` the UI iterates.
- Core systems: `MatchController` state machine (server-owned online), `FighterController`, `HitResolver`, `BrawlCamera` (frames fighter bounding box, clamped to stage bounds — 20-player-safe by construction), `StageHazardSystem` (global deterministic timers), `CeremonyDirector` (podium/stickers/Polaroid), `SaveSystem` + `AwardLedger` (**append-only event ledger** — CapsEarned/BragUnlocked/… — wallet = fold over ledger; versioned local JSON, checksummed, sync-ready), `AwardTracker` (pure subscriber to match events), UGUI HUD/CharSelect/Results/Pause.

### Gameplay physics
- **Kinematic hand-rolled motor** (kinematic RB + capsule casts in fixed tick) — exact platformer feel, plain reconcilable state struct. Knockback = `Launched` state, self-integrated velocity/gravity/hitstun (no ragdoll). Sim locked to XY plane; full-3D visuals; cosmetic physics never touches gameplay collision.
- **Hitboxes via explicit `OverlapCapsuleNonAlloc`** during active ticks against bone-attached hurtbox capsules; `HitResolver` dedupes, applies formula, raises events (presentation → hitstop/shake; netcode → replication). Gizmo debug overlay in week one.
- Smash-style formula (tune later): `kb = (((p/10 + p*d/20) * (200/(w+100)) * 1.4 + 18) * g/100) + b`; hitstun ≈ `kb*0.4` ticks; blast zone → KO → stock loss → invuln respawn. Tick frame data is timing truth; animation is presentation.

### VCS & CI
- Forgejo repo (source of truth) + GitHub push-mirror. Force-text, visible meta. **Git LFS** for binaries (fbx/blend/png/wav…), NOT for .unity/.prefab/.asset/.meta. Verify the GitHub mirror carries LFS objects after first big push (often doesn't — acceptable).
- Forgejo Actions CI = **compile + EditMode/PlayMode tests only** for the demo (unityci/editor images; Personal `.ulf` as a secret — budget one painful afternoon). Player builds **local** via `scripts/build.sh` (`-batchmode -executeMethod BB.Editor.BuildScripts.Build{LinuxClient,WindowsClient,LinuxServer}`); server build feeds `podman build`.

### Assets (~$0)
CC0: Kenney (props/UI/audio/**controller glyphs**), Quaternius (critters), KayKit, Poly Pizza, ambientCG, freesound/jsfxr; licenses tracked in `Art/CREDITS.md`. Blender (Flatpak) → FBX → Unity **Generic** rigs (Mixamo out — humanoid-only). Chip = 1–3 bones + runtime squash tween (**cheapest fighter — build first**); ants/snake = 6–12 bone armatures (snake wiggle procedural); Gnorman = simple biped. ~9 clips/fighter + generic run/jump/carry/celebrate (Recess needs them); feel comes from hitstop/VFX/frame data.

---

## 3. Milestones (~4–6 months)

| # | Milestone | Gate | Size |
|---|---|---|---|
| M0 | Foundation: dnf installs, repo+LFS on Forgejo, Unity project, URP, Input System, asmdefs, Boot/MainMenu, build.sh, CI tests, GDD committed | compiles in CI | ~1 wk |
| M1 | **One fighter feels good** (offline): motor, `Stage_Greybox`, Chip with 2–3 attacks, HitResolver + gizmos, knockback/stocks, camera v1 | fun with 1 pad + 1 dummy | 3–4 wk |
| M2 | **Couch brawl (MVP slice)**: 2–6 local players, HUD (marker-scrawl percents), match flow, results | human playtest | 2 wk |
| M3 | Data-driven roster & stages: char select from roster SO, 3 fighters (Chip/Sarge/Gnorman), 3 greybox stages, first hazard (Grillmore flare-up) | | 2–3 wk |
| M4 | **Netcode core** ⚠ riskiest: Fish-Net prediction over the motor, server-auth hits, LAN direct-IP, headless server boots a match | playable at simulated 80–120 ms | 3–5 wk |
| M5 | Online for real: container→k3s→MetalLB→VPS DNAT, online lobby (ready-up, pick sync), disconnect handling | 6-client WAN playtest | 2–3 wk |
| M6 | Content & polish: all 8 fighters (Pepper/Noodle/Dandy/Turbo/Wiener join), art pass + full hazards on 3 stages, juice (hitstop/shake/SFX), Shoebox awards + ceremony + Scrapbook, coming-soon teasers on select screens, Windows builds | | 4–5 wk |
| M7 | Hardening & demo release: WAN playtests, netcode tuning, packaging (itch.io / Forgejo release) | | 1–2 wk |

Order is fixed: prove fun offline (M1–M2) before netcode; do NOT slide M4 later — prediction over the custom motor is risk #1 and must be validated mid-project.

**Top risks:** (1) client prediction of the custom motor under latency — mitigated by asmdef split, struct state, latency-sim testing, server-only hits; (2) VPS+WireGuard hop latency — measure early, fallback cloud VM; (3) CI Unity licensing — local builds are primary; (4) Fish-Net third-party — asmdef boundary caps migration; (5) Linux editor on the P400 — X11 + proprietary driver, worst case edit elsewhere/build here.

---

## 4. Verification

- **M1/M2:** controller in hand — hit dummy, launch players, KO off blast zones; gizmo overlay confirms hitboxes match `AttackDefinition` frame data. EditMode tests: knockback formula, motor step, ledger fold. PlayMode smoke test in CI.
- **M4:** Fish-Net latency sim (80–120 ms, 2% loss) — crisp movement, no attack rubber-banding; real LAN match between two machines.
- **M5:** server pod on k3s; UDP reachability of VPS:7777 from outside; full 6-client internet match; verify server authority by attempting a client-side cheat.
- **Awards:** finish match → ceremony (podium, sticker thwap, Polaroid) → ledger JSON reflects events → survives restart; same events fire from a bot-only match (mode-agnostic contract).

## 5. Critical files (to create)

- `Assets/_Project/Code/Simulation/FighterController.cs` — kinematic motor + attack state machine (everything orbits this)
- `Assets/_Project/Code/Simulation/HitResolver.cs` — hitbox queries, knockback formula, hit events
- `Assets/_Project/Code/Data/FighterDefinition.cs` (+ Attack/Stage/Hazard/Award definitions) — the SO schema making content data-driven
- `Assets/_Project/Code/Netcode/NetworkFighter.cs` — Fish-Net `[Replicate]`/`[Reconcile]` wrapper (riskiest file)
- `Assets/_Project/Code/Core/AwardLedger.cs` — append-only reward ledger all modes share
- `scripts/build.sh` + `ServerBuild/Dockerfile` — CLI client builds + dedicated-server image for podman/k3s
- `docs/GDD.md` — the full game design document (lore, roster kits, stage specs, award system)

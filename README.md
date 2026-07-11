# Backyard Battle

**v0.1.1**

A multiplayer party platform brawler set in **The Great Backyonder** — the Finch
family backyard as seen from bug's-eye view. Eight-year-old **Max Finch** pits
backyard objects and critters against each other in imaginary battles: pebbles,
ants, garden gnomes, snakes, snails, dandelions, and one extremely confident
squeaky dog toy.

- **Engine:** Unity 6 (6000.0 LTS), C#, URP, 3D low-poly "toy shelf" art, 2.5D side-view camera
- **Players:** 6 simultaneous (demo) → 20+ (later); couch + LAN + online
- **Netcode:** Fish-Net v4, client-server, server-authoritative with client prediction
- **Modes:** Backyard Battle (platform brawler) now; **Rumble Pile** (physics brawler) and **Recess** (minigames) coming soon — all sharing one roster, stage set, and award system (**The Shoebox**)

Full design document: [`docs/GDD.md`](docs/GDD.md). Implementation plan and
milestones: [`docs/PLAN.md`](docs/PLAN.md).

## Repository layout

```
Assets/_Project/
  Code/
    Data/          BB.Data          — ScriptableObject schemas (fighters, attacks, stages, awards)
    Simulation/    BB.Simulation    — tick-driven fighter motor, hit resolution, hazards (no input/UI/net refs)
    Core/          BB.Core          — game/match flow, player slots, save system, award ledger
    Netcode/       BB.Netcode      — Fish-Net wrappers (added at M4)
    Presentation/  BB.Presentation  — camera, VFX/SFX glue
    UI/            BB.UI            — menus, HUD
    Editor/        BB.Editor        — build scripts, validators
    Tests/         EditMode/PlayMode tests
  DataAssets/      ScriptableObject instances (roster, moves, stages, awards)
  Prefabs/  Art/  Audio/  Scenes/  Settings/
docs/              GDD, plan
scripts/build.sh   CLI builds (Linux/Windows clients, Linux dedicated server)
ServerBuild/       Dockerfile for the dedicated server image (podman → k3s)
.forgejo/workflows CI (compile + tests)
```

## Getting started (Fedora)

1. Install tooling (needs sudo):
   ```bash
   sudo sh -c 'echo -e "[unityhub]\nname=Unity Hub\nbaseurl=https://hub.unity3d.com/linux/repos/rpm/stable\nenabled=1\ngpgcheck=1\ngpgkey=https://hub.unity3d.com/linux/repos/rpm/stable/repodata/repomd.xml.key" > /etc/yum.repos.d/unityhub.repo'
   sudo dnf install -y unityhub git-lfs dotnet-sdk-8.0
   flatpak install -y flathub org.blender.Blender
   git lfs install
   ```
2. In Unity Hub: install **6000.0 LTS** with modules *Linux Build Support (IL2CPP)*,
   *Linux Dedicated Server Build Support*, *Windows Build Support*. Sign in
   (Personal license).
3. Open this folder as a project. First open generates `Library/` and default
   ProjectSettings; if the installed patch differs from
   `ProjectSettings/ProjectVersion.txt`, accept the upgrade and commit the rewrite.
4. Linux editor notes: use an X11 session (not Wayland) and the proprietary NVIDIA
   driver; 100%/200% display scaling.

## Builds

```bash
scripts/build.sh linux-client     # → Builds/LinuxClient/
scripts/build.sh windows-client   # → Builds/WindowsClient/
scripts/build.sh linux-server     # → Builds/LinuxServer/  (headless, feeds podman build)
scripts/build.sh server-image     # linux-server + podman build → backyard-battle-server image
```

## CI

Forgejo Actions runs asmdef/JSON validation always; Unity compile + EditMode/PlayMode
tests run once the `UNITY_LICENSE` secret is configured and the `UNITY_CI_ENABLED`
repo variable is `true` (see `.forgejo/workflows/ci.yml`). Player builds are local
by design during the demo phase.

## License note

Third-party CC0 assets are tracked in `Assets/_Project/Art/CREDITS.md`.

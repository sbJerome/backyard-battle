#!/usr/bin/env bash
# Backyard Battle local build driver.
#   scripts/build.sh linux-client | windows-client | linux-server | server-image | test
# Finds the editor version pinned in ProjectSettings/ProjectVersion.txt under ~/Unity/Hub/Editor.
set -euo pipefail

REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
VERSION="$(sed -nE 's/^m_EditorVersion: //p' "$REPO/ProjectSettings/ProjectVersion.txt")"
UNITY="${UNITY_EDITOR:-$HOME/Unity/Hub/Editor/$VERSION/Editor/Unity}"

if [[ ! -x "$UNITY" ]]; then
    echo "Unity $VERSION not found at $UNITY (set UNITY_EDITOR to override)" >&2
    exit 1
fi

run_unity() {
    "$UNITY" -batchmode -quit -projectPath "$REPO" -logFile - "$@"
}

case "${1:-}" in
    linux-client)   run_unity -executeMethod BB.Editor.BuildScripts.BuildLinuxClient ;;
    windows-client) run_unity -executeMethod BB.Editor.BuildScripts.BuildWindowsClient ;;
    linux-server)   run_unity -executeMethod BB.Editor.BuildScripts.BuildLinuxServer ;;
    server-image)
        "$0" linux-server
        podman build -t backyard-battle-server:latest -f "$REPO/ServerBuild/Dockerfile" "$REPO"
        echo "Image built: backyard-battle-server:latest"
        ;;
    test)
        "$UNITY" -batchmode -projectPath "$REPO" -logFile - \
            -runTests -testPlatform EditMode -testResults "$REPO/Builds/test-results-editmode.xml"
        ;;
    *)
        echo "usage: $0 {linux-client|windows-client|linux-server|server-image|test}" >&2
        exit 2
        ;;
esac

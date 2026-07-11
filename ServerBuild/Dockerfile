# Backyard Battle dedicated server.
# Build the player first: scripts/build.sh linux-server  (→ Builds/LinuxServer/)
# Then: podman build -t backyard-battle-server -f ServerBuild/Dockerfile .
FROM ubuntu:22.04

RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates libatomic1 \
    && rm -rf /var/lib/apt/lists/*

RUN useradd --create-home --uid 1000 game
USER game
WORKDIR /app

COPY --chown=game:game Builds/LinuxServer/ /app/

EXPOSE 7777/udp
ENTRYPOINT ["/app/BackyardBattle.x86_64", "-batchmode", "-nographics", "-port", "7777"]

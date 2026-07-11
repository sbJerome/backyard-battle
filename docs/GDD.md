# BACKYARD BATTLE — Game Design Document v0.1

**Engine:** Unity 6 · **Camera:** 2.5D side-view fighter cam over 3D low-poly "toy shelf" art · **Demo target:** 6 simultaneous players, 8 fighters, 3 stages, shared award system

---

## 1. LORE & TONE

### The World: THE GREAT BACKYONDER

Max Finch is eight years old, it's the first week of summer vacation, the Wi-Fi is out, and his mom said "go play outside" in the tone that means it. So he did. And he found a whole civilization.

From bug's-eye view, the Finch family backyard isn't a backyard — it's **The Great Backyonder**, a vast continent of warring kingdoms:

- **The Lawnlands** — endless grass savanna, patrolled by ant legions
- **The Paver Steppes** — the stone highlands where the Gnome King has stood watch since Before The Deck Was Built (2019)
- **The Garden Reaches** — lush tomato jungles, snake territory
- **Mount Grillmore** — the smoking volcano the Big Ones light on Sundays
- **The Chlorine Sea** — the pool. Nobody comes back the same.
- **The Forbidden Zone** — the neighbor's yard, over the fence. (Fighters teased from here are "invaders from beyond the Fence.")

**Why does everything come alive?** Max's imagination, full stop — but the game plays it deadpan-straight. When Max picks up a pebble and a gnome and smacks them together making explosion noises, *we* see an epic slow-mo clash between two legendary warriors. The joke is that the "gods" of this world are a bored kid, a sprinkler timer, and a dog named Biscuit who occasionally walks through the battlefield like a kaiju. In-world lore is written with total gravity ("The Gnome has not blinked in six years. Scholars fear the day he does.") which is the adult-facing humor layer. Kids get slapstick; adults get the deadpan.

**The mythology's magic McGuffin (light-touch):** Legend says whoever holds the **Golden Bottle Cap** — a mythic cap Max buried "for safekeeping" and forgot where — rules the Backyonder. Every tournament is nominally fought for it. Nobody has ever found it. (This gives future modes/seasons an eternal quest hook without needing real lore stakes.)

### Framing Device (drives UI/UX everywhere)

- **Match start:** Max's giant hands descend from the sky and *place* each fighter on the stage (this is the character-select-to-spawn transition). A kid voice mutters commentary: "Okay okay okay... GNOME versus... SNAKE. Ohhh this is gonna be GOOD."
- **Countdown:** Max's voice: "Three... two... one... BACKYARD BATTLE!" with crayon-drawn numbers slamming in.
- **UI language:** Crayon, marker, and graph-paper. Damage percentages look written in smudgy marker. Menus are pages of Max's spiral notebook, complete with doodles, eraser marks, and a grade-school "MAX F." name tag in the corner.
- **KOs:** When a fighter is launched off-screen, Max's hand sometimes snatches them out of the air with a "you're OUT" — or they smack the "camera" (a smudge appears on screen).
- **Pause:** The whole world freezes because Mom called Max in for lunch. Fighters visibly hold their pose, one of them checks a tiny watch.
- **Victory:** Juice-box podium ceremony (see Section 4).
- **Interruptions as flavor, not mechanics (demo):** Rare non-gameplay background events — Biscuit the dog wanders past behind the stage, a lawnmower drones by, Max's older sister yells "MAX, DINNER." These are the world's "weather."

### Tone Rules (write these on the wall)

1. Deadpan epic narration over ridiculous subjects. Never wink twice.
2. No real violence coding — fighters "bonk," get dizzy, get launched. KO'd fighters reappear grumpy, not hurt.
3. Every fighter believes they are the protagonist of the Backyonder.
4. Max is never mean. He's a benevolent, easily-distracted god.

---

## 2. ROSTER — Demo 8

**Core combat model (context for the kits):** Smash-style — damage % increases knockback, KO by ring-out/blast zones, each fighter has 4 specials (Neutral/Side/Up/Down B), a passive **Gimmick**, standard tilts/aerials/smashes, 2–3 stocks default, 6-player free-for-all and teams.

**Archetype spread:** 1 compact heavy, 1 turret heavy-zoner, 2 rushdown rivals, 1 grappler, 1 floaty aerial zoner, 1 trap/defense, 1 chaos trickster. Full coverage, no doubled niche.

---

### 1. CHIP — The Pebble
*"A chip off the old boulder. The old boulder is very proud."*

- **Archetype:** Compact heavy / armored bruiser. Smallest hurtbox in the game with the heaviest weight class — the anti-heavy heavy.
- **Personality:** Stoic. Has one facial expression (painted on by Max in Sharpie). Communicates in single grunts. Secretly sentimental.
- **Specials:**
  - **Neutral B — Skip Shot:** Chip hucks a smaller pebble that *skips* along the ground up to 3 bounces, each bounce a separate hitbox. Chargeable for more skips.
  - **Side B — Rock & Roll:** Tucks into a rolling boulder charge with super armor; slow startup, plows through projectiles. Can roll off ledges into a falling meteor hit.
  - **Up B — Skipping Stone:** Max's hand flicks Chip like skipping a stone — 1–3 air "skips" off invisible water, each redirectable. Unique multi-angle recovery.
  - **Down B — Sediment:** Chip sits down and becomes terrain for 1.5s — full armor, can't move, reflects verticals, allies can stand on him.
- **Gimmick — Dense:** Immune to the first hit of any string every 8 seconds ("Geology" armor icon appears as a crayon shield). Falls very fast; combos on Chip are hard, but so is his recovery if Up B is spent.
- **Strengths:** Nearly un-launchable early, tiny target, punishes hard. **Weaknesses:** Worst air mobility in game, short reach, struggles vs. camping.
- **Taunt:** Does absolutely nothing. A tumbleweed (dandelion fluff) rolls by. **Victory:** Max stacks two smaller pebbles on him — Chip has become a cairn. Grunts once, contentedly.

---

### 2. SARGE — Black Ant
*"Six legs. Zero excuses. Carries 50x her body weight in grudges."*

- **Archetype:** Honest rushdown / all-rounder. The "Mario slot" — tutorial-friendly, high skill ceiling. **Sworn rival: PEPPER.**
- **Personality:** Drill-sergeant discipline, colony-first, physically incapable of retreating. Refers to everyone as "recruit."
- **Specials:**
  - **Neutral B — Crumb Toss:** Lobs a breadcrumb in an arc; on hit it sticks, and Sarge's next dash homes slightly toward the crumb-marked target.
  - **Side B — Column March:** Summons a brief 3-ant conga line that dashes forward as a moving wall (weak multi-hit, great for approach cover).
  - **Up B — Chain of Command:** Ants link into a ladder beneath her — a climbable tether recovery that can also be angled to grab ledge from far.
  - **Down B — Dig In:** Quick burrow; invincible for 12 frames, pops up with a launcher. Her anti-air/combo starter.
- **Gimmick — Colony Discipline:** Perfect-shield 3 attacks and Sarge gains "Formation" — 5 seconds of +15% damage. Rewards defense feeding offense.
- **Strengths:** Best dash-dance, safe pressure, great frame data. **Weaknesses:** Light, no kill-move without a read, stubby range.
- **Taunt:** Does one-armed... one-*legged* pushups, counting off. **Victory:** Salutes the sky (Max), then orders the losers to drop and give her twenty.

---

### 3. PEPPER — Red Fire Ant
*"Sarge's ex-squadmate. It's a long story. It involves a picnic."*

- **Archetype:** Aggressive rushdown / burn-DoT bully. Faster and dirtier than Sarge; the mirror that fights nothing like the mirror. **Sworn rival: SARGE.** (In-lore: they were on the same crumb-heist crew until Pepper ate the score. Every red-vs-black match triggers unique intro dialogue and a crayon "RIVALRY!!" splash.)
- **Personality:** Chaotic little arsonist energy, zero discipline, all appetite. Laughs at her own jokes.
- **Specials:**
  - **Neutral B — Spice Spit:** Short-range venom glob applying **Sizzle** (burn DoT, 1%/s for 4s, stacks to 3). Low damage now, tax later.
  - **Side B — Fire Line:** Blazing dash that leaves a brief flame trail on the ground; crossing it refreshes Sizzle on enemies. Stage-control rushdown.
  - **Up B — Eruption:** Bursts upward from a mini anthill of flame — strong out-of-shield kill move, but stubby recovery distance.
  - **Down B — Swarm Bite:** Command-grab pounce (short range): latches on, mashes bites, applies max Sizzle stacks, then kicks off. Her grappler flavor.
- **Gimmick — Boiling Point:** KO potential scales with victim's Sizzle stacks — knockback +8% per stack. Pepper's whole gameplan: tag everyone, then cash out.
- **Strengths:** Fastest ground speed, damage snowball, terrifying in 6-player chaos (DoT everywhere). **Weaknesses:** Lightest fighter in demo, poor range, weak without Sizzle setup, exploitable recovery.
- **Taunt:** Points at opponent, then draws a tiny line across her neck... then giggles and waves like it was a joke. It was not a joke. **Victory:** Roasting a crumb marshmallow-style over her own flame trail.

---

### 4. NOODLE — Garden Snake
*"He's not venomous. He's not even mad. He just wants a hug. A really tight one."*

- **Archetype:** Grappler / long-reach whip hybrid. Solves the classic grappler problem (getting in) with the best disjointed normals in the game.
- **Personality:** Terminally chill surfer-bro. Calls everyone "bud." Genuinely confused why everyone screams when he appears.
- **Specials:**
  - **Neutral B — Coil:** Winds up into a coil (chargeable, holdable). Release = spring-loaded lunging strike; range and damage scale with charge. His threat-stance.
  - **Side B — Big Hug (command grab):** Lunging constrict. On grab: 3 squeeze pulses, then throw in any direction (stick-angled). In 6-player, other fighters can hit the hug to break it — risk/reward in crowds.
  - **Up B — Periscope:** Extends body straight up like a stretching cartoon — hits above, then he "falls over" in a chosen direction as a slow-fall glide. Recovers by draping onto ledges.
  - **Down B — Shed Skin:** Leaves a decoy skin that flops convincingly; Noodle turns semi-transparent for 1s. One use per stock airborne. Escape tool.
- **Gimmick — Long Boy:** His hurtbox is genuinely his body — the tail segments take reduced damage but CAN be hit. Positioning his own tail is a skill (curl it behind you at ledge).
- **Strengths:** Range king, nightmarish ledge-trapping, throw kills. **Weaknesses:** Huge total hurtbox, slow turnaround, weak scramble/panic options.
- **Taunt:** Ties himself in a bow. **Victory:** Coiled in a warm patch of sun, sunglasses on (where did he get sunglasses), gives a tongue-flick "tsss" of approval.

---

### 5. GNORMAN THE UNBLINKING — Garden Gnome
*"He has guarded this yard for seven years. He has seen things. He tells no one."*

- **Archetype:** Super-heavyweight zoner / turret. The immovable object with a fishing pole.
- **Personality:** Speaks in grave prophecies about mundane events ("The sprinklers... come at dawn."). The most self-serious being in the Backyonder, which makes him the funniest.
- **Specials:**
  - **Neutral B — Gnome Fishing:** Casts his fishing line at 3 selectable angles. Hooked enemies are yanked toward him into his (very scary) up-close smashes. His whole gameplan.
  - **Side B — Garden Warding:** Plants a tiny ceramic mushroom turret (max 2) that pulses a slow damage aura. Enemies must approach through his garden.
  - **Up B — Pedestal Rise:** A stone pedestal erupts under him, launching him up (hits on the way); the pedestal remains as a temporary platform for 4s. Recovery that changes the stage.
  - **Down B — Ceramic Stance:** Becomes a literal inanimate gnome. Full armor, reflects projectiles, but a fully-charged smash attack SHATTERS the stance and stuns him (crack VFX). Counterplay is committing hard.
- **Gimmick — Load-Bearing Lawn Ornament:** Cannot be flinched by attacks under 5% damage while grounded. Walk speed is the slowest in the game — his dash is a comical waddle-hop.
- **Strengths:** Massive kill power, area denial, unshakeable. **Weaknesses:** Slowest fighter, juggle food once airborne, turrets punishable on placement.
- **Taunt:** Blinks. Once. The screen does a record-scratch freeze-frame with the caption "he blinked." **Victory:** Stands perfectly still on the podium while a butterfly lands on his hat. Whispers: "The yard... is safe. For now."

---

### 6. DANDY LION — Dandelion
*"He believes he is a lion. Nobody has the heart to tell him. Also — he kind of is?"*

- **Archetype:** Floaty aerial zoner. Lowest weight, highest air control, death-by-a-thousand-seeds.
- **Personality:** Delusionally brave puffball with a tiny drawn-on fierce face. ROARS (it sounds like "pff").
- **Specials:**
  - **Neutral B — Seed Shot:** Rapid-fire drifting seed puffs (up to 5); they float and linger, curving with his movement. Screen-clutter zoning.
  - **Side B — Pollen Gust:** A windbox breath that pushes enemies and *carries his lingering seeds* with it — a projectile that relocates other projectiles. Signature tech.
  - **Up B — Make a Wish:** Max's face appears in the sky background and *blows* — Dandy is carried upward on the breeze in a long steerable float. Best recovery in the demo.
  - **Down B — Deep Roots:** Anchors to the ground: immune to windboxes/grabs and gains a counterattack posture, but locked in place. His answer to being the lightest thing alive.
- **Gimmick — Going to Seed:** At high damage (100%+), Dandy visually goes from yellow flower to white puffball — he takes +10% knockback but his seeds double in count. High-percent Dandy is desperate AND dangerous.
- **Strengths:** Air superiority, edge-guarding menace, controls space in FFAs. **Weaknesses:** Dies off the top absurdly early, no burst kill option, folds to fast rushdown in his face.
- **Taunt:** Tiny fierce ROAR ("pff") with a crayon lion-mane briefly drawn around his head. **Victory:** Poses majestically on a "pride rock" (a garden hose nozzle) while dramatic savanna music plays on a kazoo.

---

### 7. TURBO — Garden Snail
*"He didn't choose the slow life. Yes he did. He loves it."*

- **Archetype:** Trap/defense tank. Wins by making the stage his house. Literally brings his house.
- **Personality:** Unbothered king of self-care. Talks slowly enough that his voice lines get cut off by the match starting.
- **Specials:**
  - **Neutral B — Slime Time:** Sprays a patch of slime on the ground (max 3 patches). Enemies slip/slide on it with reduced traction; TURBO moves *faster* on his own slime (his only fast movement).
  - **Side B — Shell Bowling:** Retreats into shell and launches as a ricocheting projectile — bounces off walls and slopes, gains speed on slime patches. He IS the projectile.
  - **Down B — Home Sweet Home:** Parks the shell as a stationary bunker and *steps out of it* — soft-bodied Turbo is faster but takes +30% damage until he returns. The shell blocks projectiles and can be repositioned. Big-brain risk toggle.
  - **Up B — Bubble Lift:** Blows a slime bubble and rides it upward; the bubble pops into a slow-fall slime rain (mini slick where it lands).
- **Gimmick — Shell Priority:** Attacks from behind (shell side) deal 25% reduced damage and never flinch him. Facing matters against Turbo — surround him or suffer.
- **Strengths:** Best stage control, absurd endurance, punishes impatience, FFA survivor. **Weaknesses:** Genuinely slow, low damage-per-hit, can be out-camped, timer games hurt.
- **Taunt:** Puts on a tiny nightcap and takes a 1-second nap. Fully vulnerable. Disrespect incarnate. **Victory:** Crosses a crayon-drawn finish line in slow motion, both arms up, confetti falling faster than he's moving.

---

### 8. WIENER — Squeaky Dog Toy (rubber hot dog)
*"Biscuit's favorite toy. Chewed by a god. Fears nothing."*

- **Archetype:** Chaos trickster. Rubber-physics, sound-based hitboxes, the wildcard character built for 6-player pandemonium.
- **Personality:** Squeaks with the confidence of a war veteran. Has bite-mark "scars" he shows off. Every squeak is subtitled with grave dialogue ("*squeak* — 'I've seen the inside of the couch, kid. You wouldn't last a minute.'").
- **Specials:**
  - **Neutral B — SQUEAK:** Self-squeeze emitting an expanding ring of sound (small damage, big flinch, reflects projectiles). Mash to squeak repeatedly with diminishing size. In-lore this summons dread in all who hear it.
  - **Side B — Fetch Yourself:** Hurls himself flat like a thrown toy, tumbling with random bounce angles off surfaces — deliberately semi-unpredictable (seeded random from 3 bounce patterns so it's learnable-ish).
  - **Up B — Squeeze Jump:** Squeezes himself until he pops upward with a huge SQUONK — height scales with how long the button is held; full hold adds a launch hitbox and a screen-shake squeak.
  - **Down B — Play Dead:** Flops lifeless. If struck within 0.5s, attacker bounces off his rubber body (counter) and Wiener springs up with a rising headbutt. If not struck, he just lies there. Commit to the bit.
- **Gimmick — Rubber Body:** Takes 15% less knockback but 10% more damage — and *bounces* off stage surfaces when launched instead of tumbling, sometimes bouncing back INTO the fight. Surviving as Wiener is a physics adventure.
- **Strengths:** Unkillable-feeling, disruption king, punishes crowds, hilarious. **Weaknesses:** Lowest raw kill power, imprecise (that's the price of the bounce kit), predictable once downloaded.
- **Taunt:** One long, slow, deflating squeak while maintaining eye contact. **Victory:** Biscuit's giant nose enters frame and sniffs him; Wiener squeaks once, triumphantly, and gets carried off in dog jaws like a hero going home.

---

### COMING SOON — 8 teaser slots (silhouettes on select screen)

1. **DUCHESS VESPA** — Wasp. Aerial glass-cannon assassin; aristocrat who considers everyone beneath her, literally.
2. **BIG SLAB** — Concrete paver. The colossal super-heavy; moves twice a minute, ends stocks when he does.
3. **SPRINX** — Pop-up sprinkler head. Stage-control zoner who redraws the battlefield in water pressure.
4. **DOUG** — Earthworm. Subterranean trickster-grappler; two ends, both of them the front, attacks from underneath.
5. **FLIP** — Lost left flip-flop. Grappler/heavy whose slap is feared across every yard in the hemisphere.
6. **GLIMMER** — Firefly. Dusk-only zoner-assassin; the screen dims when she taunts.
7. **SIR CONE-A-LOT** — Pinecone. Spiky counter-knight; opens and closes like armor plating.
8. **MINGO** — Plastic lawn flamingo. Rangy trickster on one leg; the other leg is the weapon.

---

## 3. STAGES — Demo 3

### Stage A — THE SANDBOX (neutral / tournament stage)

*"The Colosseum of the Backyonder. Countless battles. One lost dump truck."*

- **Role:** Flat competitive stage. This is the ranked/training default.
- **Layout:** A wooden-framed sandbox viewed side-on. Main floor is packed sand (flat, ~2.5 platform-widths wide). One low soft platform on each side: a **plastic shovel** laid across a bucket (left) and a **ruler** bridging the frame corner (right) — both static, drop-through. The wooden frame edges form clean, grabbable ledges. Blast zones generous and symmetrical.
- **Interactive hazards:** None during standard play (tournament mode toggle = pure). **Casual toggle adds one:** every ~90s a plastic **dump truck** slowly drives across the background lane behind the stage — purely cosmetic in tournament, but in casual it dumps a sand pile stage-left OR right that forms a temporary walkable ramp for 20s (subtle terrain variance, no damage).
- **Background storytelling:** Half-buried army men "spectating" from the sand; a moat dug around a sandcastle in the far background (the "Old Capital"); Max's knee occasionally visible at frame edge, giant and out of focus; a juice box the size of a building.
- **Music:** Playground-march kazoo-and-recorder orchestra played completely straight, like a Roman epic scored on elementary school instruments.
- **Build notes:** Symmetric collision, zero moving colliders in tournament mode = ideal netcode/perf baseline stage; ship this one first.

### Stage B — MOUNT GRILLMORE (hazard-heavy)

*"The volcano wakes on Sundays. Today is Sunday."*

- **Role:** The chaos stage. Item-mode energy even with items off.
- **Layout:** Fought across the top of a kettle grill. Main surface: the **grill grate** — a wide platform whose gaps are visual only (solid collision) EXCEPT two marked "flare gaps" (see hazards). Left side: the **thermometer dial** juts out as a small circular platform. Right side: a **spatula** left leaning against the grill = a slanted hard platform. Above center: the **grill lid**, held open, acting as a high ceiling-platform players can stand on (dome = slippery slopes at edges).
- **Hazards (telegraphed, on a readable cycle):**
  1. **Flare-Ups (~every 25s):** 2 of 5 grate zones glow orange for 2s (crayon "!!" warning) then erupt in a flame pillar — big vertical launch + Sizzle burn. Zones chosen semi-randomly, never all adjacent.
  2. **Kebab Skewer (~every 45s):** A skewer slides horizontally across the mid-level from a random side — slow, blockable, rideable for 3s before it exits (moving platform + threat in one).
  3. **The Lid (once per match, ~2:00 mark):** Max's hand closes the lid for 8 seconds — ceiling drops, dome becomes the new main floor, everyone fights "on top of the volcano" while smoke vents from the sides (visual only). Massive telegraph: kid voice says "and THEN the volcano ERUPTS—" first.
- **Background storytelling:** Dad's tongs resting like an ancient siege weapon; a burnt hot dog rolled off into the drip tray below, mourned by a small circle of ants; oven mitt draped like a fallen banner; "GRILL CHAMPION" apron visible on the deck rail.
- **Music:** Surf-rock with sizzle percussion (actual sizzle foley as the hi-hat), doubling tempo during Lid phase.
- **Build notes:** All hazards on global timers broadcast from stage controller (deterministic for future rollback); flare warning = 2.0s always, tune damage not warning time.

### Stage C — THE TOMATO TRELLIS (platform-heavy)

*"The vertical jungle. The tomatoes are ripe. The tomatoes are load-bearing."*

- **Role:** The vertical/aerial stage. Juggle characters feast; movement skill showcase.
- **Layout:** A garden lattice trellis viewed side-on, 3 tiers tall. Ground level: a **terracotta pot rim** and soil bed (main floor, modest width, walls below = wall-jump territory). Mid tier: two **lattice crossbar** platforms (drop-through) at staggered heights, connected by a **vine** that acts as a climbable soft platform (stand on it, it sags slightly — 20cm dip under 2+ fighters, pure feel/vfx). Top tier: one narrow **wooden stake** summit platform. Left edge: a **tomato cluster** — 3 tomatoes that function as platforms with a catch (below). A watering can hangs top-right as decoration/landmark.
- **Hazards (soft, terrain-based rather than damage-based — contrast with Grillmore):**
  1. **Ripe Tomatoes:** Each tomato platform supports ~4s of cumulative standing before it *drops* (stem snaps, telegraphed by wobble + darker red) — falls as a soft projectile (splat = brief slippery puddle on the floor below), regrows in 20s. Platforms with a fuse.
  2. **The Sprinkler Pass (~every 60s):** Off-screen sprinkler arcs a water line across the stage left-to-right over 4s — a moving windbox that pushes airborne fighters (no damage), waters the soil, and instantly regrows any fallen tomatoes. Readable by sound (chk-chk-chk-chk) before arrival.
  3. **Bottom hazard geometry:** The pot interior below the rim is a pit with wall-jumpable sides — recoverable but scary. No damage floor; falling out the bottom of the pot area = blast zone.
- **Background storytelling:** A "MAX'S GARDEN — DO NOT TUCH" sign in crayon; one suspiciously bitten tomato (Biscuit-sized bite); a ladybug commuting up and down the far lattice like an elevator NPC; the gnome's empty pedestal in the background *if Gnorman is in the match* (detail flex).
- **Music:** Gentle acoustic "secret garden" theme that adds a banjo layer for every tier the current damage-leader climbs (dynamic vertical mixing — subtle, but it slaps).
- **Build notes:** Tomato timers are per-platform local state; sprinkler is global timer. Vine sag is animation-only (collision stays fixed) to keep physics deterministic.

### Coming Soon — 10 stage one-liners

1. **The Chlorine Sea (Pool):** Fight on floaties and a drifting pool noodle; the filter is a whirlpool blast zone.
2. **Pergola Heights:** String-light tightropes and rafter platforms above a long, long fall to the patio.
3. **The Gazebo Rotunda:** Elegant symmetrical royal-court stage; the ceiling fan is both platform carousel and hazard.
4. **Porch Step Ascent:** Auto-scrolling climb up the back steps before the "tide" (a spilled juice flood) catches you.
5. **The Shed of Legends:** Fight across a pegboard of hanging tools; things fall. Many things fall.
6. **Birdbath Falls:** Tiered fountain platforms with slippery rims — and periodic visits from The Bird.
7. **Fort Doghouse:** Biscuit is asleep inside. The stage's only rule: do not wake Biscuit. (You will wake Biscuit.)
8. **The Gutter Run:** Rain-gutter river stage; flowing water current, floating leaf-boat platforms, downspout drop.
9. **The Compost Kingdom:** Rot-punk terraced mound; steam vents, fruit-peel slides, extremely proud worm citizens.
10. **The Forbidden Fence:** Fight ON the fence top between yards — pure tightrope stage, wind gusts, neighbor's cat watching.

---

## 4. AWARD / PROGRESSION SYSTEM — "THE SHOEBOX" (shared across all modes)

### Currency

- **Bottle Caps ("Caps")** — primary soft currency. Earned every match, everywhere, in every mode, forever. Spent on cosmetics in **Max's Trade Blanket** (the shop — a picnic blanket where items are laid out).
- **Shinies** — rare achievement collectible (a marble, a foil wrapper, a lucky penny, a piece of sea glass — each Shiny is a unique named object, not a number). Earned ONLY from Brag completions and rank milestones, never from grinding matches. Spent on premium-tier cosmetics and podium flair. Fixed catalog = naturally scarce, no economy inflation.
- **No purchasable currency in demo.** Design the wallet as a signed local ledger anyway (below).

### Match Rewards (every mode pays into the same wallet)

- Base payout: participation Caps + placement bonus + match-length scaling (anti-quit, anti-idle: minimum on-stage activity required).
- **Style Stickers:** end-of-match bonus tags worth Caps, awarded per-player, always at least one per player, comedy-first: "Longest Airtime," "Most Polite (zero taunts)," "Rivalry Winner" (beat your ant rival), "Slept Through It" (Turbo nap taunt landed), "Gravity's Favorite" (most SDs — a pity/joke reward). Stickers physically slap onto your results card with a *thwap*.
- First-win-of-the-day: a **Juice Box** bonus (big Cap multiplier).

### Trophies & Badges — "BRAGGING RIGHTS"

- Achievement system = **Brags**, organized into crayon "merit badge" pages in Max's notebook. Three tiers: **Doodle** (bronze/easy), **Marker** (silver), **Gold Star** (hard, awards a Shiny).
- Categories: per-fighter mastery (win X, land signature move Y times, character-specific stunts like "KO someone with Chip's third skip"), per-stage ("survive a full Lid phase at 100%+"), per-mode, and cross-mode meta-Brags ("win a match in all 3 modes with the same fighter" — designed now, unlockable later).
- Fighter mastery track (per character, 10 levels): unlocks that fighter's title cards, victory-pose variants, and their **Gold Star skin** at max.

### Cosmetics (all cosmetic, zero gameplay)

- **Hats & Accessories:** Gnorman's hat catalog is the flagship (traffic-cone hat, acorn cap, birthday hat). Every fighter has an accessory anchor point spec'd from day one (Chip: googly eyes & paint jobs; Noodle: tiny scarves at intervals along his body; Sarge: medals; Wiener: band-aids and chew-mark patterns; Dandy: mane styles; Turbo: shell decals; Pepper: sunglasses).
- **Paints/Skins:** "Max painted me" variants — visible brushstrokes, deliberately kid-craft (glitter glue tier = Shiny purchases).
- **Podium Flair:** custom victory podium props, confetti types, victory kazoo fanfares.
- **Profile cosmetics:** notebook-page backgrounds, name-tag sticker fonts, Shoebox diorama decorations.

### Profile & Rank — "THE PECKING ORDER"

- Player profile = **your Shoebox**: a 3D diorama shoebox (literally how kids store treasures) displaying your Shinies, top Brags, main fighter figurine in chosen cosmetics, and rank card. Viewable by other players in lobbies.
- Rank ladder (account-level, XP from all modes): **Ant Food → Grub → Yard Squirt → Fence Hopper → Porch Boss → Deck Captain → Lawn Legend → Monarch of the Backyonder.** Seasonal prestige later ("Summer of '26" ring around the rank icon); demo ships the ladder without seasons.
- Rank = progression prestige only in demo; competitive MMR is a separate hidden value added when online ships (never conflate the two).

### The Award Ceremony (post-match, every mode, skippable but charming)

1. Max's hands place the **juice-box podium** (1st: full juice box, 2nd: capri-sun style pouch, 3rd: the sad squeezed empty one).
2. Winners do victory poses; losers stand at the side doing personality-appropriate sulks (Gnorman is simply placed facing the fence).
3. **Macaroni Medal:** Max's hand lowers a macaroni-and-yarn medal onto the winner. Caps rain like tossed confetti; Style Stickers thwap onto each results card.
4. **Polaroid:** a snapshot of the podium (with stickers, damage stats doodled on it) saves to the **Scrapbook** — your local match history is literally a photo album. Shareable image export = free marketing.

### Technical design (local-first, sync-ready) — build checklist

- Single `PlayerProfile` save: JSON, locally GUID-keyed, checksummed.
- **Append-only event ledger** (`CapsEarned`, `BragUnlocked`, `CosmeticPurchased`, `XpGained`) with timestamps + mode/match IDs; current wallet/rank = fold over ledger. Later server sync = ledger replay + server-side validation, zero migration pain.
- All reward *rules* in ScriptableObject data tables (payout curves, sticker conditions, Brag definitions) — modes emit gameplay events; the award system is a pure subscriber. **No mode ever writes Caps directly.** This is the contract that makes all three modes share one system.
- Cosmetic catalog = addressable content IDs; ownership is ledger entries referencing IDs (DLC/season drops need no save changes).

---

## 5. GAME FEEL PILLARS

1. **Chaos is fair.** Every hazard telegraphs (2s minimum, audio + visual), every random has a readable pattern, every death is your fault in hindsight. 6-player pandemonium must never feel like the game rolled dice at you — it should feel like you laughed too long at someone else's KO.
2. **Readable at 6 players.** Silhouette-first character design (each fighter identifiable in solid black), team-color rim lighting, hit-VFX budget caps per screen, camera that frames the fight not the stage. If a playtest observer can't say who hit whom, cut VFX before cutting players.
3. **Every object is a personality.** No fighter, stage prop, or menu item is generic. If a pebble can have a soul, so can the pause menu. The bar: every element should be screenshot-captionable.
4. **Toy physics, arcade truth.** It should *look* like toys clattering on a shelf (bounce, wobble, squeak, secondary motion everywhere) but *feel* frame-precise underneath. Animation lies; hitboxes never do. Cosmetic physics never touches gameplay collision.
5. **A seven-year-old and their parent lose to each other.** Every fighter has a one-button-satisfying floor and a lab-monster ceiling (Chip's armor forgives; Chip's skip-angles reward). Comeback texture (Dandy's seed mode, Wiener's bounce-backs) without comeback handouts.

---

## 6. MODE ROADMAP

**Rumble Pile (physics arena brawler, Gang Beasts-like):** Same roster, same skeletons, swapped controller — fighters become full active-ragdoll toys grabbing, shoving, and dragging each other off hazards. Fighter identity carries through physical stats already defined in their ScriptableObjects (Chip's density, Noodle's length, Wiener's rubber restitution, Gnorman's ceramic brittleness become the *comedy* physics parameters), so kits don't port but personalities do — Noodle literally being a rope in this mode is the trailer moment. Stages reuse demo geometry with hazard timers cranked and blast zones replaced by physical drops (Grillmore's lid becomes a crush hazard). All matches emit the same reward events into the Shoebox — mode-specific Brags ("throw someone into the pool filter") slot into existing Brag pages, and Rumble Pile earns the same Caps, rank XP, and cosmetics (cosmetics are already physics-safe because they're anchor-point props).

**Recess (party minigame collection, Mario Party-like):** Minigames are framed as "Max's other games" — the same toy fighters roped into whatever Max invents next (pebble bowling on the pavers, sprinkler-dodging, ant-line racing down the gutter, a Grillmore hot-potato with the burnt hot dog). Each minigame is a scene reusing existing stage environments as backdrops and the fighters' locomotion/emote sets — no new character tech, just new mini-rules — which is why the roster's animation set must include generic run/jump/carry/celebrate from day one (build checklist item). Recess is the progression engine's volume knob: short sessions, frequent Style Stickers, high Cap flow at lower per-match value, and it's where seasonal Brag pages live. The Scrapbook/Polaroid system makes Recess results shareable, and the juice-box podium closes every session so all three modes end on the same beat.

---

## 7. NAME BIBLE (canonical list)

| Thing | Name |
|---|---|
| Game | **Backyard Battle** |
| World | **The Great Backyonder** |
| The kid | **Max Finch** (dog: **Biscuit**; unseen sister: **Nat**) |
| Mythic McGuffin | **The Golden Bottle Cap** |
| Demo fighters | **Chip** (pebble) · **Sarge** (black ant) · **Pepper** (red ant) · **Noodle** (garden snake) · **Gnorman the Unblinking** (gnome) · **Dandy Lion** (dandelion) · **Turbo** (snail) · **Wiener** (squeaky toy) |
| Coming soon | Duchess Vespa · Big Slab · Sprinx · Doug · Flip · Glimmer · Sir Cone-a-Lot · Mingo |
| Demo stages | **The Sandbox** · **Mount Grillmore** · **The Tomato Trellis** |
| Currencies | **Bottle Caps** (soft) · **Shinies** (rare, named collectibles) |
| Shop | **Max's Trade Blanket** |
| Achievements | **Bragging Rights** ("Brags": Doodle / Marker / Gold Star) |
| Match bonuses | **Style Stickers** · **Juice Box** (first win of day) |
| Profile | **The Shoebox** (match history: **The Scrapbook**) |
| Rank ladder | **The Pecking Order** (Ant Food → Monarch of the Backyonder) |
| Victory moment | **Juice-Box Podium** + **Macaroni Medal** + **Polaroid** |
| Future modes | **Rumble Pile** (physics brawler) · **Recess** (minigames) |

# CS461 (First Term Requirements)

## REQ‑001 (Must) — Player Movement & Jump
The system should allow the player to move left/right and jump via keyboard (and later controller).
AC:
Given the player is on ground, when ←/→ (A/D) is held, then horizontal motion occurs with visible acceleration; and when Jump is pressed, then the player leaves ground and lands with no clip‑through.

## REQ‑002 (Must) — Basic Attack
The system shall perform a basic attack that plays an attack animation and applies damage with cooldown.
AC:
Given that the warrior is idle or moving, when Attack is pressed and cooldown elapsed, then an animation plays and an enemy within the hitbox loses HP; when cooldown did not elapse, then no new attack triggers.

## REQ‑003 (Must) — Enemy Prototype
The system shall include at least one enemy type with idle/patrol and contact or hitbox damage.
AC:
Given an enemy in patrol state, when the player enters its range, then the enemy damages on hit or takes damage from the player’s attack; and when enemy HP ≤ 0, then it despawns or plays a death animation.

## REQ‑005 (Must) — 2D Physics & Collisions
The system shall use Unity 2D physics for stable ground detection, jump arcs, and collision without trap falls on the demo path.
AC:
Given the demo path, when the player traverses platforms, then no unintended clip‑through or infinite fall occurs; and when colliding with ground/hazards, then expected collision responses occur.

## REQ‑007 (Must) — Demo Path
The system shall provide a 1 to 2-minute guided path from spawn to endpoint with at least one jump gap and one enemy encounter.
AC:
Given a fresh run, when a tester follows the main path, then the level can be completed without designer intervention in ≤5 minutes.

## REQ‑009 (Must) — Build & Traceability
The system shall produce a demo build and report the commit hash in the Sprint report; claims link to issues/PRs/board.
AC:
Given the Sprint 2 report, when a reviewer opens links, then the build is downloadable/runnable and each claim traces to a GitHub issue/PR.

## REQ‑011 (Must) — Stability (NFR)
The system shall exhibit zero crashes across 30 minutes of cumulative play on the demo path.
AC:
Given three back‑to‑back playthroughs, when the demo path is completed, then no app crash occurs, and no blocker defect prevents completion.

## REQ‑013 (Must) — Licensing & Attribution
The system shall track third‑party assets in /docs/ATTRIBUTIONS.md with license (e.g., CC0/CC‑BY) and source link; code remains All Rights Reserved until partner confirms otherwise in LICENSE.
AC:
Given a build uses third‑party assets, when a reviewer opens the attribution doc, then each asset lists name, source URL, and license; and when the repo is viewed, then LICENSE reflects the current policy and commit history.

## REQ‑004 (Should) — HP UI
The system shall display player HP in a minimal HUD that updates on damage.

## REQ‑006 (Should) — Follow Camera
The system should keep the player in frame using a follow camera (e.g., Cinemachine) tuned for comfortable motion.

## REQ‑008 (Should) — Title/Start
The system shall show a start screen with Start and Quit options before loading the level.

## REQ‑010 (Should, NFR) — Performance
The system shall sustain a good FPS on the reference laptop across the demo path.

## REQ‑012 (Should) — Controls Visibility / Basic Accessibility
The system should present a control scheme screen or overlay (jump/attack/move) accessible from Start or Pause.

## REQ‑014 (Could) — Controller Input
The system shall support a common controller for move/jump/attack where available.
NFR coverage (subset). REQ‑011 Stability, REQ‑010 Performance, REQ‑009 Traceability/Maintainability, REQ‑013 Licensing/Compliance, REQ‑012 Basic Accessibility.


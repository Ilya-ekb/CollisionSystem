# Collision System (UPM)

**Purpose:** unified collision registration (3D/2D) with per-physics-step deduplication and configurable conditions (ScriptableObject) for each participant.

## Installation
1. In Unity: *Window → Package Manager → + → Add package from disk...* (or Git URL)
2. Select the package folder.

## Quick Start
1. Add a `CollisionManager` to the scene.
2. Add `CollisionParticipant` + `CollisionReporter3D/2D` to the objects.
3. Create SO-based conditions and assign them to the `CollisionParticipant`.
4. (Tests) Open the **Test Runner** and run the package's PlayMode tests.  
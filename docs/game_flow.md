# Game Flow

This document explains how game state is managed in **steel-clash**.

## Game Start

- `GameManager` hides UI panels.
- Player is controllable.
- Time flows normally.

## Player Death

- Triggered by `PlayerStatus.health <= 0`
- Calls `PlayerDied()` in `GameManager`.
- Shows Game Over panel and freezes time.

## Boss Defeated

- Triggered when Boss HP = 0
- Calls `BossDied()` in `GameManager`
- Shows Victory panel and freezes time.

## Boss Spawn

- Triggered when player is within 30 units.
- Plays animation then enables AI.

## Control Locking

- On death, control is disabled and player is hidden.

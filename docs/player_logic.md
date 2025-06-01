# Player Logic

This document outlines all the core player mechanics and implementation logic in **steel-clash**, based on the actual behavior defined in `PlayerController.cs`, `PlayerStatus.cs`, and animation usage.

---

## Player Stats

- **Max Health:** `100.0f`
- **Default Attack Damage:** `5.0f`
- **Movement Speed:** `15.0f`
- **Run Multiplier:** ×3 when `Shift` is held
- **Jump Height:** `1 unit`
- **Gravity:** `-9.81f`
- **Attack Range:** `6.0f`

---

## Input & Controls

| Action       | Input                           |
| ------------ | ------------------------------- |
| Move         | WASD or Arrow Keys              |
| Run          | Hold `Left Shift`               |
| Jump         | `Spacebar`                      |
| Light Attack | `Left Click`                    |
| Kick         | `K` key                         |
| Block        | Hold `Left Alt`                 |
| Dodge        | `Q` key _(not implemented yet)_ |

---

## Core Systems

### 1. Movement & Gravity

- Uses Unity's `CharacterController` for all movement.
- Forward movement from `Vertical` axis input (`W/S`, `Up/Down`).
- Run speed is 3× normal when `Left Shift` is held.
- Rotation uses `Horizontal` axis input (`A/D`, `Left/Right`).
- Gravity applied using `yVelocity`, accumulates over time.
- Jump calculated via:  
  `Mathf.Sqrt(jumpHeight * -2 * gravity)`

---

### 2. Combat Logic

#### Light Attack

- Triggered by **Left Click**
- Plays `"Attack"` animation trigger
- Calls `FindClosest()` to get nearest enemy within range
- Applies `attackValue` to `AIStatus` or `BossStatus` via `ApplyDamage()`

#### Kick

- Triggered by **`K` key**
- Plays `"Kick"` animation
- _No damage applied yet_

#### Blocking

- Triggered by holding **Left Alt**
- Sets:
  - `isBlocking` animation boolean
  - `PlayerStatus.IsBlocking` logic flag
- Reduces or negates incoming damage

---

### 3. Damage & Health System

- Managed in `PlayerStatus.cs`
- Starts at 100 HP
- `ApplyDamage()` subtracts health and checks death condition

#### On Death:

- Sets `isDead = true`
- Triggers `"isDead"` animation
- Calls `GameManager.PlayerDied()`
- Disables control + hides character mesh

#### Blocking Effects:

- Melee damage reduced by 50%
- Ranged arrows = 0 damage if blocking

---

### 4. Boost System

- Triggered by killing **ArcherMcStabby**
- Doubles player’s attack for 60 seconds
- Managed by coroutine `AttackBoostRoutine(2f, 60f)`
- Controlled by `isBoosted` flag
- Restores original attack after duration

---

### 5. Health Pickups

- **Health Pack Pickup:** +5 HP when collected from environment
- **Sir Stabs-a-Lot Death:** +15 HP via `AddHealthAfterEnemyDeath()` coroutine

---

### 6. UI Display

- Player’s numeric health shown in top-right via `OnGUI()`
- Pulls from `PlayerStatus.GetHealth()`

---

### 7. Animation State Integration

Controlled via hashed parameters:

- `isWalking`: True if player is moving forward
- `isRunning`: True if running + moving
- `Attack` trigger: Left click
- `Kick` trigger: `K` key
- `isBlocking`: While `Left Alt` held
- `isDead`: Triggered on death

---

### 8. Enemy Targeting Logic

- `FindClosest()`:
  - Searches for all enemies tagged `"Enemy"`
  - Skips null, dead, or invalid targets
  - Finds nearest enemy within `attackDistance`
  - Handles both `AIStatus` (normal enemies) and `BossStatus` (boss)

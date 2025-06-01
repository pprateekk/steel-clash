# Enemy Logic

This document details the logic, attack behaviors, AI perception, and health/death management for all enemy types in **steel-clash**. Each enemy is a modular GameObject controlled by shared AI scripts and specialized attack components.

---

## General AI System (`AIController.cs`)

### Core Concepts

- All non-boss enemies use `AIController.cs`
- Player detection based on:
  - **Field of View (FOV):** 180° by default
  - **Sight Radius:** 30 units
- Movement handled using Unity’s `NavMeshAgent`

### Behavior Flow

1. **Idle:** Player is out of range
2. **Chasing:** Player in sight but outside attack range
3. **Attacking:** Player within attack range
4. **Dead:** `AIStatus.isAlive()` returns `false`

### Player Detection Logic (`IsPlayerInRange()`)

- Checks:
  - Distance to player
  - Directional angle between enemy forward vector and player position
  - If both pass, enemy will react

---

## Enemy Types & Behavior

### 1. Wobbly Steve (`PunchAttack.cs`)

| Stat        | Value       |
| ----------- | ----------- |
| Attack Type | Melee punch |
| Damage      | 5 HP        |
| Cooldown    | 1.0 s       |

- Uses `PunchAttack.cs`
- Plays `"Attack"` animation
- If player is blocking → damage is halved

---

### 2. Sir Stabs-a-Lot (`SwordAttack.cs`)

| Stat        | Value            |
| ----------- | ---------------- |
| Attack Type | Sword strike     |
| Damage      | 10 HP            |
| Cooldown    | 1.0 s            |
| On Death    | +15 HP to player |

- Uses `SwordAttack.cs`
- Same flow as `PunchAttack`, with increased damage
- Grants health bonus to player on death via coroutine

---

### 3. Archer McStabby (`ArcherAttack.cs`)

| Stat        | Value                                 |
| ----------- | ------------------------------------- |
| Attack Type | Ranged arrows                         |
| Damage      | 5 HP                                  |
| Cooldown    | 1.0 s                                 |
| On Death    | Player gets attack boost (×2 for 60s) |

- Uses `ArcherAttack.cs`
- Stationary: does **not** use NavMesh
- Spawns `Arrow` prefab in player’s direction
- `Arrow.cs` checks:
  - If player is blocking → 0 damage
  - If not → 5 HP damage applied

---

### 4. Exploder (`ExploderAI.cs` + `ExploderAttack.cs`)

| Stat               | Value          |
| ------------------ | -------------- |
| Explosion Trigger  | Within 6 units |
| Damage             | 20 HP          |
| Delay Before Death | 5 seconds      |

- Uses custom AI + attack scripts
- Triggers explosion when player is close
- Plays `"attack01"` animation
- Ignores block state and applies damage immediately
- Destroys self after delay

---

### 5. Boss Enemy (`BossController.cs` + `BossAttack.cs`)

| Stat           | Value          |
| -------------- | -------------- |
| Health         | 100 HP         |
| Regular Attack | 15 HP          |
| Pound Attack   | 25 HP          |
| Cooldown       | 2.0 s          |
| Attack Range   | 3.0 units      |
| Spawns At      | 30-unit radius |

- Plays cinematic spawn animation when player approaches
- Randomly chooses between:
  - **Punch attack**: 15 HP
  - **Pound attack**: 25 HP
- Uses `LookAt` + `NavMeshAgent` for pathing
- On death:
  - Triggers win screen
  - Removes self from scene

---

## Enemy Health System (`AIStatus.cs`)

- Default health: **50 HP**
- Status tracked via `dead` flag
- Health updated via `ApplyDamage()`
- Death effects:
  - **Sir Stabs-a-Lot:** +15 HP to player
  - **Archer McStabby:** Triggers attack boost

### Health UI

- Each enemy has a child `HealthBar.cs` component
- Updated in real-time via:
  ```csharp
  healthBar.UpdateHealthBar(current, max);
  ```

## Shared Attack Interface

All enemy attack scripts implement this shared interface:

```csharp
public abstract class EnemyAttack : MonoBehaviour
{
    public float damage = 10.0f;
    public abstract void Attack(GameObject player);
}
```

### Custom Attack Scripts

- `PunchAttack.cs`

- `SwordAttack.cs`

- `ArcherAttack.cs`

- `ExploderAttack.cs`

- `BossAttack.cs`

Each script:

- Triggers enemy attack animations

- Applies damage with optional blocking logic

- Manages attack cooldowns using coroutines

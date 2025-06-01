# Power Ups and Items

This document details all collectible item logic in **steel-clash**.

## Health Packs

### 1. Environmental

- Script: `HealthPack.cs`
- +5 HP when touched
- Uses `OnTriggerEnter` with tag `"Player"`

### 2. Enemy Drops

- `SirStabsALot`: Drops +15 HP to player on death.

## Attack Boost

### Trigger

- Kill `Archer McStabby`

### Effect

- ×2 damage for 60 seconds

### Logic

- Coroutine: `AttackBoostRoutine(2f, 60f)`
- Triggered via `AIStatus.cs`

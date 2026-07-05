### H3Status 0.5.0
> **Supports H3VR 1.0 / Update 120!**
+ Added a configuration file (`BepInEx/config`)
+ Updated encryption icons for [TNHEncryptionTimer](https://github.com/TakingFire/H3Status/tree/main/Overlays#tnh-encryption-timer) 
* Refactored most of the plugin
* Moved to new dependencies, reducing plugin size by 75%
* **[Breaking]** Unified `event` types to camelCase
  * e.g. `TNHScoreEvent` → `tnhScoreEvent`, see [Protocol.md](https://github.com/TakingFire/H3Status/blob/main/Protocol.md#event-object)

### H3Status 0.4.0
+ Added a [New Overlay](https://github.com/TakingFire/H3Status/tree/main/Overlays#hl2-theme) with a familiar theme
+ Added `equipmentSeed` to `TNHLevelEvent`
- **[Breaking]** Removed `TNHLostStealthBonus` and `TNHLostNoHitBonus`
  - These are no longer used in the new scoring system
* **[Breaking]** Made the following changes to `TNHScoreEvent`:
  + Added `HoldPhaseHealthBonus`
  + Added `TakePhaseHealthBonus`
  + Added `TakeGuardClearSpeedBonus`
  - Removed `HoldWaveCompleteNoDamage`
  - Removed `HoldPhaseCompleteNoDamage`
  - Removed `TakeCompleteNoDamage`
  - Removed `TakeCompleteNoAlert`
  - Removed `HoldKillDistanceBonus`
  * Renamed `HoldKill` to `KillBonus`
  * Renamed `HoldHeadshotKill` to `HeadShotBonus`
  * Renamed `HoldMeleeKill` to `MeleeKillBonus`
  * Renamed `HoldJointBreak` to `JointBreakBonus`
  * Renamed `HoldJointSever` to `JointSeverBonus`
  * Renamed `HoldKillStreakBonus` to `KillStreakBonus`

### H3Status 0.3.0
+ Added a [New Overlay](https://github.com/TakingFire/H3Status/tree/main/Overlays#tnh-encryption-timer) for timing encryption clears
+ Added `TNHEncryptionDestroyed`
+ Added `scoreMultiplier` to `TNHLevelEvent`
+ Added `encryptionCount`, `encryptionTime` to `TNHHoldPhaseEvent`
  * **[Breaking]** Renamed `encryption` to `encryptionType`

### H3Status 0.2.0
+ Added `TNHLevelEvent`
+ Added `TNHTokenEvent`
+ Added game & mod version info
* **[Breaking]** Merged `playerDamage`, `playerHeal`, and `playerKill` into `healthEvent`
* **[Breaking]** Renamed `playerBuff` to `buffEvent`

### H3Status 0.1.1
+ Added a basic overlay template with comments
* Fixed lost hold stealth bonus being triggered by non-hold guards
* Rounded health values from decimal to integers

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

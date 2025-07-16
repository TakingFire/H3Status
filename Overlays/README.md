# Example Overlays

### [Basic Template](Template/)

A very simple overlay with detailed comments. If you are interested in building an overlay, start here.

### [TNH Score Log](TNHScoreLog/)

A Take and Hold focused overlay that uses all the information currently exposed by H3Status.\
Featuring a score counter, hold progress, and icons for every ammo type in the game.

> ![Preview Image](https://github.com/user-attachments/assets/2528f85c-0fa1-4e1d-9832-5d3e35604092)

**Direct URL: [tnh.overlays.bacur.xyz](https://tnh.overlays.bacur.xyz)**\
Download: [DownGit](https://downgit.github.io/#/home?url=https://github.com/TakingFire/H3Status/tree/main/Overlays/TNHScoreLog)

### [TNH Encryption Timer](TNHEncryptionTimer/)

A Take and Hold focused overlay that tracks time and score while clearing encryption nodes.\
Automatically hides itself when not in use.

> ![Preview Image](https://github.com/user-attachments/assets/90e101f0-3440-4cb4-a94b-e01626025383)

**Direct URL: [encryption.overlays.bacur.xyz](https://encryption.overlays.bacur.xyz)**\
Download: [DownGit](https://downgit.github.io/#/home?url=https://github.com/TakingFire/H3Status/tree/main/Overlays/TNHEncryptionTimer)

### [TNH Score Breakdown](https://github.com/quilavad/TNHScoreBreakdown) by [Quilavad](https://github.com/quilavad)

An extension to [TNHScoreLog](#tnh-score-log) that records and displays a detailed score breakdown upon run completion.\
Also includes audio cues that play when score bonuses are lost.\
**To Install:** follow the instructions on the overlay [homepage](https://github.com/quilavad/TNHScoreBreakdown).

Download: [DownGit](https://downgit.github.io/#/home?url=https://github.com/quilavad/TNHScoreBreakdown/tree/main)

## Usage

### In OBS

1. Create a new Browser Source.
2. Add the overlay URL to the source.
   - If downloading, check `Local file` and select `index.html`.
3. Set the Width and Height. A **minimum** of 400x250 is recommended.
4. To restart the connection, enable `Refresh browser when scene becomes active`, then hide/show the source.

## Configuring

If you download the overlay, you may configure it directly from [`config.js`](TNHScoreLog/config.js). The changes will be applied when the overlay is refreshed.

If using the direct URL, you may add any value in [`config.js`](TNHScoreLog/config.js) to the URL. For example, to hide the Ammo Panel and make the Event Log longer, you can use the following URL:\
[`tnh.overlays.bacur.xyz?showAmmoPanel=false&eventLogLength=12`](https://tnh.overlays.bacur.xyz?showAmmoPanel=false&eventLogLength=12)

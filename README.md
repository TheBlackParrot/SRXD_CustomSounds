# CustomSounds
**Replace Spin Rhythm XD's default sounds with your own!**

The mod will look in `Spin Rhythm/BepInEx/config/CustomSounds` for custom sound packs. Restart the game if you add or remove sound pack folders while the game is running.

Changing sound packs while in-game is supported via the added Mod Settings page. Entering *Default* as the active pack name will essentially disable the mod.

I've provided a few custom packs in the [Releases page](https://github.com/TheBlackParrot/SRXD_CustomSounds/releases).

## Dependencies
- SpinCore

## Pack setup
Create any of these sub-folders inside a folder within `Spin Rhythm/BepInEx/config/CustomSounds`:

> [!NOTE]
> All folders are optional, defaults will be used if the folder is not present.

### Default / Base Game triggers
These sound triggers are present in the game unmodded:

| Folder                        | Modifies                                                                                                                                                                             |
|-------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **AnnouncerReady**            | Announcer saying "Ready?" once a map loads                                                                                                                                           |
| **Announcer3**                | Announcer saying "3" for the map countdown                                                                                                                                           |
| **Announcer2**                | Announcer saying "2" for the map countdown                                                                                                                                           |
| **Announcer1**                | Announcer saying "1" for the map countdown                                                                                                                                           |
| **AnnouncerGo**               | Announcer saying "Go!" once the map starts                                                                                                                                           |
| **AnnouncerComplete**         | Announcer saying "Complete!" once the map ends                                                                                                                                       |
| **AnnouncerFullCombo**        | Announcer saying "Full combo!" if the map is finished with a full combo                                                                                                              |
| **AnnouncerPerfectFullCombo** | Announcer saying "Perfect full combo!" if the map is finished with a perfect full combo                                                                                              |
| **AnnouncerForRank**          | Announcer voice lines said on the results screen if no full combo is achieved, dependent on rank<br/><br/>**This folder expects to contain subfolders for each rank:** S, A, B, C, D |
| **MatchNoteHit**              | Hit sounds for match notes                                                                                                                                                           |
| **TapNoteHit**                | Hit sounds for tap notes                                                                                                                                                             |
| **BeatHit**                   | Hit sounds for beats                                                                                                                                                                 |
| **SpinHit**                   | Hit sounds for spinners                                                                                                                                                              |
| **ScratchHit**                | Hit sounds for scratchers                                                                                                                                                            |
| **UIHover**                   | Sound for hovering over UI elements                                                                                                                                                  |
| **UISelect**                  | Sound for pressing UI buttons                                                                                                                                                        |
| **UIBack**                    | Sound for navigating back one screen                                                                                                                                                 |
| **UIPlayHighlighted**         | Sound that plays when highlighting the PLAY button in the chart selection screen                                                                                                     |
| **UIConfirmPlay**             | Sound that plays when pressing the PLAY button, or replaying a map                                                                                                                   |
| **UIExperienceIncreasing**    | Sound that loops while experience is increasing in the results screen                                                                                                                |
| **UIExperienceStart**         | Sound that plays once experience begins increasing in the results screen                                                                                                             |
| **UILevelUp**                 | Sound that plays when leveling up                                                                                                                                                    |
| **EndSongCrowd**              | Background crowd sound that plays when no notes remain on a map                                                                                                                      |
| **CrowdFailure**              | Sound that plays when failing a map                                                                                                                                                  |
| **ResultsMedalImpact**        | Impact sound that plays when the medal is revealed for your rank on the results screen                                                                                               |
| **EditorClap**                | Clap sound used in the editor                                                                                                                                                        |

### Custom triggers
These triggers are added by the mod:

| Folder          | Event                                              |
|-----------------|----------------------------------------------------|
| **GamePaused**  | Pausing the game while in a map                    |
| **GameResumed** | Resuming from the pause menu while in a map        |
| **NoteMissed**  | Missing the first note in a string of missed notes |
| **OverbeatHit** | "Hitting" an overbeat                              |

The mod will look for audio files ending in `.mp3`, `.ogg`, and `.wav`. Empty folders will disable the sound entirely.
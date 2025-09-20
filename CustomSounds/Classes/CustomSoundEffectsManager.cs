using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace CustomSounds.Classes;

internal class CustomSoundEffectList
{
#pragma warning disable CS0649
    internal SoundEffect? MatchNoteHitSound;
    internal SoundEffect? TapNoteHitSound;
    internal SoundEffect? BeatHitSound;
    internal SoundEffect? SpinHitSound;
    internal SoundEffect? ScratchHitSound;
    
    internal SoundEffect? VoiceReadySound;
    internal SoundEffect? Voice3Sound;
    internal SoundEffect? Voice2Sound;
    internal SoundEffect? Voice1Sound;
    internal SoundEffect? VoiceGoSound;
    internal readonly Dictionary<string, SoundEffect?> VoiceRankSounds = new()
    {
        ["D"] = null,
        ["C"] = null,
        ["B"] = null,
        ["A"] = null,
        ["S"] = null
    };
    
    internal SoundEffect? UISelectSound;
    internal SoundEffect? UIBackSound;
    internal SoundEffect? UIHoverSound;
    internal SoundEffect? UIPlaySelectedSound;
    internal SoundEffect? UIConfirmPlaySound;
    internal SoundEffect? UIChangeDifficultySound;
    internal SoundEffect? UIExperienceIncreasingSound;
    internal SoundEffect? UIExperienceStartSound;
    internal SoundEffect? UILevelUpSound;
    internal SoundEffect? UICompleteSound;
    internal SoundEffect? UICompleteFullComboSound;
    internal SoundEffect? UICompletePerfectFullComboSound;
    internal SoundEffect? UICompleteFailedSound;
    
    internal SoundEffect? EndSongCrowdSound;
    internal SoundEffect? TrackCompleteCrowdSound;
#pragma warning restore CS0649

    internal CustomSoundEffectList() {}
    internal CustomSoundEffectList(SoundEffectAssets assets)
    {
        MatchNoteHitSound = assets.coloredNoteSoundEffect;
        TapNoteHitSound = assets.coloredHitNoteSoundEffect;
        BeatHitSound = assets.drumHitSoundEffect;
        SpinHitSound = assets.spinnerNoteSoundEffect;
        ScratchHitSound = assets.scratchNoteSoundEffect;
        
        VoiceReadySound = assets.voiceReadySound;
        Voice3Sound = assets.voice3Sound;
        Voice2Sound = assets.voice2Sound;
        Voice1Sound = assets.voice1Sound;
        VoiceGoSound = assets.voiceGoSound;
        
        UIHoverSound = assets.uiNavigationSelectionSound;
        UIBackSound = assets.backSound;
        UISelectSound = assets.forwardSound;
        UIConfirmPlaySound = assets.playSongSound;
        UIChangeDifficultySound = assets.changeDifficultySound;
        
        EndSongCrowdSound = assets.endSongCrowdSound;
        TrackCompleteCrowdSound = assets.trackCompleteCrowdSound;

        UIExperienceIncreasingSound = assets.expUpLoop;
        UIExperienceStartSound = assets.expUpStart;
        UILevelUpSound = assets.levelUpAchieved;

        UICompletePerfectFullComboSound = assets.levelCompletePFCSound;
        UICompleteFullComboSound = assets.levelCompleteFCSound;
        UICompleteFailedSound = assets.trackFailedSound;
        UICompleteSound = assets.trackCompleteSound;
        
        // can't use foreach here, results in an invalid operation since the enumerable can't be modified
        for(int idx = 0; idx < VoiceRankSounds.Count; idx++)
        {
            string rank = VoiceRankSounds.ElementAt(idx).Key;
            
            foreach (SoundEffectAssets.LevelCompleteRankSound levelCompleteRankSound in assets.levelCompleteRankSounds)
            {
                if (!levelCompleteRankSound.IsValidForRankStr(rank))
                {
                    continue;
                }
                
#if DEBUG
                Plugin.Log.LogInfo($"{rank} is valid for ({string.Join(", ", levelCompleteRankSound.ranks)})");
#endif
                VoiceRankSounds[rank] = levelCompleteRankSound.sound;
                break;
            }
        }
    }

    internal void SetSoundAssets()
    {
        CustomSoundEffectList defaults = CustomSoundEffectsManager.SoundEffectLists["Default"];
        
        SoundEffectAssets.Instance.coloredNoteSoundEffect = MatchNoteHitSound ?? defaults.MatchNoteHitSound!.Value;
        SoundEffectAssets.Instance.coloredHitNoteSoundEffect = TapNoteHitSound ?? defaults.TapNoteHitSound!.Value;
        SoundEffectAssets.Instance.drumHitSoundEffect = BeatHitSound ?? defaults.BeatHitSound!.Value;
        SoundEffectAssets.Instance.spinnerNoteSoundEffect = SpinHitSound ?? defaults.SpinHitSound!.Value;
        SoundEffectAssets.Instance.scratchNoteSoundEffect = ScratchHitSound ?? defaults.ScratchHitSound!.Value;
        
        SoundEffectAssets.Instance.voiceReadySound = VoiceReadySound ?? defaults.VoiceReadySound!.Value;
        SoundEffectAssets.Instance.voice3Sound = Voice3Sound ?? defaults.Voice3Sound!.Value;
        SoundEffectAssets.Instance.voice2Sound = Voice2Sound ?? defaults.Voice2Sound!.Value;
        SoundEffectAssets.Instance.voice1Sound = Voice1Sound ?? defaults.Voice1Sound!.Value;
        SoundEffectAssets.Instance.voiceGoSound = VoiceGoSound ?? defaults.VoiceGoSound!.Value;
        List<SoundEffectAssets.LevelCompleteRankSound> tempRankSoundList = [];
        tempRankSoundList.AddRange(VoiceRankSounds.Keys.Select(rank => new SoundEffectAssets.LevelCompleteRankSound()
            { ranks = [rank, $"{rank}+"], sound = VoiceRankSounds[rank] ?? defaults.VoiceRankSounds[rank]!.Value }));
        SoundEffectAssets.Instance.levelCompleteRankSounds = tempRankSoundList.ToArray();

        SoundEffectAssets.Instance.uiNavigationSelectionSound = UIHoverSound ?? defaults.UIHoverSound!.Value;
        SoundEffectAssets.Instance.backSound = UIBackSound ?? defaults.UIBackSound!.Value;
        SoundEffectAssets.Instance.forwardSound = UISelectSound ?? defaults.UISelectSound!.Value;
        SoundEffectAssets.Instance.playSongSound = UIConfirmPlaySound ?? defaults.UIConfirmPlaySound!.Value;
        
        SoundEffectAssets.Instance.changeDifficultySound = UIChangeDifficultySound ?? defaults.UIChangeDifficultySound!.Value;
        SoundEffectAssets.Instance.expUpLoop = UIExperienceIncreasingSound ?? defaults.UIExperienceIncreasingSound!.Value;
        SoundEffectAssets.Instance.expUpStart = UIExperienceStartSound ?? defaults.UIExperienceStartSound!.Value;
        SoundEffectAssets.Instance.levelUpAchieved = UILevelUpSound ?? defaults.UILevelUpSound!.Value;
        SoundEffectAssets.Instance.trackCompleteSound = UICompleteSound ?? defaults.UICompleteSound!.Value;
        SoundEffectAssets.Instance.levelCompleteFCSound = UICompleteFullComboSound ?? defaults.UICompleteFullComboSound!.Value;
        SoundEffectAssets.Instance.levelCompletePFCSound = UICompletePerfectFullComboSound ?? defaults.UICompletePerfectFullComboSound!.Value;
        SoundEffectAssets.Instance.trackFailedSound = UICompleteFailedSound ?? defaults.UICompleteFailedSound!.Value;

        SoundEffectAssets.Instance.endSongCrowdSound = EndSongCrowdSound ?? defaults.EndSongCrowdSound!.Value;
        SoundEffectAssets.Instance.trackCompleteCrowdSound = TrackCompleteCrowdSound ?? defaults.TrackCompleteCrowdSound!.Value;
    }
}

[HarmonyPatch]
internal static class CustomSoundEffectsManager
{
    internal static Dictionary<string, CustomSoundEffectList> SoundEffectLists = null!;

    internal static void Initialize()
    {
        SoundEffectLists = new Dictionary<string, CustomSoundEffectList> { ["Default"] = new(SoundEffectAssets.Instance) };
    }
    
    private static async Task<SoundEffect?> InitSoundEffectObject(string folder, float vol = 1f)
    {
        Plugin.Log.LogInfo($"Initializing sound effects folder: {folder}");
        string soundFolder = Path.Combine(Plugin.DataPath, folder);
        List<AudioClip> clips = [];

        if (!Directory.Exists(soundFolder))
        {
            return null;
        }

        IEnumerable<string> soundFiles = Directory.EnumerateFiles(soundFolder, "*", SearchOption.TopDirectoryOnly);
        
        foreach (string soundPath in soundFiles)
        {
            AudioType audioType;
            switch (Path.GetExtension(soundPath))
            {
                case ".aiff":
                    audioType = AudioType.AIFF;
                    break;
                
                case ".mp3":
                    audioType = AudioType.MPEG;
                    break;
                
                case ".ogg":
                    audioType = AudioType.OGGVORBIS;
                    break;
                
                case ".wav":
                    audioType = AudioType.WAV;
                    break;
                
                default:
                    continue;
            }
            
            // i'm guessing this has to be here for some funny windows security thing or something
            // if it's not here, sounds will randomly be empty. not null! but empty.
            await Awaitable.EndOfFrameAsync();
            
            // (i'm lazy) (sorry)
            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"file://{soundPath}", audioType);
            await request.SendWebRequest();
            
            await Awaitable.MainThreadAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Plugin.Log.LogError(request.error);
                continue;
            }

            AudioClip? clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Plugin.Log.LogError($"{Path.GetFileName(soundPath)} came back null? uhh...");
                continue;
            }

            clip.LoadAudioData();
            if (clip.loadState == AudioDataLoadState.Failed)
            {
                Plugin.Log.LogError($"{Path.GetFileName(soundPath)} failed to load? uhh...");
                continue;
            }
            if (clip.samples == 0)
            {
                Plugin.Log.LogError($"{Path.GetFileName(soundPath)} is empty? uhh...");
                continue;
            }
            
#if DEBUG
            Plugin.Log.LogInfo($"{Path.GetFileName(soundPath)} has {clip.samples} samples");
#endif
            clips.Add(clip);
        }
        
        Plugin.Log.LogInfo($"Found {clips.Count} audio clips");

        return new SoundEffect
        {
            clips = clips.ToArray(),
            volume = vol
        };
    }

    internal static async Task InitializeSounds(string packFolder, bool showNotifications = true)
    {
        Plugin.Log.LogInfo($"Initializing sound pack {packFolder}...");

        await Awaitable.MainThreadAsync();
        
        if (packFolder == "Default")
        {
            Plugin.Log.LogInfo("Wants the default sounds, no need to initialize");
            Plugin.ActivePackName.Value = "Default";
            SoundEffectLists["Default"].SetSoundAssets();
            return;
        }
        
        if (SoundEffectLists.TryGetValue(packFolder, out CustomSoundEffectList preExistingList))
        {
            Plugin.Log.LogInfo($"{packFolder} already initialized, no need to initialize again");
            Plugin.ActivePackName.Value = packFolder;
            preExistingList.SetSoundAssets();
            return;
        }

        if (Plugin.ValidPackNames.Contains(packFolder))
        {
            Plugin.Log.LogInfo($"{packFolder} doesn't exist yet, let's initialize it");
            SoundEffectLists[packFolder] = new CustomSoundEffectList();

            if (showNotifications)
            {
                NotificationSystemGUI.AddMessage($"Loading sound pack <b>{packFolder}</b>...");
            }
        }
        else
        {
            Plugin.Log.LogInfo("...well this is awkward, just use defaults");
            Plugin.ActivePackName.Value = "Default";
            SoundEffectLists["Default"].SetSoundAssets();
            return;
        }
        
        SoundEffectLists[packFolder].MatchNoteHitSound = await InitSoundEffectObject($"{packFolder}/MatchNoteHit", 0.75f);
        SoundEffectLists[packFolder].TapNoteHitSound = await InitSoundEffectObject($"{packFolder}/TapNoteHit");
        SoundEffectLists[packFolder].BeatHitSound = await InitSoundEffectObject($"{packFolder}/BeatHit");
        SoundEffectLists[packFolder].SpinHitSound = await InitSoundEffectObject($"{packFolder}/SpinHit");
        SoundEffectLists[packFolder].ScratchHitSound = await InitSoundEffectObject($"{packFolder}/ScratchHit");
        
        SoundEffectLists[packFolder].VoiceReadySound = await InitSoundEffectObject($"{packFolder}/AnnouncerReady");
        SoundEffectLists[packFolder].Voice3Sound = await InitSoundEffectObject($"{packFolder}/Announcer3");
        SoundEffectLists[packFolder].Voice2Sound = await InitSoundEffectObject($"{packFolder}/Announcer2");
        SoundEffectLists[packFolder].Voice1Sound = await InitSoundEffectObject($"{packFolder}/Announcer1");
        SoundEffectLists[packFolder].VoiceGoSound = await InitSoundEffectObject($"{packFolder}/AnnouncerGo");
        
        // can't use foreach here, results in an invalid operation since the enumerable can't be modified
        for (int idx = 0; idx < SoundEffectLists[packFolder].VoiceRankSounds.Count; idx++)
        {
            string rank = SoundEffectLists[packFolder].VoiceRankSounds.ElementAt(idx).Key;
            SoundEffectLists[packFolder].VoiceRankSounds[rank] = await InitSoundEffectObject($"{packFolder}/AnnouncerForRank/{rank}");
        }
        
        SoundEffectLists[packFolder].UIHoverSound = await InitSoundEffectObject($"{packFolder}/UIHover");
        SoundEffectLists[packFolder].UIBackSound = await InitSoundEffectObject($"{packFolder}/UIBack");
        SoundEffectLists[packFolder].UISelectSound = await InitSoundEffectObject($"{packFolder}/UISelect");
        
        SoundEffectLists[packFolder].UIPlaySelectedSound = await InitSoundEffectObject($"{packFolder}/UIPlaySelected");
        SoundEffectLists[packFolder].UIConfirmPlaySound = await InitSoundEffectObject($"{packFolder}/UIConfirmPlay");
        
        SoundEffectLists[packFolder].UIChangeDifficultySound = await InitSoundEffectObject($"{packFolder}/UIChangeDifficulty"); // unused?
        SoundEffectLists[packFolder].UIExperienceIncreasingSound = await InitSoundEffectObject($"{packFolder}/UIExperienceIncreasing");
        SoundEffectLists[packFolder].UIExperienceStartSound = await InitSoundEffectObject($"{packFolder}/UIExperienceStart");
        SoundEffectLists[packFolder].UILevelUpSound = await InitSoundEffectObject($"{packFolder}/UILevelUp");
        SoundEffectLists[packFolder].UICompleteSound = await InitSoundEffectObject($"{packFolder}/UIComplete");
        SoundEffectLists[packFolder].UICompleteFullComboSound = await InitSoundEffectObject($"{packFolder}/UICompleteFullCombo");
        SoundEffectLists[packFolder].UICompletePerfectFullComboSound = await InitSoundEffectObject($"{packFolder}/UICompletePerfectFullCombo");
        SoundEffectLists[packFolder].UICompleteFailedSound = await InitSoundEffectObject($"{packFolder}/UICompleteFailed"); // unused?

        SoundEffectLists[packFolder].EndSongCrowdSound = await InitSoundEffectObject($"{packFolder}/EndSongCrowd"); // unused?
        SoundEffectLists[packFolder].TrackCompleteCrowdSound = await InitSoundEffectObject($"{packFolder}/TrackCompleteCrowd"); // unused?
        
        Plugin.ActivePackName.Value = packFolder;
        SoundEffectLists[packFolder].SetSoundAssets();

        if (showNotifications)
        {
            NotificationSystemGUI.AddMessage($"Loaded sound pack <b>{packFolder}</b>!");
        }
    }
    
    [HarmonyPatch(typeof(SoundEffectAssets), nameof(SoundEffectAssets.GetSoundEffect))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void PatchColoredHitNoteSoundEffect(ref string soundEffectName, ref SoundEffect __result)
    {
        if (Plugin.ActivePackName.Value == "Default")
        {
            return;
        }

        CustomSoundEffectList soundList = SoundEffectLists[Plugin.ActivePackName.Value];
        
        switch (soundEffectName)
        {
            case "voiceReadySound":
                if (soundList.VoiceReadySound != null)
                {
                    __result = soundList.VoiceReadySound.Value;
                }
                return;
            
            case "voice3Sound":
                if (soundList.Voice3Sound != null)
                {
                    __result = soundList.Voice3Sound.Value;
                }
                return;
            
            case "voice2Sound":
                if (soundList.Voice2Sound != null)
                {
                    __result = soundList.Voice2Sound.Value;
                }
                return;
            
            case "voice1Sound":
                if (soundList.Voice1Sound != null)
                {
                    __result = soundList.Voice1Sound.Value;
                }
                return;
            
            case "voiceGoSound":
                if (soundList.VoiceGoSound != null)
                {
                    __result = soundList.VoiceGoSound.Value;
                }
                return;
            
            case "backSound":
                if (soundList.UIBackSound != null)
                {
                    __result = soundList.UIBackSound.Value;
                }
                return;
            
            case "PlaySelected":
                if (soundList.UIPlaySelectedSound != null)
                {
                    __result = soundList.UIPlaySelectedSound.Value;
                }
                return;
        }
        
#if DEBUG
        Plugin.Log.LogInfo($"SoundEffectAssets.GetSoundEffect -- {soundEffectName}");
#endif
    }
}
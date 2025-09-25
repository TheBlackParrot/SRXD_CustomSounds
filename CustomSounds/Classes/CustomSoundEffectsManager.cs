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
    internal SoundEffect? VoiceCompleteSound;
    internal SoundEffect? VoiceFullComboSound;
    internal SoundEffect? VoicePerfectFullComboSound;
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
    internal SoundEffect? UIPlayHighlightedSound;
    internal SoundEffect? UIConfirmPlaySound;
    internal SoundEffect? UIChangeDifficultySound;
    internal SoundEffect? UIExperienceIncreasingSound;
    internal SoundEffect? UIExperienceStartSound;
    internal SoundEffect? UILevelUpSound;
    internal SoundEffect? UICompleteSound;
    internal SoundEffect? UICompleteFullComboSound;
    internal SoundEffect? UICompletePerfectFullComboSound;
    
    internal SoundEffect? CrowdFailureSound;
    internal SoundEffect? ResultsMedalImpactSound;
    
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
        SoundEffectAssets assets = SoundEffectAssets.Instance;
        
        assets.coloredNoteSoundEffect = MatchNoteHitSound ?? defaults.MatchNoteHitSound!.Value;
        assets.coloredHitNoteSoundEffect = TapNoteHitSound ?? defaults.TapNoteHitSound!.Value;
        assets.drumHitSoundEffect = BeatHitSound ?? defaults.BeatHitSound!.Value;
        assets.spinnerNoteSoundEffect = SpinHitSound ?? defaults.SpinHitSound!.Value;
        assets.scratchNoteSoundEffect = ScratchHitSound ?? defaults.ScratchHitSound!.Value;
        
        assets.voiceReadySound = VoiceReadySound ?? defaults.VoiceReadySound!.Value;
        assets.voice3Sound = Voice3Sound ?? defaults.Voice3Sound!.Value;
        assets.voice2Sound = Voice2Sound ?? defaults.Voice2Sound!.Value;
        assets.voice1Sound = Voice1Sound ?? defaults.Voice1Sound!.Value;
        assets.voiceGoSound = VoiceGoSound ?? defaults.VoiceGoSound!.Value;
        List<SoundEffectAssets.LevelCompleteRankSound> tempRankSoundList = [];
        tempRankSoundList.AddRange(VoiceRankSounds.Keys.Select(rank => new SoundEffectAssets.LevelCompleteRankSound()
            { ranks = [rank, $"{rank}+"], sound = VoiceRankSounds[rank] ?? defaults.VoiceRankSounds[rank]!.Value }));
        assets.levelCompleteRankSounds = tempRankSoundList.ToArray();

        assets.uiNavigationSelectionSound = UIHoverSound ?? defaults.UIHoverSound!.Value;
        assets.backSound = UIBackSound ?? defaults.UIBackSound!.Value;
        assets.forwardSound = UISelectSound ?? defaults.UISelectSound!.Value;
        assets.playSongSound = UIConfirmPlaySound ?? defaults.UIConfirmPlaySound!.Value;
        
        assets.changeDifficultySound = UIChangeDifficultySound ?? defaults.UIChangeDifficultySound!.Value;
        assets.expUpLoop = UIExperienceIncreasingSound ?? defaults.UIExperienceIncreasingSound!.Value;
        assets.expUpStart = UIExperienceStartSound ?? defaults.UIExperienceStartSound!.Value;
        assets.levelUpAchieved = UILevelUpSound ?? defaults.UILevelUpSound!.Value;
        assets.trackCompleteSound = UICompleteSound ?? defaults.UICompleteSound!.Value;
        assets.levelCompleteFCSound = UICompleteFullComboSound ?? defaults.UICompleteFullComboSound!.Value;
        assets.levelCompletePFCSound = UICompletePerfectFullComboSound ?? defaults.UICompletePerfectFullComboSound!.Value;

        assets.endSongCrowdSound = EndSongCrowdSound ?? defaults.EndSongCrowdSound!.Value;
        assets.trackCompleteCrowdSound = TrackCompleteCrowdSound ?? defaults.TrackCompleteCrowdSound!.Value;
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

        CustomSoundEffectList? assets = SoundEffectLists[packFolder];
        
        assets.MatchNoteHitSound = await InitSoundEffectObject($"{packFolder}/MatchNoteHit", 0.75f);
        assets.TapNoteHitSound = await InitSoundEffectObject($"{packFolder}/TapNoteHit");
        assets.BeatHitSound = await InitSoundEffectObject($"{packFolder}/BeatHit");
        assets.SpinHitSound = await InitSoundEffectObject($"{packFolder}/SpinHit");
        assets.ScratchHitSound = await InitSoundEffectObject($"{packFolder}/ScratchHit");
        
        assets.VoiceReadySound = await InitSoundEffectObject($"{packFolder}/AnnouncerReady");
        assets.Voice3Sound = await InitSoundEffectObject($"{packFolder}/Announcer3");
        assets.Voice2Sound = await InitSoundEffectObject($"{packFolder}/Announcer2");
        assets.Voice1Sound = await InitSoundEffectObject($"{packFolder}/Announcer1");
        assets.VoiceGoSound = await InitSoundEffectObject($"{packFolder}/AnnouncerGo");
        assets.VoiceCompleteSound = await InitSoundEffectObject($"{packFolder}/AnnouncerComplete");
        assets.VoiceFullComboSound = await InitSoundEffectObject($"{packFolder}/AnnouncerFullCombo");
        assets.VoicePerfectFullComboSound = await InitSoundEffectObject($"{packFolder}/AnnouncerPerfectFullCombo");
        
        // can't use foreach here, results in an invalid operation since the enumerable can't be modified
        for (int idx = 0; idx < assets.VoiceRankSounds.Count; idx++)
        {
            string rank = assets.VoiceRankSounds.ElementAt(idx).Key;
            assets.VoiceRankSounds[rank] = await InitSoundEffectObject($"{packFolder}/AnnouncerForRank/{rank}");
        }
        
        assets.UIHoverSound = await InitSoundEffectObject($"{packFolder}/UIHover");
        assets.UIBackSound = await InitSoundEffectObject($"{packFolder}/UIBack");
        assets.UISelectSound = await InitSoundEffectObject($"{packFolder}/UISelect");
        
        assets.UIPlayHighlightedSound = await InitSoundEffectObject($"{packFolder}/UIPlayHighlighted");
        assets.UIConfirmPlaySound = await InitSoundEffectObject($"{packFolder}/UIConfirmPlay");
        
        assets.UIChangeDifficultySound = await InitSoundEffectObject($"{packFolder}/UIChangeDifficulty"); // unused?
        assets.UIExperienceIncreasingSound = await InitSoundEffectObject($"{packFolder}/UIExperienceIncreasing");
        assets.UIExperienceStartSound = await InitSoundEffectObject($"{packFolder}/UIExperienceStart");
        assets.UILevelUpSound = await InitSoundEffectObject($"{packFolder}/UILevelUp");
        assets.UICompleteSound = await InitSoundEffectObject($"{packFolder}/UIComplete"); // unused?
        assets.UICompleteFullComboSound = await InitSoundEffectObject($"{packFolder}/UICompleteFullCombo"); // unused?
        assets.UICompletePerfectFullComboSound = await InitSoundEffectObject($"{packFolder}/UICompletePerfectFullCombo"); // unused?
        
        assets.CrowdFailureSound = await InitSoundEffectObject($"{packFolder}/CrowdFailure");
        assets.ResultsMedalImpactSound = await InitSoundEffectObject($"{packFolder}/ResultsMedalImpact");

        assets.EndSongCrowdSound = await InitSoundEffectObject($"{packFolder}/EndSongCrowd"); // unused?
        assets.TrackCompleteCrowdSound = await InitSoundEffectObject($"{packFolder}/TrackCompleteCrowd"); // unused?
        
        Plugin.ActivePackName.Value = packFolder;
        assets.SetSoundAssets();

        if (showNotifications)
        {
            NotificationSystemGUI.AddMessage($"Loaded sound pack <b>{packFolder}</b>!");
        }
    }
    
#if DEBUG
    [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayOneShot))]
    [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayOneShotScheduled))]
    [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayLooping))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void TellMeWhichSoundsArePlayingPlease(ref SoundEffect soundEffect)
    {
        if (soundEffect.clips == null)
        {
            return;
        }
        if (soundEffect.clips[0] == null)
        {
            return;
        }
        
        Plugin.Log.LogInfo($"--> Playing sound {soundEffect.clips[0].name}");
    }
#endif
    
    [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayOneShot))]
    [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayOneShotScheduled))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void ChangeSoundsTheHardWay(ref SoundEffect soundEffect)
    {
        if (soundEffect.clips == null)
        {
            return;
        }
        if (soundEffect.clips[0] == null)
        {
            return;
        }

        CustomSoundEffectList? defaults = SoundEffectLists["Default"];
        CustomSoundEffectList? activePack = SoundEffectLists[Plugin.ActivePackName.Value];
        
        switch (soundEffect.clips[0].name)
        {
            // honestly i probably don't need to set defaults since null will just, not modify the sound, but like, you never know
            
            case "TrackCompleteXD":
                defaults.VoiceCompleteSound ??= soundEffect;
                soundEffect = activePack.VoiceCompleteSound ?? soundEffect;
                break;
            
            case "FC1":
                defaults.VoiceFullComboSound ??= soundEffect;
                soundEffect = activePack.VoiceFullComboSound ?? soundEffect;
                break;
            
            case "PFC1":
                defaults.VoicePerfectFullComboSound ??= soundEffect;
                soundEffect = activePack.VoicePerfectFullComboSound ?? soundEffect;
                break;
            
            case "UILevelUpStart":
                // wtf
                soundEffect = activePack.UIExperienceStartSound ?? soundEffect;
                break;
            
            case "UIResultsScoreMedalReveal":
                defaults.ResultsMedalImpactSound ??= soundEffect;
                soundEffect = activePack.ResultsMedalImpactSound ?? soundEffect;
                break;
            
            case "CrowdFailure1":
                defaults.CrowdFailureSound ??= soundEffect;
                soundEffect = activePack.CrowdFailureSound ?? soundEffect;
                break;
        }
    }

    [HarmonyPatch(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.PlayLooping))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ChangeLoopingSoundsTheHardWay(AudioSource __result, ref SoundEffect soundEffect)
    {
        if (soundEffect.clips == null)
        {
            return;
        }
        if (soundEffect.clips[0] == null)
        {
            return;
        }

        switch (soundEffect.clips[0].name)
        {
            case "UILevelUpLoop":
                // i hate looking at this. hate hate hate hate h
                __result.Stop();
                __result.clip = (SoundEffectLists[Plugin.ActivePackName.Value].UIExperienceIncreasingSound ?? soundEffect).clips.GetRandomElementOrDefault<AudioClip>();
                __result.Play();
                break;
        }
    }

    [HarmonyPatch(typeof(SoundEffectAssets), nameof(SoundEffectAssets.GetSoundEffect))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void PatchSoundEffectAssetsGet(ref string soundEffectName, ref SoundEffect __result)
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
                if (soundList.UIPlayHighlightedSound != null)
                {
                    __result = soundList.UIPlayHighlightedSound.Value;
                }
                return;
        }
        
#if DEBUG
        Plugin.Log.LogInfo($"SoundEffectAssets.GetSoundEffect -- {soundEffectName}");
#endif
    }
}
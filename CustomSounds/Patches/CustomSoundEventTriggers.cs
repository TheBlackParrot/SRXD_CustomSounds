using CustomSounds.Classes;
using HarmonyLib;
using UnityEngine;

namespace CustomSounds.Patches;

[HarmonyPatch]
public static class CustomSoundEventTriggers
{
    private static CustomSoundEffectList SoundList => CustomSoundEffectsManager.SoundEffectLists[Plugin.ActivePackName.Value];
    
    [HarmonyPatch(typeof(Track), nameof(Track.HandlePauseGame))]
    [HarmonyPostfix]
    private static void HandlePauseGamePostfix()
    {
        if (SoundList.GamePausedSound != null)
        {
            SoundEffectPlayer.Instance.PlayOneShot(SoundList.GamePausedSound.Value);
        }
    }
    
    [HarmonyPatch(typeof(Track), nameof(Track.HandleUnpauseGame))]
    [HarmonyPostfix]
    private static void HandleUnpauseGamePostfix()
    {
        if (SoundList.GameResumedSound != null)
        {
            SoundEffectPlayer.Instance.PlayOneShot(SoundList.GameResumedSound.Value);
        }
    }

    internal static NoteTimingAccuracy PreviousNoteTimingAccuracy = NoteTimingAccuracy.Pending;
    
    [HarmonyPatch(typeof(DomeHud), nameof(DomeHud.AddToAccuracyLog))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AddToAccuracyLogPostfix(DomeHud __instance, ref NoteTimingAccuracy accuracy)
    {
        if (__instance.PlayState.previewState is not PreviewState.NotPreview)
        {
            return;
        }
        
        switch (accuracy)
        {
            case NoteTimingAccuracy.Pending or NoteTimingAccuracy.Valid:
                return;
            case NoteTimingAccuracy.Failed when PreviousNoteTimingAccuracy is not NoteTimingAccuracy.Failed:
            {
                if (SoundList.NoteMissedSound != null)
                {
                    SoundEffectPlayer.Instance.PlayOneShot(SoundList.NoteMissedSound.Value);
                }

                break;
            }
        }

        PreviousNoteTimingAccuracy = accuracy;
    }
    
    [HarmonyPatch(typeof(ScoreState), nameof(ScoreState.AddOverbeat))]
    [HarmonyPostfix]
    private static void AddOverbeatPostfix()
    {
        if (SoundList.OverbeatHitSound != null)
        {
            SoundEffectPlayer.Instance.PlayOneShot(SoundList.OverbeatHitSound.Value);
        }
    }

    private static AudioSource? _healthLowAudioSource;
    private static float _healthLowTargetVolume;
    private static bool IsNearlyDead
    {
        get;
        set
        {
            bool oldValue = field;
            field = value;
            if (oldValue == value)
            {
                return;
            }

            if (SoundList.HealthRecoveredSound != null && !value && Track.IsPlaying && PlayState.Active.health >= 120)
            {
                SoundEffectPlayer.Instance.PlayOneShot(SoundList.HealthRecoveredSound.Value);
            }

            if (SoundList.HealthLowSound == null)
            {
                return;
            }
            if (_healthLowAudioSource == null)
            {
                _healthLowAudioSource = SoundEffectPlayer.Instance.PlayLooping(SoundList.HealthLowSound.Value, 0f);
            }
            _healthLowTargetVolume = (value ? 1f : 0f);
        }
    }
    private static bool IsDead
    {
        get;
        set
        {
            bool oldValue = field;
            field = value;
            if (oldValue == value)
            {
                return;
            }
            
            if (SoundList.DiedSound == null)
            {
                return;
            }

            if (value)
            {
                SoundEffectPlayer.Instance.PlayOneShot(SoundList.DiedSound.Value);
            }
        }
    }
    
    [HarmonyPatch(typeof(DomeHud), nameof(DomeHud.LateUpdate))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void DomeHud_LateUpdatePostfix(DomeHud __instance)
    {
        if (__instance.PlayState.previewState is not PreviewState.NotPreview)
        {
            return;
        }

        if (_healthLowAudioSource != null)
        {
            _healthLowAudioSource.volume =
                Mathf.Lerp(_healthLowAudioSource.volume, _healthLowTargetVolume, 0.1f);
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault (intentional)
        switch (__instance.PlayState.playStateStatus)
        {
            case PlayStateStatus.Failure:
            case PlayStateStatus.Success:
            case PlayStateStatus.None:
                IsNearlyDead = false;
                IsDead = false;
                return;
        }

        // the game loops through all animators on the DomeHud instance when setting these bool values, so it doesn't matter which one we select
        Animator animator = __instance.animators[0].animator;

        IsNearlyDead = animator.GetBool(DomeHud.Hashes.HealthLow) switch
        {
            true when !IsNearlyDead => true,
            false => false,
            _ => IsNearlyDead
        };
        IsDead = animator.GetBool(DomeHud.Hashes.HealthDied) switch
        {
            true when !IsDead => true,
            false => false,
            _ => IsDead
        };
    }
}
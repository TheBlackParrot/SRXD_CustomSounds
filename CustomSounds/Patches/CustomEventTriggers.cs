using CustomSounds.Classes;
using HarmonyLib;

namespace CustomSounds.Patches;

[HarmonyPatch]
public static class CustomSoundTriggers
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
    
    [HarmonyPatch(typeof(ScoreState), nameof(ScoreState.ClearMultiplier))]
    [HarmonyPostfix]
    private static void ClearMultiplierPostfix(bool wasFromOverbeat)
    {
        if (!wasFromOverbeat)
        {
            return;
        }
        
        if (SoundList.OverbeatHitSound != null)
        {
            SoundEffectPlayer.Instance.PlayOneShot(SoundList.OverbeatHitSound.Value);
        }
    }
}
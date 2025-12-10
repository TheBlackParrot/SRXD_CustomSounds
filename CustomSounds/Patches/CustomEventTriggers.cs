using CustomSounds.Classes;
using HarmonyLib;

namespace CustomSounds.Patches;

[HarmonyPatch]
public static class CustomSoundTriggers
{
    [HarmonyPatch(typeof(Track), nameof(Track.HandlePauseGame))]
    [HarmonyPostfix]
    private static void HandlePauseGamePostfix()
    {
        CustomSoundEffectList soundList = CustomSoundEffectsManager.SoundEffectLists[Plugin.ActivePackName.Value];
        if (soundList.GamePausedSound != null)
        {
            SoundEffectPlayer.Instance.PlayOneShot(soundList.GamePausedSound.Value);
        }
    }
}
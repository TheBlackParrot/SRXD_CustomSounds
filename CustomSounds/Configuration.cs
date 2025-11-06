using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.Configuration;
using CustomSounds.Classes;
using SpinCore.Translation;
using SpinCore.UI;
using UnityEngine;

namespace CustomSounds;

public partial class Plugin
{
    internal static ConfigEntry<string> ActivePackName = null!;
    internal static ConfigEntry<int> AudioBufferLength = null!;
    
    internal static ConfigEntry<float> MatchNoteVolumeMultiplier = null!;
    internal static ConfigEntry<float> TapNoteVolumeMultiplier = null!;
    internal static ConfigEntry<float> BeatVolumeMultiplier = null!;
    internal static ConfigEntry<float> SpinVolumeMultiplier = null!;
    internal static ConfigEntry<float> ScratchVolumeMultiplier = null!;
    
    private const string TRANSLATION_PREFIX = $"{nameof(CustomSounds)}_";

    internal static readonly List<string> ValidPackNames = ["Default"];

    private void RegisterConfigEntries()
    {
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}Name", nameof(CustomSounds));
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}Volume", "Volume Settings");
        TranslationHelper.AddTranslation($"{nameof(CustomSounds)}_GitHubButtonText", $"{nameof(CustomSounds)} Releases (GitHub)");
        
        ActivePackName =
            Config.Bind("General", nameof(ActivePackName), "Default", "Name of the active sound pack");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(ActivePackName)}", "Active sound pack");
        
        AudioBufferLength =
            Config.Bind("General", nameof(AudioBufferLength), 300,
                "Audio buffer length for hitsounds (higher = more sound dropouts, but longer audio cutoff; lower = less sound dropouts, but shorter audio cutoff)");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(AudioBufferLength)}", "Hitsound audio buffer length (ms)");
        
        MatchNoteVolumeMultiplier =
            Config.Bind("Volume", nameof(MatchNoteVolumeMultiplier), 0.8f, "Volume multiplier for match note hitsounds");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(MatchNoteVolumeMultiplier)}", "Match note volume multiplier");
        TapNoteVolumeMultiplier =
            Config.Bind("Volume", nameof(TapNoteVolumeMultiplier), 1f, "Volume multiplier for tap note hitsounds");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(TapNoteVolumeMultiplier)}", "Tap note volume multiplier");
        BeatVolumeMultiplier =
            Config.Bind("Volume", nameof(BeatVolumeMultiplier), 1f, "Volume multiplier for beat hitsounds");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(BeatVolumeMultiplier)}", "Beat volume multiplier");
        SpinVolumeMultiplier =
            Config.Bind("Volume", nameof(SpinVolumeMultiplier), 1f, "Volume multiplier for spinner hitsounds");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(SpinVolumeMultiplier)}", "Spinner volume multiplier");
        ScratchVolumeMultiplier =
            Config.Bind("Volume", nameof(ScratchVolumeMultiplier), 1f, "Volume multiplier for scratcher hitsounds");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(ScratchVolumeMultiplier)}", "Scratcher volume multiplier");
    }

    private static void CreateModPage()
    {
        foreach (string? packDirectory in Directory.EnumerateDirectories(DataPath, "*", SearchOption.TopDirectoryOnly))
        {
            string? packName = packDirectory?.Split(Path.DirectorySeparatorChar).Last();
            if (packName is null)
            {
                continue;
            }
            
            ValidPackNames.Add(packName);
        }

        CustomPage rootModPage = UIHelper.CreateCustomPage("ModSettings");
        rootModPage.OnPageLoad += RootModPageOnPageLoad;
        
        UIHelper.RegisterMenuInModSettingsRoot($"{TRANSLATION_PREFIX}Name", rootModPage);
    }

    private static void RootModPageOnPageLoad(Transform rootModPageTransform)
    {
        CustomGroup modGroup = UIHelper.CreateGroup(rootModPageTransform, nameof(CustomSounds));
        UIHelper.CreateSectionHeader(modGroup, "ModGroupHeader", $"{TRANSLATION_PREFIX}Name", false);
            
        #region ActivePackName
        CustomGroup activePackNameGroup = UIHelper.CreateGroup(modGroup, "ActivePackNameGroup");
        activePackNameGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(activePackNameGroup, "ActivePackNameLabel", $"{TRANSLATION_PREFIX}{nameof(ActivePackName)}");
        CustomInputField activePackNameInput = UIHelper.CreateInputField(activePackNameGroup, nameof(ActivePackName), (_, newValue) =>
        {
            if (newValue == ActivePackName.Value)
            {
                // erm
                return;
            }
            
            if (!ValidPackNames.Contains(newValue))
            {
                newValue = "Default";
                NotificationSystemGUI.AddMessage(
                    $"Invalid pack name!\n<size=67%>Valid names: <b>{string.Join(", ", ValidPackNames)}</b>", 15f);
            }
            
            ActivePackName.Value = newValue;
            Task.Run(async () =>
            {
                try
                {
                    Log.LogInfo($"Changing active sound pack to {ActivePackName.Value}...");
                    await CustomSoundEffectsManager.InitializeSounds(ActivePackName.Value);
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }
            });
        });
        activePackNameInput.InputField.SetText(ActivePackName.Value);
        #endregion
        
        #region AudioBufferLength
        CustomGroup audioBufferLengthGroup = UIHelper.CreateGroup(modGroup, "AudioBufferLengthGroup");
        audioBufferLengthGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(audioBufferLengthGroup, "AudioBufferLengthLabel", $"{TRANSLATION_PREFIX}{nameof(AudioBufferLength)}");
        CustomInputField audioBufferLengthInput = UIHelper.CreateInputField(audioBufferLengthGroup, nameof(AudioBufferLength), (_, newValue) =>
        {
            if (!int.TryParse(newValue, out int newIntValue))
            {
                return;
            }
            
            if (newIntValue == AudioBufferLength.Value)
            {
                return;
            }
            
            AudioBufferLength.Value = newIntValue;
            NotificationSystemGUI.AddMessage("A game restart is required for this change to take effect.", 6f);
        });
        audioBufferLengthInput.InputField.SetText(AudioBufferLength.Value.ToString());
        #endregion
        
        UIHelper.CreateSectionHeader(modGroup, "ModGroupHeader", $"{TRANSLATION_PREFIX}Volume", false);
        
        #region MatchNoteVolumeMultiplier
        CustomGroup matchNoteVolumeMultiplierGroup = UIHelper.CreateGroup(modGroup, "MatchNoteVolumeMultiplierGroup");
        matchNoteVolumeMultiplierGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(matchNoteVolumeMultiplierGroup, "MatchNoteVolumeMultiplierLabel", $"{TRANSLATION_PREFIX}{nameof(MatchNoteVolumeMultiplier)}");
        CustomInputField matchNoteVolumeMultiplierInput = UIHelper.CreateInputField(matchNoteVolumeMultiplierGroup,
            nameof(MatchNoteVolumeMultiplier), (_, newValue) =>
        {
            if (!float.TryParse(newValue, out float newFloatValue))
            {
                return;
            }
            
            if (Mathf.Approximately(newFloatValue, MatchNoteVolumeMultiplier.Value))
            {
                return;
            }
            
            MatchNoteVolumeMultiplier.Value = newFloatValue;
            CustomSoundEffectsManager.ModifyVolume(CustomSoundEffectsManager.DynamicVolumeSounds.MatchNoteHitSound, newFloatValue);
        });
        matchNoteVolumeMultiplierInput.InputField.SetText(MatchNoteVolumeMultiplier.Value.ToString(CultureInfo.CurrentCulture));
        #endregion
        
        #region TapNoteVolumeMultiplier
        CustomGroup tapNoteVolumeMultiplierGroup = UIHelper.CreateGroup(modGroup, "TapNoteVolumeMultiplierGroup");
        tapNoteVolumeMultiplierGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(tapNoteVolumeMultiplierGroup, "TapNoteVolumeMultiplierLabel", $"{TRANSLATION_PREFIX}{nameof(TapNoteVolumeMultiplier)}");
        CustomInputField tapNoteVolumeMultiplierInput = UIHelper.CreateInputField(tapNoteVolumeMultiplierGroup,
            nameof(TapNoteVolumeMultiplier), (_, newValue) =>
            {
                if (!float.TryParse(newValue, out float newFloatValue))
                {
                    return;
                }
            
                if (Mathf.Approximately(newFloatValue, TapNoteVolumeMultiplier.Value))
                {
                    return;
                }
            
                TapNoteVolumeMultiplier.Value = newFloatValue;
                CustomSoundEffectsManager.ModifyVolume(CustomSoundEffectsManager.DynamicVolumeSounds.TapNoteHitSound, newFloatValue);
            });
        tapNoteVolumeMultiplierInput.InputField.SetText(TapNoteVolumeMultiplier.Value.ToString(CultureInfo.CurrentCulture));
        #endregion
        
        #region BeatVolumeMultiplier
        CustomGroup beatVolumeMultiplierGroup = UIHelper.CreateGroup(modGroup, "BeatVolumeMultiplierGroup");
        beatVolumeMultiplierGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(beatVolumeMultiplierGroup, "BeatVolumeMultiplierLabel", $"{TRANSLATION_PREFIX}{nameof(BeatVolumeMultiplier)}");
        CustomInputField beatVolumeMultiplierInput = UIHelper.CreateInputField(beatVolumeMultiplierGroup,
            nameof(BeatVolumeMultiplier), (_, newValue) =>
            {
                if (!float.TryParse(newValue, out float newFloatValue))
                {
                    return;
                }
            
                if (Mathf.Approximately(newFloatValue, BeatVolumeMultiplier.Value))
                {
                    return;
                }
            
                BeatVolumeMultiplier.Value = newFloatValue;
                CustomSoundEffectsManager.ModifyVolume(CustomSoundEffectsManager.DynamicVolumeSounds.BeatHitSound, newFloatValue);
            });
        beatVolumeMultiplierInput.InputField.SetText(BeatVolumeMultiplier.Value.ToString(CultureInfo.CurrentCulture));
        #endregion
        
        #region SpinVolumeMultiplier
        CustomGroup spinVolumeMultiplierGroup = UIHelper.CreateGroup(modGroup, "SpinVolumeMultiplierGroup");
        spinVolumeMultiplierGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(spinVolumeMultiplierGroup, "SpinVolumeMultiplierLabel", $"{TRANSLATION_PREFIX}{nameof(SpinVolumeMultiplier)}");
        CustomInputField spinVolumeMultiplierInput = UIHelper.CreateInputField(spinVolumeMultiplierGroup,
            nameof(SpinVolumeMultiplier), (_, newValue) =>
            {
                if (!float.TryParse(newValue, out float newFloatValue))
                {
                    return;
                }
            
                if (Mathf.Approximately(newFloatValue, SpinVolumeMultiplier.Value))
                {
                    return;
                }
            
                SpinVolumeMultiplier.Value = newFloatValue;
                CustomSoundEffectsManager.ModifyVolume(CustomSoundEffectsManager.DynamicVolumeSounds.SpinHitSound, newFloatValue);
            });
        spinVolumeMultiplierInput.InputField.SetText(SpinVolumeMultiplier.Value.ToString(CultureInfo.CurrentCulture));
        #endregion
        
        #region ScratchVolumeMultiplier
        CustomGroup scratchVolumeMultiplierGroup = UIHelper.CreateGroup(modGroup, "ScratchVolumeMultiplierGroup");
        scratchVolumeMultiplierGroup.LayoutDirection = Axis.Horizontal;
        UIHelper.CreateLabel(scratchVolumeMultiplierGroup, "ScratchVolumeMultiplierLabel", $"{TRANSLATION_PREFIX}{nameof(ScratchVolumeMultiplier)}");
        CustomInputField scratchVolumeMultiplierInput = UIHelper.CreateInputField(scratchVolumeMultiplierGroup,
            nameof(ScratchVolumeMultiplier), (_, newValue) =>
            {
                if (!float.TryParse(newValue, out float newFloatValue))
                {
                    return;
                }
            
                if (Mathf.Approximately(newFloatValue, ScratchVolumeMultiplier.Value))
                {
                    return;
                }
            
                ScratchVolumeMultiplier.Value = newFloatValue;
                CustomSoundEffectsManager.ModifyVolume(CustomSoundEffectsManager.DynamicVolumeSounds.ScratchHitSound, newFloatValue);
            });
        scratchVolumeMultiplierInput.InputField.SetText(ScratchVolumeMultiplier.Value.ToString(CultureInfo.CurrentCulture));
        #endregion
        
        UIHelper.CreateButton(modGroup, $"Open{MyPluginInfo.PLUGIN_NAME}RepositoryButton", $"{nameof(CustomSounds)}_GitHubButtonText", () =>
        {
            Application.OpenURL($"https://github.com/{REPO_AUTHOR}/{REPO_NAME}/releases/latest");
        });
    }
}
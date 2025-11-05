using System;
using System.Collections.Generic;
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
    
    private const string TRANSLATION_PREFIX = $"{nameof(CustomSounds)}_";

    internal static readonly List<string> ValidPackNames = ["Default"];

    private void RegisterConfigEntries()
    {
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}Name", nameof(CustomSounds));
        TranslationHelper.AddTranslation($"{nameof(CustomSounds)}_GitHubButtonText", $"{nameof(CustomSounds)} Releases (GitHub)");
        
        ActivePackName =
            Config.Bind("General", nameof(ActivePackName), "Default", "Name of the active sound pack");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(ActivePackName)}", "Active sound pack");
        
        AudioBufferLength =
            Config.Bind("General", nameof(AudioBufferLength), 300,
                "Audio buffer length for hitsounds (higher = more sound dropouts, but longer audio cutoff; lower = less sound dropouts, but shorter audio cutoff)");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(AudioBufferLength)}", "Hitsound audio buffer length (ms)");
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
        
        UIHelper.CreateButton(modGroup, $"Open{MyPluginInfo.PLUGIN_NAME}RepositoryButton", $"{nameof(CustomSounds)}_GitHubButtonText", () =>
        {
            Application.OpenURL($"https://github.com/{REPO_AUTHOR}/{REPO_NAME}/releases/latest");
        });
    }
}
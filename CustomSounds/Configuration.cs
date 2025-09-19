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
    private const string TRANSLATION_PREFIX = $"{nameof(CustomSounds)}_";

    internal static readonly List<string> ValidPackNames = ["Default"];

    private void RegisterConfigEntries()
    {
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}Name", nameof(CustomSounds));
        
        ActivePackName =
            Config.Bind("General", nameof(ActivePackName), "Default", "Name of the active sound pack");
        TranslationHelper.AddTranslation($"{TRANSLATION_PREFIX}{nameof(ActivePackName)}", "Active sound pack");
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
    }
}
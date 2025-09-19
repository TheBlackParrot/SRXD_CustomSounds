using System;
using System.IO;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using CustomSounds.Classes;
using HarmonyLib;
using UnityEngine;

namespace CustomSounds;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("srxd.raoul1808.spincore", "1.1.2")]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log = null!;
    private static Harmony _harmony = null!;
    internal static string DataPath => Path.Combine(Paths.ConfigPath, nameof(CustomSounds));

    private void Awake()
    {
        Log = Logger;

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        
        RegisterConfigEntries();
        CreateModPage();
        
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        
        MainCamera.OnCurrentCameraChanged += MainCameraOnCurrentCameraChanged;
        
        Logger.LogInfo("Plugin loaded");
    }

    private void OnEnable()
    {
        _harmony.PatchAll();
    }

    private void OnDisable()
    {
        _harmony.UnpatchSelf();
    }

    private static void MainCameraOnCurrentCameraChanged(Camera obj)
    {
        MainCamera.OnCurrentCameraChanged -= MainCameraOnCurrentCameraChanged;
        CustomSoundEffectsManager.Initialize();
        
        Task.Run(async () =>
        {
            try
            {
                await CustomSoundEffectsManager.InitializeSounds(ActivePackName.Value, false);
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        });
    }
}
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using CustomSounds.Classes;
using CustomSounds.Patches;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomSounds;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("srxd.raoul1808.spincore", "1.1.2")]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log = null!;
    private static Harmony _harmony = null!;
    internal static string DataPath => Path.Combine(Paths.ConfigPath, nameof(CustomSounds));

    private const string REPO_NAME = $"SRXD_{MyPluginInfo.PLUGIN_NAME}";
    private const string REPO_AUTHOR = "TheBlackParrot";

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
        
        Task.Run(async () =>
        {
            try
            {
                await Awaitable.MainThreadAsync();
                await CustomSoundEffectsManager.Initialize();
                await CustomSoundEffectsManager.InitializeSounds(ActivePackName.Value, false);
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        });

        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.numRealVoices = int.MaxValue;
        config.numVirtualVoices = int.MaxValue;
        AudioSettings.SetConfiguration(config);

        Track.OnStartedPlayingTrack += (_, _) => CustomSoundTriggers.PreviousNoteTimingAccuracy = NoteTimingAccuracy.Pending;
    }

    private void OnDisable()
    {
        _harmony.UnpatchSelf();
    }

    private static void MainCameraOnCurrentCameraChanged(Camera obj)
    {
        MainCamera.OnCurrentCameraChanged -= MainCameraOnCurrentCameraChanged;
        
        Task.Run(async () =>
        {
            try
            {
                HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    $"{MyPluginInfo.PLUGIN_NAME}/{MyPluginInfo.PLUGIN_VERSION} (https://github.com/TheBlackParrot/{REPO_NAME})");
                HttpResponseMessage responseMessage = await httpClient.GetAsync(
                    $"https://api.github.com/repos/{REPO_AUTHOR}/{REPO_NAME}/releases/latest");
                responseMessage.EnsureSuccessStatusCode();
                string json = await responseMessage.Content.ReadAsStringAsync();

                ReleaseVersion? releaseVersion = JsonConvert.DeserializeObject<ReleaseVersion>(json);
                if (releaseVersion == null)
                {
                    Log.LogInfo("Could not get newest release version information");
                    return;
                }

                if (releaseVersion.Version == null)
                {
                    Log.LogInfo("Could not get newest release version information");
                    return;
                }

                if (releaseVersion.IsPreRelease)
                {
                    Log.LogInfo("Newest release version is a pre-release");
                    return;
                }

                Version currentVersion = new(MyPluginInfo.PLUGIN_VERSION);
                Version latestVersion = new(releaseVersion.Version);
#if DEBUG
                // just so we can see the notifications
                if (currentVersion != latestVersion)
#else
                if (currentVersion < latestVersion)
#endif
                {
                    Log.LogMessage(
                        $"{MyPluginInfo.PLUGIN_NAME} is out of date! (using v{currentVersion}, latest is v{latestVersion})");

                    await Awaitable.MainThreadAsync();
                    NotificationSystemGUI.AddMessage(
                        $"<b>{MyPluginInfo.PLUGIN_NAME}</b> has an update available! <alpha=#AA>(v{currentVersion} <alpha=#77>-> <alpha=#AA>v{latestVersion})\n<alpha=#FF><size=67%>See the shortcut button in the Mod Settings page to grab the latest update.",
                        15f);
                }
                else
                {
                    Log.LogMessage($"{MyPluginInfo.PLUGIN_NAME} is up to date!");
                }
            } catch(Exception e)
            {
                Log.LogError(e);
            }
        });
    }
}
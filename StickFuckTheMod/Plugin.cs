using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace StickFuckTheMod;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("StickFight.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    
    private void Start()
    {
        Plugin.Log = base.Logger;

        // Plugin startup logic
        Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        ControllerPatches.Patch(harmony);
		GameManagerPatches.Patch(harmony);
    }
}
